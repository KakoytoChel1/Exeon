using System;
using Exeon.Models.Actions;
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

            modifyCommandPage.SizeChanged += ModifyCommandPage_SizeChanged;
        }

        private void ModifyCommandPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Getting width of the available space for TextBox
            var buttonWidth = AddDropDownButton.ActualWidth;
            var margin = PanelGrid.Margin.Left + PanelGrid.Margin.Right + AddDropDownButton.Margin.Left;

            // Расчёт доступной ширины
            var availableWidth = e.NewSize.Width - buttonWidth - margin;
            AdaptiveTextBox.Width = System.Math.Clamp(availableWidth, 170, 600);
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
    }
}
