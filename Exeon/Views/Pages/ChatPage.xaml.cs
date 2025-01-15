using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Specialized;
using Windows.System;

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

        private void TextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == VirtualKey.Enter)
            {
                if (ViewModel.SendEnteredTextCommand.CanExecute(null))
                {
                    ViewModel.SendEnteredTextCommand.Execute(null);
                }
            }
        }
    }
}
