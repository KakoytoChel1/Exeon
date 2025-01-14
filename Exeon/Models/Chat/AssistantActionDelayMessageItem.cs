namespace Exeon.Models.Chat
{
    public class AssistantActionDelayMessageItem : MessageItem
    {
        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged(); }
        }
    }
}
