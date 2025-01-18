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
using Microsoft.Xaml.Interactivity;
using System.Threading;

namespace Exeon.ViewModels
{
    public class ChatPageViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly DispatcherQueueProvider _dispatcherQueueProvider;
        private CancellationTokenSource? _cts;
        public AppState AppState { get; }

        public ObservableCollection<MessageItem> MessageItems { get; set; }

        public ChatPageViewModel(INavigationService navigationService, AppState appState, DispatcherQueueProvider dispatcherQueueProvider)
        {
            AppState = appState;
            _navigationService = navigationService;
            _dispatcherQueueProvider = dispatcherQueueProvider;

            IsEnterCommandPanelEnabled = true;

            MessageItems = new ObservableCollection<MessageItem>();
        }

        #region Properties

        private string? _commandTextField;
        public string? CommandTextField
        {
            get { return _commandTextField; }
            set { _commandTextField = value; OnPropertyChanged(); }
        }

        private bool _IsEnterCommandPanelEnabled;
        public bool IsEnterCommandPanelEnabled
        {
            get { return _IsEnterCommandPanelEnabled; }
            set { _IsEnterCommandPanelEnabled = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands

        private ICommand? _sendEnteredTextCommand;
        public ICommand SendEnteredTextCommand
        {
            get
            {
                if (_sendEnteredTextCommand == null)
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

        private ICommand? _cancelCommandExecutionCommand;
        public ICommand CancelCommandExecutionCommand
        {
            get
            {
                if(_cancelCommandExecutionCommand == null)
                {
                    _cancelCommandExecutionCommand = new RelayCommand((obj) =>
                    {
                        if(_cts != null)
                        {
                            _cts.Cancel();
                            AppState.IsCommandRunning = false;
                            IsEnterCommandPanelEnabled = true;
                            AppState.CommandExecutionProgress = 0;
                        }
                    });
                }
                return _cancelCommandExecutionCommand;
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
                AppState.IsCommandRunning = true;
                IsEnterCommandPanelEnabled = false;
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

        private async void ExecuteRequestedCommand(CustomCommand command)
        {
            AssistantMessageItem messageItem = null!;
            _cts = new CancellationTokenSource();

            Action<bool, string> sendSucccesOrFailedMessage = (result, details) =>
            {
                _dispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    if (result)
                    {
                        // If successfully completed
                        messageItem.MessageItems.Add(new AssistantActionSucceededMessageItem()
                        { SendingTime = DateTime.Now, Text = details });
                    }
                    else
                    {
                        // If failed
                        messageItem.MessageItems.Add(new AssistantActionFailedMessageItem()
                        { SendingTime = DateTime.Now, Text = details });
                    }
                });
            };

            Action<object> sendDelayMessage = (obj) =>
            {
                _dispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    if (obj is AssistantActionDelayMessageItem delayMessageItem)
                    {
                        messageItem.MessageItems.Add(delayMessageItem);
                    }
                });
            };

            Action<double> changeCommandExecutionProgressBarValue = (value) =>
            {
                _dispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    AppState.CommandExecutionProgress = value;
                });
            };

            if (command.Actions.Any())
            {
                messageItem = new AssistantMessageItem() { SendingTime = DateTime.Now };
                MessageItems.Add(messageItem);

                try
                {
                    await command.Execute(sendSucccesOrFailedMessage, sendDelayMessage, changeCommandExecutionProgressBarValue, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    messageItem.MessageItems.Add(new AssistantActionFailedMessageItem()
                    { SendingTime = DateTime.Now, Text = "Виконання команди було скасовано." });
                }
                finally
                {
                    AppState.IsCommandRunning = false;
                    IsEnterCommandPanelEnabled = true;
                    AppState.CommandExecutionProgress = 0;
                }
            }
        }
        #endregion
    }
}