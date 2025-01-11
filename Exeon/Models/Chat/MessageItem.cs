using System;

namespace Exeon.Models.Chat
{
    public abstract class MessageItem
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTime SendingTime { get; set; }
    }
}
