using Exeon.Models.Actions;
using Exeon.Models.Chat;
using Exeon.ViewModels.Tools;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Exeon.Models.Commands
{
    public class CustomCommand : ObservableObject
    {
        public CustomCommand() { }

        public CustomCommand(string command)
        {
            Command = command;
        }

        public int Id { get; set; }

        private string _command = null!;
        public string Command
        {
            get { return _command; }
            set { _command = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Actions.Action> _actions = new ObservableCollection<Actions.Action>();
        public virtual ObservableCollection<Actions.Action> Actions
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

        public async void Execute(Action<bool, string> func, Action<object> subFunc)
        {
            await Task.Run(async () =>
            {
                foreach(Actions.Action action in _actions)
                {
                    await Task.Delay(100);

                    if(action is PauseAction pauseAction)
                    {
                        subFunc.Invoke(pauseAction);
                        await action.Execute();
                        continue;
                    }

                    ValueTuple<bool, string> result = await action.Execute();

                    func.Invoke(result.Item1, result.Item2);
                }
            });
        }
    }
}
