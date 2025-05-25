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
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;

namespace Exeon.ViewModels
{
    public class ChatPageViewModel : ViewModelBase
    {
        private CancellationTokenSource? _cts;

        public ObservableCollection<MessageItem> MessageItems { get; set; }

        public ChatPageViewModel(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService,
            IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService)
            : base(appState, dispatcherQueueProvider, navigationService, configurationService, speechRecognitionService)
        {
            SpeechRecognitionService.FinalRecognition += _speechRecognitionService_FinalRecognition;
            SpeechRecognitionService.PartialRecognition += _speechRecognitionService_PartialRecognition;

            IsEnterCommandPanelEnabled = true;

            MessageItems = new ObservableCollection<MessageItem>();
        }

        private void _speechRecognitionService_PartialRecognition(object? sender, string result)
        {
            DispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
            {
                Dictionary<string, string>? value = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                if(value != null)
                {
                    CommandTextField = value["partial"];
                }
            });
        }

        private void _speechRecognitionService_FinalRecognition(object? sender, string result)
        {
            DispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
            {
                Dictionary<string, string>? value = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                if (value != null)
                {
                    CommandTextField = value["text"];

                    _cts?.Cancel();
                    SpeechRecognitionService.StopRecognition();
                    AppState.IsListening = false;

                    SendAndExecuteUserCommand(CommandTextField);
                }
            });
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
                        // dont send command when: it is null or listening is active
                        if (!string.IsNullOrWhiteSpace(CommandTextField))
                        {
                            var commandText = CommandTextField;
                            CommandTextField = string.Empty;
                            SendAndExecuteUserCommand(commandText);
                        }
                    }, (p) => !AppState.IsListening && !AppState.IsCommandRunning);
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

        private ICommand? _startListenCommand;
        public ICommand StartListenCommand
        {
            get
            {
                if( _startListenCommand == null)
                {
                    _startListenCommand = new RelayCommand(async (obj) =>
                    {
                        _cts = new CancellationTokenSource();

                        await Task.Run(() =>
                        {
                            SpeechRecognitionService.StartRecognitionAsync(_cts.Token);
                        });

                        await Task.Delay(100);
                        AppState.IsListening = true;

                    }, (obj) => !AppState.IsListening);
                }
                return _startListenCommand;
            }
        }

        private ICommand? _stopListenCommand;
        public ICommand StopListenCommand
        {
            get
            {
                if(_stopListenCommand == null)
                {
                    _stopListenCommand = new RelayCommand((obg) =>
                    {
                        AppState.IsListening = false;
                        _cts?.Cancel();
                        SpeechRecognitionService.StopRecognition();

                    });
                }
                return _stopListenCommand;
            }
        }

        private ICommand? _cleanUpMessagesHistoryCommand;
        public ICommand CleanUpMessagesHistoryCommand
        {
            get
            {
                if(_cleanUpMessagesHistoryCommand == null)
                {
                    _cleanUpMessagesHistoryCommand = new RelayCommand(async (obj) =>
                    {
                        if(!AppState.IsCommandRunning && !AppState.IsListening && MessageItems.Any())
                        {
                            var xamlRoot = App.MainWindow.Content.XamlRoot;

                            var result = await DialogManager.ShowContentDialog(xamlRoot, "Очищення чату", "Очистити",
                                ContentDialogButton.Primary, "Ви дійсно бажаєте очистити історію всіх відправлених команд?", closeBtnText: "Скасувати");

                            if (result == ContentDialogResult.Primary)
                            {
                                MessageItems.Clear();
                            }
                        }
                    });
                }
                return _cleanUpMessagesHistoryCommand;
            }
        }
        #endregion

        #region Methods

        private async void SendAndExecuteUserCommand(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
                return;

            MessageItems.Add(new UserMessageItem(commandText));

            CustomCommand? command = await FindRequestedCommandAsync(commandText);

            if (command != null)
            {
                if (command.Actions.Any())
                {
                    AppState.IsCommandRunning = true;
                    IsEnterCommandPanelEnabled = false;
                    ExecuteRequestedCommand(command);
                }
                else
                {
                    var messageItem = new AssistantMessageItem();
                    messageItem.MessageItems.Add(new AssistantSimpleMessageItem
                        ($"Виконання команди - \"{commandText}\" скасовано, у команді відсутні дії."));

                    MessageItems.Add(messageItem);
                    CommandTextField = string.Empty;
                }
            }
            else
            {
                var messageItem = new AssistantMessageItem();
                messageItem.MessageItems.Add(new AssistantSimpleMessageItem
                    ($"Не вдалось знайти команду за наступним запитом - \"{commandText}\"."));

                MessageItems.Add(messageItem);
                CommandTextField = string.Empty;
            }
        }

        private async Task<CustomCommand?> FindRequestedCommandAsync(string requestedCommand)
        {
            CustomCommand? command = null;

            //await Task.Run(() =>
            //{
            //    command = AppState.CustomCommands.FirstOrDefault(c => c.Command.ToLower() == requestedCommand);
            //});

            return command;
        }

        private async void ExecuteRequestedCommand(CustomCommand command)
        {
            AssistantMessageItem messageItem = null!;
            _cts = new CancellationTokenSource();

            Action<bool, string> sendSucccesOrFailedMessage = (result, details) =>
            {
                DispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    if (result)
                    {
                        // If successfully completed
                        messageItem.MessageItems.Add(new AssistantActionSucceededMessageItem() { Text = details});
                    }
                    else
                    {
                        // If failed
                        messageItem.MessageItems.Add(new AssistantActionFailedMessageItem() { Text = details });
                    }
                });
            };

            Action<object> sendDelayMessage = (obj) =>
            {
                DispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    if (obj is AssistantActionDelayMessageItem delayMessageItem)
                    {
                        messageItem.MessageItems.Add(delayMessageItem);
                    }
                });
            };

            Action<double> changeCommandExecutionProgressBarValue = (value) =>
            {
                DispatcherQueueProvider.DispatcherQueue!.TryEnqueue(() =>
                {
                    AppState.CommandExecutionProgress = value;
                });
            };

            if (command.Actions.Any())
            {
                messageItem = new AssistantMessageItem();
                MessageItems.Add(messageItem);

                try
                {
                    await command.Execute(sendSucccesOrFailedMessage, sendDelayMessage, changeCommandExecutionProgressBarValue, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    messageItem.MessageItems.Add(new AssistantActionFailedMessageItem()
                    { Text = "Виконання команди було скасовано." });
                }
                finally
                {
                    AppState.IsCommandRunning = false;
                    IsEnterCommandPanelEnabled = true;
                    AppState.CommandExecutionProgress = 0;
                    CommandTextField =  string.Empty;
                }
            }
        }
        #endregion
    }
}