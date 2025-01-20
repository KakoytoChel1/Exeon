using Exeon.Services;
using Exeon.Services.IServices;
using Exeon.ViewModels;
using Exeon.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using System.Linq;
using System;
using Microsoft.UI;

namespace Exeon
{
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            var coreTitleBar = AppWindow.TitleBar;
            coreTitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;

            // To avoid a bug when hover and foreground color of the titlebar's buttons don't change when the system theme changes
            UpdateTitleBarTheme();
            this.Activated += MainWindow_Activated;

            App.Services.GetRequiredService<DispatcherQueueProvider>().Initialize(DispatcherQueue.GetForCurrentThread());
            ViewModel = App.Services.GetRequiredService<MainViewModel>();

            var navigationService = App.Services.GetRequiredService<INavigationService>();
            navigationService.InitializeFrame(RootFrame);
            navigationService.ChangePage<ChatPage>();
            MainNavigationView.SelectedItem = MainNavigationView.MenuItems.OfType<NavigationViewItem>()
                .FirstOrDefault(i => i.Tag.ToString() == "Chat");

            MainNavigationView.SelectionChanged += MainNavigationView_SelectionChanged;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            UpdateTitleBarTheme();
        }

        private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                if (selectedItem.Tag is string pageTag)
                {
                    ViewModel.NavigatePageCommand.Execute(pageTag);
                }
            }
        }

        private void UpdateTitleBarTheme()
        {
            var theme = Application.Current.RequestedTheme;
            var coreTitleBar = AppWindow.TitleBar;

            if (theme == ApplicationTheme.Light)
            {
                coreTitleBar.ButtonForegroundColor = Colors.Black;
                coreTitleBar.ButtonBackgroundColor = Colors.Transparent;
                coreTitleBar.ButtonHoverForegroundColor = Colors.Black;
                coreTitleBar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(255, 241, 241, 241);
            }
            else // Dark Theme
            {
                coreTitleBar.ButtonForegroundColor = Colors.White;
                coreTitleBar.ButtonBackgroundColor = Colors.Transparent;
                coreTitleBar.ButtonHoverForegroundColor = Colors.White;
                coreTitleBar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(255, 60, 60, 60);
            }
        }
    }
}
