using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;
using Exeon.Views.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Windows.Input;

namespace Exeon.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        public AppState AppState { get; }

        public MainViewModel(INavigationService navigationService, AppState appState)
        {
            _navigationService = navigationService;
            AppState = appState;

            AppState.IsSidePanelButtonsEnabled = true;

            var test = AppState.IsSidePanelButtonsEnabled;
        }

        #region Properties

        #endregion

        #region Commands

        private ICommand? _navigatePageCommand;
        public ICommand NavigatePageCommand
        {
            get
            {
                if(_navigatePageCommand == null)
                {
                    _navigatePageCommand = new RelayCommand((obj) =>
                    {
                        if (obj is string tag)
                        {
                            switch (tag)
                            {
                                case "Chat":
                                    _navigationService.ChangePage<ChatPage>(SlideNavigationTransitionEffect.FromLeft);
                                    break;
                                case "Commands":
                                    _navigationService.ChangePage<CommandsPage>(SlideNavigationTransitionEffect.FromLeft);
                                    break;
                                case "Settings":
                                    _navigationService.ChangePage<SettingsPage>(SlideNavigationTransitionEffect.FromLeft);
                                    break;
                            }
                        }
                    });
                }
                return _navigatePageCommand;
            }
        }
        #endregion
    }
}
