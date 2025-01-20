using Exeon.Models.Actions;
using Exeon.Models.Commands;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace Exeon.Views.Pages
{
    public sealed partial class CommandsPage : Page
    {
        private const string _noResultsTitle = "Немає результату";

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

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<AutoSuggestCommandItem>();
                var splitText = sender.Text.ToLower().Split(" ");

                foreach (var command in ViewModel.AppState.CustomCommands)
                {
                    var found = splitText.All(key =>
                        command.Command.ToLower().Contains(key));

                    if (found)
                    {
                        suitableItems.Add(new AutoSuggestCommandItem
                        {
                            CustomCommand = command,
                            Title = command.Command
                        });
                    }
                }

                if (suitableItems.Count == 0)
                {
                    suitableItems.Add(new AutoSuggestCommandItem { Title = _noResultsTitle });
                }

                sender.ItemsSource = suitableItems;
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is AutoSuggestCommandItem selectedItem)
            {
                if (selectedItem.Title == _noResultsTitle && selectedItem.CustomCommand == null)
                {
                    sender.Text = string.Empty;
                    return;
                }

                CustomCommandsList.ScrollIntoView(selectedItem.CustomCommand);
            }
        }

    }
}
