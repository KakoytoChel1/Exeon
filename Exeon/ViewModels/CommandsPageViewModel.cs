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
using System.Threading.Tasks;
using System.Windows.Input;

namespace Exeon.ViewModels
{
    public class CommandsPageViewModel : ViewModelBase
    {
        public CommandsPageViewModel(AppState appState, DispatcherQueueProvider dispatcherQueueProvider, INavigationService navigationService,
            IConfigurationService configurationService, ISpeechRecognitionService speechRecognitionService)
            : base(appState, dispatcherQueueProvider, navigationService, configurationService, speechRecognitionService)
        {
        }

        #region Properties

        private string? _newCustomCommandCommandText;
        public string? NewCustomCommandCommandText
        {
            get { return _newCustomCommandCommandText; }
            set { _newCustomCommandCommandText = value; OnPropertyChanged(); }
        }
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

                        // Showing content dialog
                        var result = await DialogManager.ShowContentDialog(xamlRoot, "Додавання команди",
                            "Додати", ContentDialogButton.Primary, new AddNewCustomCommandPage(), closeBtnText: "Скасувати");

                        // If was pressed primary button - adding new custom command
                        if (result == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(NewCustomCommandCommandText))
                        {
                            // Trimming and replacing 2 and more spaces with 1
                            string newCustomCommandtext = Regex.Replace(NewCustomCommandCommandText.Trim(), @"\s{2,}", " ");

                            if (await AppState.CanAddNewCustomCommand(newCustomCommandtext.ToLower()))
                            {
                                var newCommand = new CustomCommand();
                                AppState.CustomCommands.Add(newCommand);

                                AppState.ApplicationContext.Add(newCommand);
                                AppState.ApplicationContext.SaveChanges();

                                NewCustomCommandCommandText = string.Empty;
                            }
                            else
                            {
                                await DialogManager.ShowContentDialog(xamlRoot, "Операцію скасовано", "ОК", ContentDialogButton.Primary,
                                    $"Неможливо створити нову команду з назвою '{newCustomCommandtext}', оскільки вона вже зайнята.");

                                NewCustomCommandCommandText = string.Empty;
                            }
                        }
                    });
                }
                return _addNewCustomCommand;
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
                    )
                };

                AppState.SelectedModifyingCustomCommand = commandToEdit;
            }
        }
    }
}
