using Exeon.Models.Commands;
using Exeon.Services;
using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;
using Microsoft.UI.Xaml;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;
using Exeon.Views.Dialog_pages;
using System;
using Exeon.Models.Actions;

namespace Exeon.ViewModels
{
    public class ModifyCommandPageViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        public AppState AppState { get; }

        public ModifyCommandPageViewModel(INavigationService navigationService, AppState appState)
        {
            _navigationService = navigationService;
            AppState = appState;
        }

        #region Properties

        private string? _newFileActionPath;
        public string? NewFileActionPath
        {
            get => _newFileActionPath;
            set { _newFileActionPath = value; OnPropertyChanged(); }
        }

        private string? _newWebActionUri;
        public string? NewWebActionUri
        {
            get => _newWebActionUri;
            set { _newWebActionUri = value; OnPropertyChanged(); }
        }

        private double _newPauseActionDelay;
        public double NewPauseActionDelay
        {
            get => _newPauseActionDelay;
            set { _newPauseActionDelay = value; OnPropertyChanged(); }
        }

        private int _newBrightnessActionLevel;
        public int NewBrightnessActionLevel
        {
            get => _newBrightnessActionLevel;
            set { _newBrightnessActionLevel = value; OnPropertyChanged(); }
        }

        private int _newSoundActionLevel;
        public int NewSoundActionLevel
        {
            get => _newSoundActionLevel;
            set { _newSoundActionLevel = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands

        private ICommand? _cancelModifyingCommand;
        public ICommand? CancelModifyingCommand
        {
            get
            {
                _cancelModifyingCommand = new RelayCommand((obj) =>
                {
                    AppState.SelectedModifyingCustomCommand = null;
                    AppState.IsSidePanelButtonsEnabled = true;

                    _navigationService.GoBack();
                });
                return _cancelModifyingCommand;
            }
        }

        
        private ICommand? _addNewWebActionCommand;
        private ICommand? _addNewPauseActionCommand;
        private ICommand? _addNewBrightnessActionCommand;
        private ICommand? _addNewSoundActionCommand;

        private ICommand? _addNewFileActionCommand;
        public ICommand AddNewFileActionCommand
        {
            get
            {
                _addNewFileActionCommand = new RelayCommand(async (obj) =>
                {
                    var xamlRoot = App.MainWindow.Content.XamlRoot;

                    var result = await DialogManager.ShowContentDialog(xamlRoot, "Відкриття файлу", "Додати",
                            ContentDialogButton.Primary, new AddNewFileActionPage(), closeBtnText: "Скасувати");
                    AddNewAction(typeof(FileAction), result);
                });
                return _addNewFileActionCommand;
            }
        }

        public ICommand? AddNewWebActionCommand
        {
            get
            {
                _addNewWebActionCommand = new RelayCommand(async (obj) =>
                {
                    var xamlRoot = App.MainWindow.Content.XamlRoot;

                    var result = await DialogManager.ShowContentDialog(xamlRoot, "Відкриття веб-сторінки", "Додати",
                        ContentDialogButton.Primary, new AddNewWebActionPage(), closeBtnText: "Скасувати");
                    AddNewAction(typeof(WebAction), result);
                });
                return _addNewWebActionCommand;
            }
        }

        public ICommand? AddNewPauseActionCommand
        {
            get
            {
                _addNewPauseActionCommand = new RelayCommand(async (obj) =>
                {
                    var xamlRoot = App.MainWindow.Content.XamlRoot;

                    var result = await DialogManager.ShowContentDialog(xamlRoot, "Додавання паузи", "Додати",
                        ContentDialogButton.Primary, new AddNewPauseActionPage(), closeBtnText: "Скасувати");
                    AddNewAction(typeof(PauseAction), result);
                });
                return _addNewPauseActionCommand;
            }
        }

        public ICommand? AddNewBrightnessActionCommand
        {
            get
            {
                _addNewBrightnessActionCommand = new RelayCommand(async (obj) =>
                {
                    var xamlRoot = App.MainWindow.Content.XamlRoot;

                    var result = await DialogManager.ShowContentDialog(xamlRoot, "Зміна рівня яскравості", "Додати",
                        ContentDialogButton.Primary, new AddNewBrightnessActionPage(), closeBtnText: "Скасувати");
                    AddNewAction(typeof(SystemBrightnessAction), result);
                });
                return _addNewBrightnessActionCommand;
            }
        }

        public ICommand? AddNewSoundActionCommand
        {
            get
            {
                _addNewSoundActionCommand = new RelayCommand(async (obj) =>
                {
                    var xamlRoot = App.MainWindow.Content.XamlRoot;

                    var result = await DialogManager.ShowContentDialog(xamlRoot, "Зміна рівня гучності", "Додати",
                        ContentDialogButton.Primary, new AddNewSoundActionPage(), closeBtnText: "Скасувати");
                    AddNewAction(typeof(SystemSoundAction), result);
                });
                return _addNewSoundActionCommand;
            }
        }

        private ICommand? _deleteActionCommand;
        public ICommand? DeleteActionCommand
        {
            get
            {
                _deleteActionCommand = new RelayCommand((obj) =>
                {
                    if(obj is Models.Actions.Action action)
                    {
                        var xamlRoot = App.MainWindow.Content.XamlRoot;

                        AppState.SelectedModifyingCustomCommand!.Actions.Remove(action);
                    }
                });
                return _deleteActionCommand;
            }
        }
        #endregion

        #region Methods

        private void AddNewAction(Type type, ContentDialogResult result)
        {
            if(result == ContentDialogResult.Primary)
            {
                if (type == typeof(FileAction) && !string.IsNullOrWhiteSpace(NewFileActionPath))
                {
                    AppState.SelectedModifyingCustomCommand!.Actions.Add(
                        new FileAction(NewFileActionPath));
                }
                else if (type == typeof(WebAction) && !string.IsNullOrWhiteSpace(NewWebActionUri))
                {
                    AppState.SelectedModifyingCustomCommand!.Actions.Add(
                        new WebAction(NewWebActionUri));
                }
                else if (type == typeof(PauseAction) && NewPauseActionDelay != default)
                {
                    AppState.SelectedModifyingCustomCommand!.Actions.Add(
                        new PauseAction(Convert.ToInt64(NewPauseActionDelay)));
                }
                else if (type == typeof(SystemBrightnessAction) && (NewBrightnessActionLevel >= 0 && NewBrightnessActionLevel <= 100))
                {
                    AppState.SelectedModifyingCustomCommand!.Actions.Add(
                        new SystemBrightnessAction(NewBrightnessActionLevel));
                }
                else if (type == typeof(SystemSoundAction) && (NewSoundActionLevel >= 0 && NewSoundActionLevel <= 100))
                {
                    AppState.SelectedModifyingCustomCommand!.Actions.Add(
                        new SystemSoundAction(NewSoundActionLevel));
                }
            }
        }
        #endregion
    }
}
