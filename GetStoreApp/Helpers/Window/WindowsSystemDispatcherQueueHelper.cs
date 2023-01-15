using GetStoreApp.WindowsAPI.PInvoke.CoreMessaging;
using System.Runtime.InteropServices;
using Windows.System;

namespace GetStoreApp.Helpers.Window
{
    /// <summary>
    /// 线程调度器辅助类
    /// </summary>
    public class WindowsSystemDispatcherQueueHelper
    {
        private object m_dispatcherQueueController = null;

        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (DispatcherQueue.GetForCurrentThread() is not null)
            {
                return;
            }

            if (m_dispatcherQueueController is null)
            {
                DispatcherQueueOptions options = new DispatcherQueueOptions()
                {
                    dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions)),
                    threadType = 2,
                    apartmentType = 2
                };

                CoreMessagingLibrary.CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
            }
        }
    }
}
