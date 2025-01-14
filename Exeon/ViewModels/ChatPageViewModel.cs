using System;
using Exeon.Models.Chat;
using System.Collections.ObjectModel;
using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;

namespace Exeon.ViewModels
{
    public class ChatPageViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        public AppState AppState { get; }

        public ObservableCollection<MessageItem> MessageItems { get; set; }

        public ChatPageViewModel(INavigationService navigationService, AppState appState)
        {
            AppState = appState;
            _navigationService = navigationService;

            MessageItems = new ObservableCollection<MessageItem>();

            MessageItems.Add(new UserMessageItem() { Text = "test command", SendingTime = DateTime.Now});

            var internalMsg = new ObservableCollection<MessageItem>();
            
            internalMsg.Add(new AssistantActionSucceededMessageItem() { SendingTime = DateTime.Now, Text = "Successfully able to open file: 'C:/Aboba'!!!!" });
            internalMsg.Add(new AssistantActionFailedMessageItem() { Text = "Unable to open file: 'C:/Aboba', because it doesn't exist in the current directory.", SendingTime = DateTime.Now });
            internalMsg.Add(new AssistantActionDelayMessageItem() { Progress = 78 });

            var assistantMessage = new AssistantMessageItem(internalMsg);

            MessageItems.Add(assistantMessage);

            //MessageItems.Add(new UserMessageItem() { Text = "Привет", SendingTime = DateTime.Now});
            //MessageItems.Add(new AssistantMessageItem() { Text = "Здравствуйте!", SendingTime = DateTime.Now});
            //MessageItems.Add(new UserMessageItem() { Text = "Пока", SendingTime = DateTime.Now });
            //MessageItems.Add(new AssistantMessageItem() { Text = "Досвидания :)", SendingTime = DateTime.Now });
            //MessageItems.Add(new UserMessageItem() { Text = "Открыть браузер", SendingTime = DateTime.Now });
            //MessageItems.Add(new AssistantMessageItem() { Text = "Выполнение комманды - открытие директории: \"C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe\"", SendingTime = DateTime.Now });
        }
    }
}
