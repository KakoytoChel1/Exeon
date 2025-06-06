using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Linq;

namespace Exeon.Views.Pages
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<MainPageViewModel>();

            MainNavigationView.SelectionChanged += MainNavigationView_SelectionChanged;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            NavigateToProvidedPage("Chat");
            MainNavigationView.SelectedItem = MainNavigationView.MenuItems.OfType<NavigationViewItem>()
                        .FirstOrDefault(i => i.Tag.ToString() == "Chat");
        }

        private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                if (selectedItem.Tag is string pageTag)
                {
                    NavigateToProvidedPage(pageTag);
                }
            }
        }

        private void NavigateToProvidedPage(string pageTag)
        {
            switch (pageTag)
            {
                case "Chat":
                    RootFrame.Navigate(typeof(ChatPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    break;
                case "Commands":
                    RootFrame.Navigate(typeof(CommandsPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    break;
                case "Settings":
                    RootFrame.Navigate(typeof(SettingsPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    break;
                case "Guide":
                    RootFrame.Navigate(typeof(GuidePage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    break;
            }
        }
    }
}
