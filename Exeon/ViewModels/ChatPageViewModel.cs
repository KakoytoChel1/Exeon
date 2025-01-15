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

        // For scrolling behavior
        // Indicates the last element of the collection
        private int _lastSelectedIndex;
        public int LastSelectedIndex
        {
            get { return _lastSelectedIndex; }
            set { _lastSelectedIndex = value; OnPropertyChanged(); }
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
            LastSelectedIndex = MessageItems.Count - 1;

            CustomCommand? command = await FindRequestedCommandAsync(commandText);

            if (command != null)
            {
                ExecuteRequestedCommand(command);
            }
            else
            {
                MessageItems.Add(new AssistantSimpleMessageItem
                    ($"Не вдалось знайти команду за наступним запитом - \"{commandText}\".")
                { SendingTime = DateTime.Now });
                LastSelectedIndex = MessageItems.Count - 1;
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
            Action<bool, string> func = (result, details) =>
            {
                _dispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    if(result)
                    {
                        // If successfully completed
                        MessageItems.Add(new AssistantActionSucceededMessageItem() 
                        {SendingTime = DateTime.Now, Text = details});
                    }
                    else
                    {
                        // If failed
                        MessageItems.Add(new AssistantActionFailedMessageItem()
                        { SendingTime = DateTime.Now, Text = details });
                    }
                    LastSelectedIndex = MessageItems.Count - 1;
                });
            };

            Action<object> subFunc = (obj) =>
            {
                _dispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    if(obj is PauseAction pauseAction)
                    {
                        MessageItems.Add(new AssistantActionDelayMessageItem() 
                        { Delay = pauseAction.DelayInSeconds, SendingTime = DateTime.Now});
                    }
                });
            };

            if(command.Actions.Any())
            {
                command.Execute(func, subFunc);
            }
        }
        #endregion
    }
}
