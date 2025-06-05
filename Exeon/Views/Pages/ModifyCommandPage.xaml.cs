using System;
using Exeon.Models.Actions;
using Exeon.Models.Commands;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace Exeon.Views.Pages
{
    public sealed partial class ModifyCommandPage : Page
    {
        public ModifyCommandPageViewModel ViewModel { get; private set; }

        public ModifyCommandPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ModifyCommandPageViewModel>();
        }

        private void DeleteFileAction_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            ViewModel.DeleteActionCommand!.Execute(button!.DataContext as Models.Actions.Action);
        }

        private void DeleteWebAction_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            ViewModel.DeleteActionCommand!.Execute(button!.DataContext as WebAction);
        }

        private void DeletePauseAction_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            ViewModel.DeleteActionCommand!.Execute(button!.DataContext as PauseAction);
        }

        private void DeleteBrightnessAction_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            ViewModel.DeleteActionCommand!.Execute(button!.DataContext as SystemBrightnessAction);
        }

        private void DeleteSoundActionButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            ViewModel.DeleteActionCommand!.Execute(button!.DataContext as SystemSoundAction);
        }

        private void CopyFileActionPath_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            FileAction? fileAction = button!.DataContext as FileAction;

            if(fileAction != null)
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(fileAction.PathToFile);
                Clipboard.SetContent(dataPackage);
            }
        }

        private void CopyWebActionUrl_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            WebAction? webAction = button!.DataContext as WebAction;

            if (webAction != null)
            {
                var dataPackage = new DataPackage();
                dataPackage.SetWebLink(new Uri(webAction.Uri));
                Clipboard.SetContent(dataPackage);
            }
        }

        private void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            ViewModel.ChangeOrderInActionCollection.Execute(null);
        }

        private void AddNewTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            var text = TriggerTextBox.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                ErrorInfoBar.Message = "Ви залишили поле пустим, заповніть його!";
                ErrorInfoBar.IsOpen = true;
                return;
            }

            if (ViewModel.IsAlreadyExist(text))
            {
                ErrorInfoBar.Message = "Такий тригер вже існує, вигадайте новий!";
                ErrorInfoBar.IsOpen = true;
                return;
            }

            ViewModel.AddNewTriggerCommand.Execute(text);
            TriggerTextBox.Text = string.Empty;
            ErrorInfoBar.IsOpen = false;
        }

        private void CopyTriggerTextBtn_Click(object sender, RoutedEventArgs e)
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

        private void RemoveTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TriggerCommand triggerCommand)
            {
                ViewModel.RemoveTriggerCommand.Execute(triggerCommand);
            }
        }
    }
}
