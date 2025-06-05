using Microsoft.UI.Xaml.Controls;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Navigation;
using Exeon.Services.IServices;

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
    }
}
