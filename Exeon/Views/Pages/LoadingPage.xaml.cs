using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Exeon.Views.Pages
{
    public sealed partial class LoadingPage : Page
    {
        public AppState AppState { get; private set; }

        public LoadingPage()
        {
            this.InitializeComponent();
            AppState = App.Services.GetRequiredService<AppState>();
        }
    }
}
