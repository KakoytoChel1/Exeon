using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System;

namespace Exeon.ViewModels.Tools
{
    public static class DialogManager
    {
        public static async Task<ContentDialogResult> ShowContentDialog<TContent>(XamlRoot root, string title,
            string primaryBtnText, ContentDialogButton defaultBtn, TContent content, string? secondarybtnText = null, string? closeBtnText = null)
        {
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = root;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = title;
            dialog.PrimaryButtonText = primaryBtnText;
            dialog.CloseButtonText = closeBtnText;
            dialog.SecondaryButtonText = secondarybtnText;
            dialog.DefaultButton = defaultBtn;
            dialog.Content = content;

            return await dialog.ShowAsync();
        }
    }
}
