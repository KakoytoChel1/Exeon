using Exeon.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Exeon.Models.Chat
{
    public class AssistantActionDelayMessageItem : MessageItem
    {
        private long _progress;
        public long Progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged(); }
        }

        public long Delay { get; set; }

        public async Task StartDelayAsync(CancellationToken cancellationToken)
        {
            var dispatcher = App.Services.GetRequiredService<DispatcherQueueProvider>().DispatcherQueue;

            if (dispatcher != null)
            {
                for (long i = 0; i <= Delay; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    dispatcher.TryEnqueue(() =>
                    {
                        Progress = (i * 100) / Delay;
                    });
                    await Task.Delay(1000);
                }
            }
        }
    }

}
