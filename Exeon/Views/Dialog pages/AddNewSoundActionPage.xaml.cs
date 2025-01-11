using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Exeon.Views.Dialog_pages
{
    public sealed partial class AddNewSoundActionPage : Page
    {
        public ModifyCommandPageViewModel ViewModel { get; private set; }

        public AddNewSoundActionPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ModifyCommandPageViewModel>();
        }
    }
}
