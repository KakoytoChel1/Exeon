using Microsoft.UI.Xaml.Controls;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Navigation;
using Exeon.Services.IServices;
using Windows.Storage;
using System.IO;
using System;
using System.Diagnostics;

namespace Exeon.Views.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPageViewModel ViewModel { get; private set; }

        public SettingsPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<SettingsPageViewModel>();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void ApproximateModeHelpBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            TeachingTip.IsOpen = true;
        }

        private void ApproxModeToggle_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(ApproxModeToggle.IsOn != App.Services.GetRequiredService<IConfigurationService>()
                .Get<bool>("IsApproximateModeOn"))
            {
                ViewModel.UnsavedChangesExist = true;
            }
        }

        private void openLocalFolderBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var path = localFolder.Path;

            OpenFolderInExplorer(path);
        }

        public async void OpenFolderInExplorer(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    Process.Start("explorer.exe", folderPath);
                }
                else
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = $"Folder wasn't found. The path: {folderPath}",
                        CloseButtonText = "ОК"
                    };
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"An error occurred while trying to access the following path: {ex.Message}",
                    CloseButtonText = "ОК"
                };
                await dialog.ShowAsync();
            }
        }
    }
}
