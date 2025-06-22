using Exeon.Models.Commands;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace Exeon.Views.Dialog_pages
{
    public sealed partial class AddNewCustomCommandPage : Page
    {
        public CommandsPageViewModel ViewModel { get; private set; }

        public AddNewCustomCommandPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<CommandsPageViewModel>();
        }

        private void AddNewTriggerCommandBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var text = EnterTriggerCommandTextBox.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                ErrorInfoBar.Message = "Ви залишили поле пустим, заповніть його!";
                ErrorInfoBar.IsOpen = true;
                return;
            }

            if (ViewModel.IsAlreadyExistAdding(text))
            {
                ErrorInfoBar.Message = "Такий тригер вже існує, вигадайте новий!";
                ErrorInfoBar.IsOpen = true;
                return;
            }

            ViewModel.AddNewTriggerCommand.Execute(text);
            EnterTriggerCommandTextBox.Text = string.Empty;
            ErrorInfoBar.IsOpen = false;
        }

        private void DeleteTrigger_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(sender is Button btn && btn.DataContext is TriggerCommand triggerCommand)
            {
               ViewModel.RemoveTriggerCommand.Execute(triggerCommand);
            }
        }

        private void CopyTextBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TriggerCommand triggerCommand)
            {
                var text = triggerCommand.CommandText;

                DataPackage dataPackage = new();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                dataPackage.SetText(text);
                Clipboard.SetContent(dataPackage);
            }
        }
    }
}
