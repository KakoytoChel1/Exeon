using Exeon.Services;
using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;
using System;
using System.Windows.Input;
using Windows.Storage.Pickers;

namespace Exeon.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPageViewModel(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService, 
            IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService) 
            : base (appState, dispatcherQueueProvider, navigationService, configurationService, speechRecognitionService)
        {
            SpeechRecognitionModelPath = ConfigurationService.Get<string>("SpeechModelPath");
        }

        #region Properties

        private bool _isSelecting;
        public bool IsSelecting
        {
            get { return _isSelecting; }
            set {  _isSelecting = value; OnPropertyChanged(); }
        }

        private string? _speechRecognitionModelPath;
        public string? SpeechRecognitionModelPath
        {
            get { return _speechRecognitionModelPath; }
            set { _speechRecognitionModelPath = value; OnPropertyChanged(); }
        }

        private bool _unsavedChangesExist;
        public bool UnsavedChangesExist
        {
            get { return _unsavedChangesExist; }
            set { _unsavedChangesExist = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands

        private ICommand? _selectSpeechRecognitionModelPathCommand;
        public ICommand? SelectSpeechRecognitionModelPathCommand
        {
            get
            {
                if(_selectSpeechRecognitionModelPathCommand == null)
                {
                    _selectSpeechRecognitionModelPathCommand = new RelayCommand(async (obj) =>
                    {
                        IsSelecting = true;

                        var openPicker = new FolderPicker();

                        var window = App.MainWindow;

                        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

                        openPicker.ViewMode = PickerViewMode.Thumbnail;
                        openPicker.FileTypeFilter.Add("*");

                        var file = await openPicker.PickSingleFolderAsync();
                        if (file != null)
                        {
                            SpeechRecognitionModelPath = file.Path;
                            UnsavedChangesExist = true;
                        }

                        IsSelecting = false;

                    }, (obg) => !IsSelecting);
                }
                return _selectSpeechRecognitionModelPathCommand;
            }
        }

        private ICommand? _saveDataToConfigurationFileCommand;
        public ICommand? SaveDataToConfigurationFileCommand
        {
            get
            {
                if(_saveDataToConfigurationFileCommand == null)
                {
                    _saveDataToConfigurationFileCommand = new RelayCommand((obj) =>
                    {
                        ConfigurationService.Set("SpeechModelPath", SpeechRecognitionModelPath);
                        ConfigurationService.Set("IsApproximateModeOn", AppState.IsApproximateModeOn);

                        UnsavedChangesExist = false;
                    });
                }
                return _saveDataToConfigurationFileCommand;
            }
        }

        private ICommand? _restoreDataFromConfigCommand;
        public ICommand? RestoreDataFromConfigCommand
        {
            get
            {
                if (_restoreDataFromConfigCommand == null)
                {
                    _restoreDataFromConfigCommand = new RelayCommand((obj) =>
                    {
                        SpeechRecognitionModelPath = ConfigurationService.Get<string>("SpeechModelPath");
                        AppState.IsApproximateModeOn = ConfigurationService.Get<bool>("IsApproximateModeOn");

                        UnsavedChangesExist = false;
                    });
                }
                return _restoreDataFromConfigCommand;
            }
        }

        #endregion
    }
}
