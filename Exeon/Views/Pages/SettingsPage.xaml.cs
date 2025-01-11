using Microsoft.UI.Xaml.Controls;
using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Navigation;

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
    }
}
