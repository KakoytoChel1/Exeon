namespace Exeon.Models.Chat
{
    public class UserMessageItem : MessageItem
    {
        public UserMessageItem() { }

        public UserMessageItem(string messageText)
        {
            Text = messageText;
        }
    }
}
