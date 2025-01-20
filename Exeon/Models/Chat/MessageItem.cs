using Exeon.ViewModels.Tools;
using System;
using System.ComponentModel.DataAnnotations;

namespace Exeon.Models.Chat
{
    public abstract class MessageItem : ObservableObject
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTime SendingTime
        {
            get { return DateTime.Now; }
        }
    }
}
