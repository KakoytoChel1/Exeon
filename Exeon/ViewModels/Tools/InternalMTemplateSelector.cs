using Exeon.Models.Chat;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Exeon.ViewModels.Tools
{
    public class InternalMTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? AssistantActionSucceededMessageTemplate { get; set; }
        public DataTemplate? AssistantActionFailedMessageTemplate { get; set; }
        public DataTemplate? AssistantActionDelayMessageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                AssistantActionSucceededMessageItem => AssistantActionSucceededMessageTemplate!,
                AssistantActionFailedMessageItem => AssistantActionFailedMessageTemplate!,
                AssistantActionDelayMessageItem => AssistantActionDelayMessageTemplate!,
                _ => base.SelectTemplateCore(item, container)
            };
        }
    }
}
