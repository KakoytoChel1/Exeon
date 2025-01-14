using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Exeon.Models.Chat
{
    public class AssistantMessageItem : MessageItem
    {
        public AssistantMessageItem() { }

        public AssistantMessageItem(IEnumerable<MessageItem> messages)
        {
            MessageItems = new ObservableCollection<MessageItem>(messages);
        }

        public ObservableCollection<MessageItem> MessageItems { get; set; } = null!;
    }
}
