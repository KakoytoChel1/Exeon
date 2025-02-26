using Exeon.Services;
using Exeon.Services.IServices;

namespace Exeon.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPageViewModel(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService, 
            IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService) 
            : base (appState, dispatcherQueueProvider, navigationService, configurationService, speechRecognitionService)
        {
        }
    }
}
