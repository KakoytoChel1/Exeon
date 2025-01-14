using Exeon.Services;
using Exeon.Services.IServices;
using Exeon.ViewModels;
using Exeon.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using System.Linq;

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

            App.Services.GetRequiredService<DispatcherQueueProvider>().Initialize(DispatcherQueue.GetForCurrentThread());
            ViewModel = App.Services.GetRequiredService<MainViewModel>();

            var navigationService = App.Services.GetRequiredService<INavigationService>();
            navigationService.InitializeFrame(RootFrame);
            navigationService.ChangePage<ChatPage>();
            MainNavigationView.SelectedItem = MainNavigationView.MenuItems.OfType<NavigationViewItem>()
                .FirstOrDefault(i => i.Tag.ToString() == "Chat");

            MainNavigationView.SelectionChanged += MainNavigationView_SelectionChanged;
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
    }
}
