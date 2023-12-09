using Microsoft.UI.Dispatching;
using System;
using System.Threading;

namespace TaskbarPinner
{
    /// <summary>
    /// DispatcherQueueSyncContext 允许开发人员等待调用并返回到UI线程。需要通过 DispatcherQueueSyncContext.SetForCurrentThread 安装在UI线程上。
    /// </summary>
    public class DispatcherQueueSynchronizationContext : SynchronizationContext
    {
        private readonly DispatcherQueue m_dispatcherQueue;

        public DispatcherQueueSynchronizationContext(DispatcherQueue dispatcherQueue)
        {
            m_dispatcherQueue = dispatcherQueue;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d));
            }

            m_dispatcherQueue.TryEnqueue(() => d(state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException("Send not supported");
        }

        public override SynchronizationContext CreateCopy()
        {
            return new DispatcherQueueSynchronizationContext(m_dispatcherQueue);
        }
    }
}
