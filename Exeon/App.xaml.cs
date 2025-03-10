using Exeon.Helpers;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using Exeon.Services.IServices;
using Exeon.Services;
using Microsoft.UI.Xaml.Controls;
using Exeon.ViewModels.Tools;
using Size = System.Drawing.Size;
using Point = System.Drawing.Point;
using Exeon.Views.Pages;

namespace Exeon
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        private static WindowHelper? _windowHelper;

        public static MainWindow MainWindow { get; private set; } = null!;

        public App()
        {
            this.InitializeComponent();
            UnhandledException += App_UnhandledException;
        }

        private async void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var exeption = e.Exception;

            // Logger here
            // ...

            if(MainWindow != null)
            {
                await DialogManager.ShowContentDialog(MainWindow.Content.XamlRoot, 
                    "UnhandledException", "Okay", ContentDialogButton.Primary, $"{exeption.Message}");
            }  
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<AppState>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<MainPageViewModel>();
            serviceCollection.AddSingleton<CommandsPageViewModel>();
            serviceCollection.AddSingleton<ModifyCommandPageViewModel>();
            serviceCollection.AddSingleton<ChatPageViewModel>();
            //serviceCollection.AddSingleton<SettingsPageViewModel>();

            // Intialization of the RootFrame and setting first selected page is in main window's code behind (MainWindow.xaml.cs)
            serviceCollection.AddSingleton<INavigationService, NavigationService>();
            serviceCollection.AddSingleton<IConfigurationService, ConfigurationService>();
            serviceCollection.AddSingleton<DispatcherQueueProvider>(); // Initializing in MainWindow's code behind (MainWindow.xaml.cs)
            serviceCollection.AddSingleton<ISpeechRecognitionService, SpeechRecognitionService>();

            Services = serviceCollection.BuildServiceProvider();

            MainWindow = new MainWindow();

            // Setting min window bounds
            _windowHelper = new WindowHelper(MainWindow);
            _windowHelper.SetMinMaxBounds(new Size(new Point(x: 418, y: 720)));

            MainWindow.Activate();

            // Setting up the first visible page
            var navigationService = Services.GetRequiredService<INavigationService>();
            navigationService.InitializeFrame(MainWindow.RootFrameProperty);

            // Setting up the loading page until the speech recognition model is initilized
            navigationService.ChangePage<LoadingPage>();

            // Initializing the speech model
            var speechRecognitionService = Services.GetRequiredService<ISpeechRecognitionService>();
            await speechRecognitionService.InitializeSpeechModel(@"C:\Users\KakoytoChel228\Desktop\vosk-model-uk-v3");

            // Setting up the default page after the speech model has been initialized
            navigationService.ChangePage<MainPage>();
        }
    }
}
