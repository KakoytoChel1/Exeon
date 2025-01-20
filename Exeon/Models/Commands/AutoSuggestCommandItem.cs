using Exeon.ViewModels.Tools;

namespace Exeon.Models.Commands
{
    public class AutoSuggestCommandItem : ObservableObject
    {
        private CustomCommand? _command;
        public CustomCommand? CustomCommand
        {
            get { return _command; }
            set { _command = value; OnPropertyChanged(); } 
        }

        private string _title = null!;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        public override string ToString()
        {
            return $"{Title}";
        }
    }
}
