using Exeon.Models.Actions;
using Exeon.ViewModels.Tools;
using System.Collections.ObjectModel;

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

        private ObservableCollection<Action> _actions = new ObservableCollection<Action>();
        public virtual ObservableCollection<Action> Actions
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
    }
}
