﻿using Exeon.Models.Actions;
using Exeon.Models.Chat;
using Exeon.ViewModels.Tools;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Exeon.Models.Commands
{
    public class CustomCommand : ObservableObject
    {
        public CustomCommand() { }

        [Key]
        public int Id { get; set; }

        public int OrderIndex { get; set; }

        private ObservableCollection<TriggerCommand> triggerCommands = new ObservableCollection<TriggerCommand>();
        public ObservableCollection<TriggerCommand> TriggerCommands
        {
            get { return triggerCommands; }
            set
            {
                if (triggerCommands != value)
                {
                    triggerCommands = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Actions.Action> _actions = new ObservableCollection<Actions.Action>();
        public ObservableCollection<Actions.Action> Actions
        {
            get { return _actions; }
            set
            {
                if( _actions != value )
                {
                    _actions = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AddTriggerCommand(TriggerCommand triggerCommand)
        {
            triggerCommand.RootCommand = this;
            triggerCommand.RootCommandId = Id;

            TriggerCommands.Add(triggerCommand);
            OnPropertyChanged(nameof(TriggerCommands));
        }

        public async Task Execute(Action<bool, string> sendSucccesOrFailedMessage, Action<object> sendDelayMessage, 
            Action<double> changeCommandExecutionProgressBarValue, CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                for(int i = 0; i < _actions.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await Task.Delay(100, cancellationToken);

                    if (_actions[i] is PauseAction pauseAction)
                    {
                        var delayMessageItem = new AssistantActionDelayMessageItem
                        {
                            Delay = pauseAction.DelayInSeconds,
                            Text = $"Пауза на {pauseAction.DelayInSeconds} секунд."
                        };

                        sendDelayMessage.Invoke(delayMessageItem);
                        await delayMessageItem.StartDelayAsync(cancellationToken);

                        changeCommandExecutionProgressBarValue.Invoke(((i + 1) * 100) / _actions.Count);
                        continue;
                    }

                    var result = await _actions[i].Execute();

                    sendSucccesOrFailedMessage.Invoke(result.Item1, result.Item2);

                    changeCommandExecutionProgressBarValue.Invoke(((i + 1) * 100) / _actions.Count);
                }
            }, cancellationToken);
        }
    }
}
