using Exeon.Models.Chat;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace Exeon.ViewModels.Tools
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? UserMessageTemplate { get; set; }
        public DataTemplate? AssistantSimpleMessageTemplate { get; set; }
        public DataTemplate? AssistantActionSucceededMessageTemplate { get; set; }
        public DataTemplate? AssistantActionFailedMessageTemplate { get; set; }
        public DataTemplate? AssistantActionDelayMessageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                UserMessageItem => UserMessageTemplate!,
                AssistantSimpleMessageItem => AssistantSimpleMessageTemplate!,
                AssistantActionSucceededMessageItem => AssistantActionSucceededMessageTemplate!,
                AssistantActionFailedMessageItem => AssistantActionFailedMessageTemplate!,
                AssistantActionDelayMessageItem => AssistantActionDelayMessageTemplate!,
                _ => base.SelectTemplateCore(item, container)
            };
        }
    }
}
