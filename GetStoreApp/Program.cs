using Microsoft.UI.Dispatching;
using System;
using System.Threading;

namespace GetStoreApp
{
    public class Program
    {
        /// <summary>
        /// 程序入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                DispatcherQueueSynchronizationContext context = new DispatcherQueueSynchronizationContext(
                    DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                new App();
            });
        }
    }
}
