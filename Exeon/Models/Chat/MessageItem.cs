using Exeon.ViewModels.Tools;
using System;

namespace Exeon.Models.Chat
{
    public abstract class MessageItem : ObservableObject
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTime SendingTime
        {
            get { return DateTime.Now; }
        }
    }
}
