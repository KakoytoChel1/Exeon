using Exeon.Services;
using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;

namespace Exeon.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        public const int _navigationViewSidePanelAreaWidth = 48;

        public AppState AppState { get; private set; }
        protected INavigationService NavigationService { get; private set; }
        protected IConfigurationService ConfigurationService { get; private set; }
        protected ISpeechRecognitionService SpeechRecognitionService { get; private set; }

        protected DispatcherQueueProvider DispatcherQueueProvider { get; private set; }

        public ViewModelBase(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService, IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService)
        {
            AppState = appState;
            DispatcherQueueProvider = dispatcherQueueProvider;
            NavigationService = navigationService;
            ConfigurationService = configurationService;
            SpeechRecognitionService = speechRecognitionService;
        }
    }
}
