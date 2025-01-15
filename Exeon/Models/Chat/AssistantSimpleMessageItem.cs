namespace Exeon.Models.Chat
{
    public class AssistantSimpleMessageItem : MessageItem
    {
        public AssistantSimpleMessageItem() { }

        public AssistantSimpleMessageItem(string messageText)
        {
            Text = messageText;
        }
    }
}
