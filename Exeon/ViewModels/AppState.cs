using Exeon.Models;
using Exeon.Models.Commands;
using Exeon.ViewModels.Tools;
using System.Collections.ObjectModel;

namespace Exeon.ViewModels
{

    /// <summary>
    /// Stores global collections, properties, commands etc.
    /// </summary>
    public class AppState : ObservableObject
    {
        public const int _navigationViewSidePanelAreaWidth = 48;

        public AppState()
        {
            //ApplicationContext = new ApplicationContext();
            CustomCommands = new ObservableCollection<CustomCommand>();
        }

        #region Properties

        public ObservableCollection<CustomCommand> CustomCommands { get; set; }

        //public ApplicationContext ApplicationContext { get; private set; }

        private bool _isSidePanelButtonsEnabled;
        public bool IsSidePanelButtonsEnabled
        {
            get { return _isSidePanelButtonsEnabled; }
            set { _isSidePanelButtonsEnabled = value; OnPropertyChanged(); }
        }

        private CustomCommand? _selectedModifyingCustomCommand;
        public CustomCommand? SelectedModifyingCustomCommand
        {
            get { return _selectedModifyingCustomCommand; }
            set { _selectedModifyingCustomCommand = value; OnPropertyChanged(); }
        }
        #endregion
    }
}
