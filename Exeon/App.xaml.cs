using Exeon.Helpers;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using Size = System.Drawing.Size;
using Point = System.Drawing.Point;
using Exeon.Services.IServices;
using Exeon.Services;
using Exeon.Views.Pages;

namespace Exeon
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        private static WindowHelper? _windowHelper;

        public static MainWindow MainWindow { get; private set; }

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<AppState>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<CommandsPageViewModel>();
            serviceCollection.AddSingleton<ModifyCommandPageViewModel>();
            serviceCollection.AddSingleton<ChatPageViewModel>();
            serviceCollection.AddSingleton<SettingsPageViewModel>();

            // Intialization of the RootFrame and setting first selected page is in main window's code behind (MainWindow.xaml.cs)
            serviceCollection.AddSingleton<INavigationService, NavigationService>();
            serviceCollection.AddSingleton<IConfigurationService, ConfigurationService>();

            Services = serviceCollection.BuildServiceProvider();

            MainWindow = new MainWindow();

            // Setting min window bounds
            _windowHelper = new WindowHelper(MainWindow);
            _windowHelper.SetMinMaxBounds(new Size(new Point(x: 418, y: 720)));

            MainWindow.Activate();
        }
    }
}
