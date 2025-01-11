using Exeon.Models.Chat;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace Exeon.ViewModels.Tools
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? UserMessageTemplate { get; set; }
        public DataTemplate? AssistantMessageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                UserMessageItem => UserMessageTemplate!,
                AssistantMessageItem => AssistantMessageTemplate!,
                _ => base.SelectTemplateCore(item, container)
            };
        }
    }
}
