using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Exeon.Views.Dialog_pages
{
    public sealed partial class AddNewCustomCommandPage : Page
    {
        public CommandsPageViewModel ViewModel { get; private set; }

        public AddNewCustomCommandPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<CommandsPageViewModel>();
        }
    }
}
