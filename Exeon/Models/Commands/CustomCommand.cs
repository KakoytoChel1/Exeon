using Exeon.Models.Actions;
using System.Collections.ObjectModel;

namespace Exeon.Models.Commands
{
    public class CustomCommand
    {
        public CustomCommand(string command)
        {
            Command = command;
        }

        public int Id { get; set; }

        public string Command { get; set; } = null!;

        public ObservableCollection<Action> Actions { get; set; } = new ObservableCollection<Action>(); 
    }
}
