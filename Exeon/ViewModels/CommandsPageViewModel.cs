using Exeon.Models.Actions;
using Exeon.Models.Commands;
using Exeon.Services;
using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;
using Exeon.Views.Dialog_pages;
using Exeon.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Exeon.ViewModels
{
    public class CommandsPageViewModel : ViewModelBase
    {
        // Хранит в себе коллекцию тригер команд в момент создания кастомной команды
        public ObservableCollection<TriggerCommand>? TemporaryTriggerCommandsCollection { get; private set; } = null;

        public CommandsPageViewModel(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService,
            IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService)
            : base(appState, dispatcherQueueProvider, navigationService, configurationService, speechRecognitionService)
        {
        }

        #region Properties

        #endregion

        #region Commands

        private ICommand? _addNewCustomCommand;
        public ICommand AddNewCustomCommand
        {
            get
            {
                if( _addNewCustomCommand == null)
                {
                    _addNewCustomCommand = new RelayCommand(async (obj) =>
                    {
                        // Getting xamlroot for using ContentDialog
                        var xamlRoot = App.MainWindow.Content.XamlRoot;

                        TemporaryTriggerCommandsCollection = new ObservableCollection<TriggerCommand>();

                        // Showing content dialog
                        var result = await DialogManager.ShowContentDialog(xamlRoot, "Створення команди",
                            "Створити", ContentDialogButton.Primary, new AddNewCustomCommandPage(), closeBtnText: "Скасувати");

                        // If was pressed primary button - adding new custom command
                        if (result == ContentDialogResult.Primary)
                        {
                            var newCommand = new CustomCommand();
                            
                            if(TemporaryTriggerCommandsCollection != null && TemporaryTriggerCommandsCollection.Count > 0)
                            {
                                newCommand.TriggerCommands = TemporaryTriggerCommandsCollection!;
                            }
                                
                            AppState.CustomCommands.Add(newCommand);
                            newCommand.OrderIndex = AppState.CustomCommands.Count - 1;

                            AppState.ApplicationContext.Add(newCommand);
                            AppState.ApplicationContext.SaveChanges();
                        }
                    });
                }
                return _addNewCustomCommand;
            }
        }

        private ICommand? _addNewTriggerCommand;
        public ICommand AddNewTriggerCommand
        {
            get
            {
                if (_addNewTriggerCommand == null)
                {
                    _addNewTriggerCommand = new RelayCommand((obj) =>
                    {
                        string? triggerCommandText = obj as string;

                        // Trimming and replacing 2 and more spaces with 1
                        string trimmedTriggerCommandText = Regex.Replace(triggerCommandText!.Trim(), @"\s{2,}", " ");
                        TemporaryTriggerCommandsCollection!.Add(new TriggerCommand() { CommandText = trimmedTriggerCommandText });
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
                        TriggerCommand? triggerCommand = obj as TriggerCommand;

                        if (triggerCommand != null)
                        {
                            TemporaryTriggerCommandsCollection?.Remove(triggerCommand);
                        }
                    });
                }
                return _removeTriggerCommand;
            }
        }

        private ICommand? _modifyCustomCommand;
        public ICommand ModifyCustomCommand
        {
            get
            {
                if(_modifyCustomCommand == null)
                {
                    _modifyCustomCommand = new RelayCommand((obj) =>
                    {
                        if (obj is CustomCommand command)
                        {
                            if (command != null)
                            {
                                StartEditingCommand(command);

                                AppState.IsSidePanelButtonsEnabled = false;

                                NavigationService.ChangePage<ModifyCommandPage>();
                            }
                        }
                    });
                }
                return _modifyCustomCommand;
            }
        }

        private ICommand? _deleteCustomCommand;
        public ICommand DeleteCustomCommand
        {
            get
            {
                if(_deleteCustomCommand == null)
                {
                    _deleteCustomCommand = new RelayCommand(async (obj) =>
                    {
                        if (obj is CustomCommand command)
                        {
                            if (command != null)
                            {
                                var xamlRoot = App.MainWindow.Content.XamlRoot;

                                var result = await DialogManager.ShowContentDialog(xamlRoot, "Видалення команди",
                                "Видалити", ContentDialogButton.Primary, "Ви впевнені що бажаєте видалити обрану команду?", closeBtnText: "Скасувати");

                                if (result == ContentDialogResult.Primary)
                                {
                                    AppState.CustomCommands.Remove(command);

                                    AppState.ApplicationContext.Remove(command);
                                    AppState.ApplicationContext.SaveChanges();
                                }
                            }
                        }
                    });
                }
                return _deleteCustomCommand;
            }
        }

        private ICommand? _dragCommandUpCommand;
        public ICommand DragCommandUpCommand
        {
            get
            {
                if(_dragCommandUpCommand == null)
                {
                    _dragCommandUpCommand = new RelayCommand((obj) =>
                    {
                        CustomCommand? command = obj as CustomCommand;

                        if(command != null)
                        {
                            var actedCommandOrderIndex = command.OrderIndex;

                            // Проверка, дабы не выйти за границы
                            if(actedCommandOrderIndex < AppState.CustomCommands.Count - 1)
                            {
                                var nextCommand = AppState.CustomCommands.FirstOrDefault
                                (c => c.OrderIndex == actedCommandOrderIndex + 1);

                                AppState.CustomCommands.Move(actedCommandOrderIndex, nextCommand!.OrderIndex);

                                command.OrderIndex = nextCommand!.OrderIndex;
                                nextCommand.OrderIndex = actedCommandOrderIndex;

                                AppState.ApplicationContext.CustomCommands.Update(command);
                                AppState.ApplicationContext.CustomCommands.Update(nextCommand);
                                AppState.ApplicationContext.SaveChanges();
                            }
                        }
                    });
                }
                return _dragCommandUpCommand;
            }
        }

        private ICommand? _dragCommandDownCommand;
        public ICommand DragCommandDownCommand
        {
            get
            {
                if (_dragCommandDownCommand == null)
                {
                    _dragCommandDownCommand = new RelayCommand((obj) =>
                    {
                        CustomCommand? command = obj as CustomCommand;

                        if (command != null)
                        {
                            var actedCommandOrderIndex = command.OrderIndex;

                            // Проверка, дабы не выйти за границы
                            if (actedCommandOrderIndex > 0)
                            {
                                var previousCommand = AppState.CustomCommands.FirstOrDefault
                                (c => c.OrderIndex == actedCommandOrderIndex - 1);

                                AppState.CustomCommands.Move(actedCommandOrderIndex, previousCommand!.OrderIndex);

                                command.OrderIndex = previousCommand!.OrderIndex;
                                previousCommand.OrderIndex = actedCommandOrderIndex;

                                AppState.ApplicationContext.CustomCommands.Update(command);
                                AppState.ApplicationContext.CustomCommands.Update(previousCommand);
                                AppState.ApplicationContext.SaveChanges();
                            }
                        }
                    });
                }
                return _dragCommandDownCommand;
            }
        }

        #endregion

        private void StartEditingCommand(CustomCommand commandToEdit)
        {
            if (commandToEdit != null)
            {
                // Глубокое копирование команды и ее действий
                AppState.OriginalCommandState = new CustomCommand
                {
                    Id = commandToEdit.Id,
                    Actions = new ObservableCollection<Action>(
                        commandToEdit.Actions.Select(AppState.CloneAction)
                    ),
                    TriggerCommands = new ObservableCollection<TriggerCommand>(commandToEdit.TriggerCommands)
                };

                AppState.SelectedModifyingCustomCommand = commandToEdit;
            }
        }

        // Проверка на, существует ли переданная триггер строка в БД
        public bool IsAlreadyExistAdding(string triggerCommandText)
        {
            bool existInDB = AppState.ApplicationContext.TriggerCommands.Any(
                tc => tc.CommandText.ToLower() == triggerCommandText.ToLower());

            bool existInTemp = TemporaryTriggerCommandsCollection!.Any(
                tc => tc.CommandText.ToLower() == triggerCommandText.ToLower());

            return existInDB || existInTemp;
        }
    }
}
