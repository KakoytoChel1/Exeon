using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Exeon.Views.Dialog_pages
{
    public sealed partial class AddNewWebActionPage : Page
    {
        public ModifyCommandPageViewModel ViewModel { get; private set; }

        public AddNewWebActionPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ModifyCommandPageViewModel>();
        }
    }
}
