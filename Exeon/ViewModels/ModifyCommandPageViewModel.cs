using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;
using Exeon.Views.Dialog_pages;
using System.Collections.ObjectModel;
using System.Linq;
using Exeon.Models.Actions;
using System;
using Microsoft.EntityFrameworkCore;
using Action = Exeon.Models.Actions.Action;
using System.Threading.Tasks;
using Exeon.Services;
using System.Text.RegularExpressions;

namespace Exeon.ViewModels
{
    public class ModifyCommandPageViewModel : ViewModelBase
    {
        public ModifyCommandPageViewModel(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService,
            IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService)
            : base(appState, dispatcherQueueProvider, navigationService, configurationService, speechRecognitionService)
        {
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
        public ICommand CancelModifyingCommand
        {
            get
            {
                if (_cancelModifyingCommand == null)
                {
                    _cancelModifyingCommand = new RelayCommand((obj) =>
                    {
                        if (AppState.OriginalCommandState != null && AppState.SelectedModifyingCustomCommand != null)
                        {
                            // Восстановление оригинального состояния команды
                            AppState.SelectedModifyingCustomCommand.Id = AppState.OriginalCommandState.Id;
                            AppState.SelectedModifyingCustomCommand.Command = AppState.OriginalCommandState.Command;
                            AppState.SelectedModifyingCustomCommand.Actions = new ObservableCollection<Action>(
                                AppState.OriginalCommandState.Actions.Select(AppState.CloneAction));
                        }

                        AppState.IsSidePanelButtonsEnabled = true;
                        NavigationService.GoBack();
                    });
                }
                return _cancelModifyingCommand;
            }
        }

        private ICommand? _saveChanges;
        public ICommand SaveChanges
        {
            get
            {
                if (_saveChanges == null)
                {
                    _saveChanges = new RelayCommand(async (obj) =>
                    { 
                        var correctCommandText = Regex.Replace(AppState.SelectedModifyingCustomCommand!.Command.Trim(), @"\s{2,}", " ");

                        if(await AppState.CanModifyCustomCommand(AppState.SelectedModifyingCustomCommand!.Id, correctCommandText.ToLower()))
                        {
                            AppState.SelectedModifyingCustomCommand!.Command = correctCommandText;
                            AppState.ApplicationContext.SaveChanges();

                            AppState.IsSidePanelButtonsEnabled = true;
                            NavigationService.GoBack();
                        }
                        else
                        {
                            var xamlRoot = App.MainWindow.Content.XamlRoot;
                            await DialogManager.ShowContentDialog(xamlRoot, "Операцію скасовано", "ОК", ContentDialogButton.Primary,
                                $"Неможливо зберегти нову назву '{correctCommandText}' команди, оскільки вона вже зайнята.");
                        }
                    });
                }
                return _saveChanges;
            }
        }

        private ICommand? _addNewFileActionCommand;
        public ICommand AddNewFileActionCommand
        {
            get
            {
                if (_addNewFileActionCommand == null)
                {
                    _addNewFileActionCommand = new RelayCommand(async (obj) =>
                    {
                        var xamlRoot = App.MainWindow.Content.XamlRoot;

                        var result = await DialogManager.ShowContentDialog(xamlRoot, "Відкриття файлу", "Додати",
                            ContentDialogButton.Primary, new AddNewFileActionPage(), closeBtnText: "Скасувати");

                        if (result == ContentDialogResult.Primary)
                        {
                            if (!string.IsNullOrWhiteSpace(NewFileActionPath) && AppState.SelectedModifyingCustomCommand != null)
                            {
                                var newAction = new FileAction(NewFileActionPath)
                                {
                                    RootCommandId = AppState.SelectedModifyingCustomCommand.Id,
                                    OrderIndex = AppState.SelectedModifyingCustomCommand.Actions.Count + 1
                                };

                                AppState.SelectedModifyingCustomCommand.Actions.Add(newAction);

                                NewFileActionPath = string.Empty;
                            }
                        }
                    });
                }
                return _addNewFileActionCommand;
            }
        }

        private ICommand? _addNewWebActionCommand;
        public ICommand? AddNewWebActionCommand
        {
            get
            {
                if( _addNewWebActionCommand == null)
                {
                    _addNewWebActionCommand = new RelayCommand(async (obj) =>
                    {
                        var xamlRoot = App.MainWindow.Content.XamlRoot;

                        var result = await DialogManager.ShowContentDialog(xamlRoot, "Відкриття веб-сторінки", "Додати",
                            ContentDialogButton.Primary, new AddNewWebActionPage(), closeBtnText: "Скасувати");

                        if (result == ContentDialogResult.Primary)
                        {
                            if(!string.IsNullOrWhiteSpace(NewWebActionUri) && AppState.SelectedModifyingCustomCommand != null)
                            {
                                var newAction = new WebAction(NewWebActionUri)
                                {
                                    RootCommandId = AppState.SelectedModifyingCustomCommand.Id,
                                    OrderIndex = AppState.SelectedModifyingCustomCommand.Actions.Count + 1
                                };

                                AppState.SelectedModifyingCustomCommand.Actions.Add(newAction);

                                NewWebActionUri = string.Empty;
                            }
                        }
                    });
                }
                return _addNewWebActionCommand;
            }
        }

        private ICommand? _addNewPauseActionCommand;
        public ICommand? AddNewPauseActionCommand
        {
            get
            {
                if(_addNewPauseActionCommand == null)
                {
                    _addNewPauseActionCommand = new RelayCommand(async (obj) =>
                    {
                        var xamlRoot = App.MainWindow.Content.XamlRoot;

                        var result = await DialogManager.ShowContentDialog(xamlRoot, "Додавання паузи", "Додати",
                            ContentDialogButton.Primary, new AddNewPauseActionPage(), closeBtnText: "Скасувати");

                        if (result == ContentDialogResult.Primary)
                        {
                            if(NewPauseActionDelay > 0 && AppState.SelectedModifyingCustomCommand != null)
                            {
                                var newAction = new PauseAction(Convert.ToInt64(NewPauseActionDelay))
                                {
                                    RootCommandId = AppState.SelectedModifyingCustomCommand.Id,
                                    OrderIndex = AppState.SelectedModifyingCustomCommand.Actions.Count + 1
                                };

                                AppState.SelectedModifyingCustomCommand.Actions.Add(newAction);

                                NewPauseActionDelay = 0;
                            }
                        }
                    });
                }
                return _addNewPauseActionCommand;
            }
        }

        private ICommand? _addNewBrightnessActionCommand;
        public ICommand? AddNewBrightnessActionCommand
        {
            get
            {
                if(_addNewBrightnessActionCommand == null)
                {
                    _addNewBrightnessActionCommand = new RelayCommand(async (obj) =>
                    {
                        var xamlRoot = App.MainWindow.Content.XamlRoot;

                        var result = await DialogManager.ShowContentDialog(xamlRoot, "Зміна рівня яскравості", "Додати",
                            ContentDialogButton.Primary, new AddNewBrightnessActionPage(), closeBtnText: "Скасувати");

                        if (result == ContentDialogResult.Primary)
                        {
                            if((NewBrightnessActionLevel >= 0 && NewBrightnessActionLevel <= 100) && AppState.SelectedModifyingCustomCommand != null)
                            {
                                var newAction = new SystemBrightnessAction(NewBrightnessActionLevel)
                                {
                                    RootCommandId = AppState.SelectedModifyingCustomCommand.Id,
                                    OrderIndex = AppState.SelectedModifyingCustomCommand.Actions.Count + 1
                                };

                                AppState.SelectedModifyingCustomCommand.Actions.Add(newAction);

                                NewBrightnessActionLevel = 0;
                            }
                        }
                    });
                }
                return _addNewBrightnessActionCommand;
            }
        }

        private ICommand? _addNewSoundActionCommand;
        public ICommand? AddNewSoundActionCommand
        {
            get
            {
                if(_addNewSoundActionCommand == null)
                {
                    _addNewSoundActionCommand = new RelayCommand(async (obj) =>
                    {
                        var xamlRoot = App.MainWindow.Content.XamlRoot;

                        var result = await DialogManager.ShowContentDialog(xamlRoot, "Зміна рівня гучності", "Додати",
                            ContentDialogButton.Primary, new AddNewSoundActionPage(), closeBtnText: "Скасувати");

                        if (result == ContentDialogResult.Primary)
                        {
                            if((NewSoundActionLevel >= 0 && NewSoundActionLevel <= 100) && AppState.SelectedModifyingCustomCommand != null)
                            {
                                var newAction = new SystemSoundAction(NewSoundActionLevel)
                                {
                                    RootCommandId = AppState.SelectedModifyingCustomCommand.Id,
                                    OrderIndex = AppState.SelectedModifyingCustomCommand.Actions.Count + 1
                                };

                                AppState.SelectedModifyingCustomCommand.Actions.Add(newAction);

                                NewSoundActionLevel = 0;
                            }
                        }
                    });
                }
                return _addNewSoundActionCommand;
            }
        }

        private ICommand? _deleteActionCommand;
        public ICommand DeleteActionCommand
        {
            get
            {
                if (_deleteActionCommand == null)
                {
                    _deleteActionCommand = new RelayCommand((obj) =>
                    {
                        if (obj is Action action && AppState.SelectedModifyingCustomCommand != null)
                        {
                            AppState.SelectedModifyingCustomCommand.Actions.Remove(action);

                            // Проверяем, отслеживается ли действие в контексте
                            if (AppState.ApplicationContext.Entry(action).State != EntityState.Detached)
                            {
                                AppState.ApplicationContext.Actions.Remove(action);
                            }
                        }
                    });
                }
                return _deleteActionCommand;
            }
        }

        private ICommand? _changeOrderInActionCollection;
        public ICommand ChangeOrderInActionCollection
        {
            get
            {
                if(_changeOrderInActionCollection == null)
                {
                    _changeOrderInActionCollection = new RelayCommand(async (obj) =>
                    {
                        await Task.Run(() =>
                        {
                            var selectedCommand = AppState.SelectedModifyingCustomCommand;

                            if (selectedCommand != null)
                            {
                                int index = 0;
                                foreach (var action in selectedCommand.Actions)
                                {
                                    action.OrderIndex = index++;
                                }

                                var existingActions = selectedCommand.Actions.Where(a => a.Id != 0).ToList();

                                AppState.ApplicationContext.UpdateRange(existingActions);
                            }
                        });
                    });
                }
                return _changeOrderInActionCollection;
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
