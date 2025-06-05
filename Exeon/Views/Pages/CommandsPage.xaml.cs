using Exeon.Models.Commands;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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

        private string ConvertCollectionToString(ObservableCollection<TriggerCommand> collection)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    resultBuilder.Append($"{item.CommandText}; ");
                }

                return resultBuilder.ToString();
            }

            return "Empty collection!";
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<AutoSuggestCommandItem>();
                var splitText = sender.Text.ToLower().Split(" ");

                foreach (var command in ViewModel.AppState.CustomCommands)
                {
                    foreach (var trigger in command.TriggerCommands)
                    {
                        var found = splitText.All(key =>
                        trigger.CommandText.ToLower().Contains(key));

                        if (found)
                        {
#pragma warning disable CS8601 // Possible null reference assignment.
                            suitableItems.Add(new AutoSuggestCommandItem
                            {
                                CustomCommand = command,
                                Title = ConvertCollectionToString(command.TriggerCommands)
                            });
#pragma warning restore CS8601 // Possible null reference assignment.
                        }
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

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn && btn.DataContext is CustomCommand customCommand)
            {
                ViewModel.DragCommandDownCommand.Execute(customCommand);
            }
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CustomCommand customCommand)
            {
                ViewModel.DragCommandUpCommand.Execute(customCommand);
            }
        }
    }
}
