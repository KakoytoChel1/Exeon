using System;
using Exeon.Models.Chat;
using System.Collections.ObjectModel;
using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;
using System.Windows.Input;
using Exeon.Models.Commands;
using System.Threading.Tasks;
using System.Linq;
using Exeon.Services;
using Exeon.Models.Actions;

namespace Exeon.ViewModels
{
    public class ChatPageViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly DispatcherQueueProvider _dispatcherQueueProvider;
        public AppState AppState { get; }

        public ObservableCollection<MessageItem> MessageItems { get; set; }

        public ChatPageViewModel(INavigationService navigationService, AppState appState, DispatcherQueueProvider dispatcherQueueProvider)
        {
            AppState = appState;
            _navigationService = navigationService;
            _dispatcherQueueProvider = dispatcherQueueProvider;

            MessageItems = new ObservableCollection<MessageItem>();
        }

        #region Properties

        private string? _commandTextField;
        public string? CommandTextField
        {
            get { return _commandTextField; }
            set { _commandTextField = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        private ICommand? _sendEnteredTextCommand;
        public ICommand SendEnteredTextCommand
        {
            get
            {
                if( _sendEnteredTextCommand == null)
                {
                    _sendEnteredTextCommand = new RelayCommand((obj) =>
                    {
                        if (!string.IsNullOrWhiteSpace(CommandTextField))
                        {
                            var commandText = CommandTextField;
                            CommandTextField = string.Empty;
                            SendUserMessageAndStart(commandText);
                        }
                    });
                }
                return _sendEnteredTextCommand;
            }
        }
        #endregion

        #region Methods

        private async void SendUserMessageAndStart(string commandText)
        {
            MessageItems.Add(new UserMessageItem(commandText) { SendingTime = DateTime.Now });

            CustomCommand? command = await FindRequestedCommandAsync(commandText);

            if (command != null)
            {
                ExecuteRequestedCommand(command);
            }
            else
            {
                var messageItem = new AssistantMessageItem();
                messageItem.MessageItems.Add(new AssistantSimpleMessageItem
                    ($"Не вдалось знайти команду за наступним запитом - \"{commandText}\".")
                { SendingTime = DateTime.Now });

                MessageItems.Add(messageItem);
            }
        }

        private async Task<CustomCommand?> FindRequestedCommandAsync(string requestedCommand)
        {
            CustomCommand? command = null;

            await Task.Run(() =>
            {
                command = AppState.CustomCommands.FirstOrDefault(c => c.Command.ToLower() == requestedCommand);
            });

            return command;
        }

        private void ExecuteRequestedCommand(CustomCommand command)
        {
            AssistantMessageItem messageItem = null!;

            Action<bool, string> func = (result, details) =>
            {
                _dispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    if(result)
                    {
                        // If successfully completed
                        messageItem.MessageItems.Add(new AssistantActionSucceededMessageItem() 
                        {SendingTime = DateTime.Now, Text = details});
                    }
                    else
                    {
                        // If failed
                        messageItem.MessageItems.Add(new AssistantActionFailedMessageItem()
                        { SendingTime = DateTime.Now, Text = details });
                    }
                });
            };

            Action<object> subFunc = (obj) =>
            {
                _dispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    if (obj is AssistantActionDelayMessageItem delayMessageItem)
                    {
                        messageItem.MessageItems.Add(delayMessageItem);
                        //await delayMessageItem.StartDelayAsync();
                    }
                });
            };

            if (command.Actions.Any())
            {
                messageItem = new AssistantMessageItem();
                MessageItems.Add(messageItem);
                command.Execute(func, subFunc);
            }
        }
        #endregion
    }
}
