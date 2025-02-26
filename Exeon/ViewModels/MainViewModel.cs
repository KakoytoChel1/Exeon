using Exeon.Services;
using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;
using Exeon.Views.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Windows.Input;

namespace Exeon.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService,
            IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService)
            : base(appState, dispatcherQueueProvider, navigationService, configurationService, speechRecognitionService)
        {
            AppState.IsSidePanelButtonsEnabled = true;
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
                                    NavigationService.ChangePage<ChatPage>(SlideNavigationTransitionEffect.FromLeft);
                                    break;
                                case "Commands":
                                    NavigationService.ChangePage<CommandsPage>(SlideNavigationTransitionEffect.FromLeft);
                                    break;
                                case "Settings":
                                    NavigationService.ChangePage<SettingsPage>(SlideNavigationTransitionEffect.FromLeft);
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
