using Exeon.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage.Pickers;

namespace Exeon.Views.Dialog_pages
{
    public sealed partial class AddNewFileActionPage : Page
    {
        public ModifyCommandPageViewModel ViewModel { get; private set; }

        public AddNewFileActionPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ModifyCommandPageViewModel>();
        }

        private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var senderButton = sender as Button;
            senderButton!.IsEnabled = false;

            // Clear previous returned file name, if it exists, between iterations of this scenario
            FilePathTextBox.Text = "";

            // Create a file picker
            var openPicker = new FileOpenPicker();

            // See the sample code below for how to make the window accessible from the App class.
            var window = App.MainWindow;

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                FilePathTextBox.Text = file.Path;
            }

            //re-enable the button
            senderButton.IsEnabled = true;

        }
    }
}
