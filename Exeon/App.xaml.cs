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
using System.IO;

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
            serviceCollection.AddSingleton<SettingsPageViewModel>();

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

            var configService = Services.GetRequiredService<IConfigurationService>();
            var appState = Services.GetRequiredService<AppState>();
            var navigationService = Services.GetRequiredService<INavigationService>();
            var speechRecognitionService = Services.GetRequiredService<ISpeechRecognitionService>();

            // Setting up the first visible page
            navigationService.InitializeFrame(MainWindow.RootFrameProperty);

            // Setting up the loading page until the speech recognition model is initilized
            navigationService.ChangePage<LoadingPage>();

            await configService.InitAsync();
            await appState.InitializeDataBase();

            appState.IsApproximateModeOn =
                configService.Get<bool>("IsApproximateModeOn");
            var pathToSpeechModel = configService.Get<bool>("SpeechModelPath");

            var extractedPathToModel = configService.Get<string>("SpeechModelPath");

            if (!string.IsNullOrWhiteSpace(extractedPathToModel) && Directory.Exists(extractedPathToModel))
            {
                // Initializing the speech model
                await speechRecognitionService.InitializeSpeechModel(extractedPathToModel);

                appState.IsSpeechModelInitializingFailed = !speechRecognitionService.IsInitialized;

                // Setting up the default page after the speech model has been initialized
                navigationService.ChangePage<MainPage>();
            }
            else
            {
                navigationService.ChangePage<MainPage>();
                appState.IsSpeechModelInitializingFailed = true;
            }
        }
    }
}
