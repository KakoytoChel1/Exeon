using Exeon.Models;
using Exeon.Models.Commands;
using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;
using Exeon.Views.Dialog_pages;
using Exeon.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Exeon.ViewModels
{
    public class CommandsPageViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        public AppState AppState { get; }

        public CommandsPageViewModel(INavigationService navigationService, AppState appState)
        {
            _navigationService = navigationService;
            AppState = appState;
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
                _addNewCustomCommand = new RelayCommand(async (obj) =>
                {
                    // Getting xamlroot for using ContentDialog
                    var xamlRoot = App.MainWindow.Content.XamlRoot;

                    // Showing content dialog
                    var result = await DialogManager.ShowContentDialog(xamlRoot, "Додавання команди",
                        "Додати", ContentDialogButton.Primary, new AddNewCustomCommandPage(), closeBtnText: "Скасувати");

                    await Task.Delay(10);

                    // If was pressed primary button - adding new custom command
                    if (result == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(NewCustomCommandCommandText))
                    {
                        AppState.CustomCommands.Add(new CustomCommand(NewCustomCommandCommandText));
                        NewCustomCommandCommandText = string.Empty;
                    }
                });
                return _addNewCustomCommand;
            }
        }

        private ICommand? _modifyCustomCommand;
        public ICommand ModifyCustomCommand
        {
            get
            {
                _modifyCustomCommand = new RelayCommand((obj) =>
                {
                    if(obj is CustomCommand command)
                    {
                        if(command != null)
                        {
                            AppState.SelectedModifyingCustomCommand = command;

                            AppState.IsSidePanelButtonsEnabled = false;

                            _navigationService.ChangePage<ModifyCommandPage>();
                        }
                    }
                });
                return _modifyCustomCommand;
            }
        }

        private ICommand? _deleteCustomCommand;
        public ICommand DeleteCustomCommand
        {
            get
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

                            if(result == ContentDialogResult.Primary)
                            {
                                AppState.CustomCommands.Remove(command);
                            }
                        }
                    }
                });
                return _deleteCustomCommand;
            }
        }
        #endregion
    }
}
