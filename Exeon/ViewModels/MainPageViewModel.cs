using Exeon.Services.IServices;
using Exeon.Services;
using System.Windows.Input;
using Exeon.ViewModels.Tools;
using Exeon.Views.Pages;
using System;
using System.IO;

namespace Exeon.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService,
            IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService)
            : base(appState, dispatcherQueueProvider, navigationService, configurationService, speechRecognitionService)
        {
            AppState.IsSidePanelButtonsEnabled = true;
        }

        #region Commands

        private ICommand? _reloadSpeechModelCommand;
        public ICommand ReloadSpeechModelCommand
        {
            get
            {
                _reloadSpeechModelCommand = new RelayCommand(async (obj) =>
                {
                    var extractedPathToModel = ConfigurationService.Get<string>("SpeechModelPath");
                    if (!string.IsNullOrWhiteSpace(extractedPathToModel) && Directory.Exists(extractedPathToModel))
                    {
                        NavigationService.ChangePage<LoadingPage>();
                        await SpeechRecognitionService.InitializeSpeechModel(extractedPathToModel);

                        AppState.IsSpeechModelInitializingFailed = !SpeechRecognitionService.IsInitialized;
                        AppState.IsSpeechModelWarningVisible = false;

                        NavigationService.ChangePage<MainPage>();
                    }
                    else
                    {
                        AppState.IsSpeechModelInitializingFailed = true;
                    }
                });
                return _reloadSpeechModelCommand;
            }
        }
        #endregion
    }
}
