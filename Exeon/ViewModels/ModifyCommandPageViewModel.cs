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
using Exeon.Models.Commands;
using System.Text.RegularExpressions;
using Exeon.Views.Pages;

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
                            AppState.SelectedModifyingCustomCommand.TriggerCommands = new ObservableCollection<TriggerCommand>(
                                AppState.OriginalCommandState.TriggerCommands);
                            AppState.SelectedModifyingCustomCommand.Actions = new ObservableCollection<Action>(
                                AppState.OriginalCommandState.Actions.Select(AppState.CloneAction));
                        }

                        AppState.IsSidePanelButtonsEnabled = true;
                        NavigationService.ChangePage<MainPage>();
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
                    _saveChanges = new RelayCommand((obj) =>
                    {
                        AppState.ApplicationContext.SaveChanges();

                        AppState.IsSidePanelButtonsEnabled = true;
                        NavigationService.ChangePage<MainPage>();
                    });
                }
                return _saveChanges;
            }
        }

        private ICommand? _addNewTriggerCommand;
        public ICommand AddNewTriggerCommand
        {
            get
            {
                if(_addNewTriggerCommand == null)
                {
                    _addNewTriggerCommand = new RelayCommand((obj) =>
                    {
                        string? triggerCommandText = obj as string;

                        // Trimming and replacing 2 and more spaces with 1
                        string trimmedTriggerCommandText = Regex.Replace(triggerCommandText!.Trim(), @"\s{2,}", " ");
                        AppState.SelectedModifyingCustomCommand!.TriggerCommands.Add(new TriggerCommand() { CommandText = trimmedTriggerCommandText });
                    });
                }
                return _addNewTriggerCommand;
            }
        }

        private ICommand? _removeTriggerCommand;
        public ICommand RemoveTriggerCommand
        {
            get
            {
                if (_removeTriggerCommand == null)
                {
                    _removeTriggerCommand = new RelayCommand((obj) =>
                    {
                        if (obj is TriggerCommand triggerCommand && AppState.SelectedModifyingCustomCommand != null)
                        {
                            AppState.SelectedModifyingCustomCommand.TriggerCommands.Remove(triggerCommand);

                            // Проверяем, отслеживается ли действие в контексте
                            if (AppState.ApplicationContext.Entry(triggerCommand).State != EntityState.Detached)
                            {
                                AppState.ApplicationContext.TriggerCommands.Remove(triggerCommand);
                            }
                        }
                    });
                }
                return _removeTriggerCommand;
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
                                // Перезапись индексов порядка
                                int index = 0;
                                foreach (var action in selectedCommand.Actions)
                                {
                                    action.OrderIndex = index++;
                                }

                                // Отбираем и обновляем только уже сохранённые записи (Id != 0)
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

        // Проверка на, существует ли переданная триггер строка в БД
        public bool IsAlreadyExist(string triggerCommandText)
        {
            bool existInDB = AppState.ApplicationContext.TriggerCommands.Any(
                tc => tc.CommandText.ToLower() == triggerCommandText.ToLower());

            bool existInTemp = AppState.SelectedModifyingCustomCommand!.TriggerCommands.Any(
                tc => tc.CommandText.ToLower() == triggerCommandText.ToLower());

            return existInDB || existInTemp;
        }
        #endregion
    }
}
