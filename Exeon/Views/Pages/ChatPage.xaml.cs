using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Exeon.Views.Pages
{
    public sealed partial class ChatPage : Page
    {
        public ChatPageViewModel ViewModel { get; private set; }

        public ChatPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ChatPageViewModel>();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
    }
}
