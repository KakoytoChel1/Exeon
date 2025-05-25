using Exeon.ViewModels.Tools;
using System.ComponentModel.DataAnnotations;

namespace Exeon.Models.Commands
{
    public class TriggerCommand : ObservableObject
    {
        public TriggerCommand() { }

        public TriggerCommand(string commandText)
        {
            CommandText = commandText;
        }

        [Key]
        public int Id { get; set; }

        private string _commandText = null!;
        public string CommandText
        {
            get { return _commandText; }
            set { _commandText = value; OnPropertyChanged(); }
        }

        public CustomCommand RootCommand { get; set; } = null!;
        public int RootCommandId { get; set; }
    }
}
