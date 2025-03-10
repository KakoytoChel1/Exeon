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
            
        }

        #region Properties

        #endregion

        #region Commands

        #endregion
    }
}
