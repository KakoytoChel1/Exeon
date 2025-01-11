using Exeon.Models.Actions;
using Exeon.Models.Commands;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace Exeon.Views.Pages
{
    public sealed partial class CommandsPage : Page
    {
        public CommandsPageViewModel ViewModel { get; private set; }

        public CommandsPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<CommandsPageViewModel>();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void CommandItemExpander_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var expander = sender as Expander;

            if (expander != null)
            {
                double newWidth = expander.ActualWidth;
                double maxWidth;

                if (newWidth <= 400)
                    maxWidth = 200;
                else
                    maxWidth = newWidth / 2;

                var border = expander.FindName("CommandTitlePanel") as Border;
                if (border != null)
                {
                    border.SetValue(Border.MaxWidthProperty, maxWidth);
                }
            }
        }

        // Calling commmands from ViewModel
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            ViewModel.DeleteCustomCommand.Execute(button!.DataContext as CustomCommand);
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            ViewModel.ModifyCustomCommand.Execute(button!.DataContext as CustomCommand);
        }
    }
}
