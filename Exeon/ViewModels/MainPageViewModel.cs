using Exeon.Services.IServices;
using Exeon.Services;

namespace Exeon.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService,
            IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService)
            : base(appState, dispatcherQueueProvider, navigationService, configurationService, speechRecognitionService)
        {

        }
    }
}
