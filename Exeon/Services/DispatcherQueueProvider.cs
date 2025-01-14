using Microsoft.UI.Dispatching;
using System;

namespace Exeon.Services
{
    public class DispatcherQueueProvider
    {
        public DispatcherQueue? DispatcherQueue { get; private set; }

        public void Initialize(DispatcherQueue dispatcherQueue)
        {
            DispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
        }
    }
}
