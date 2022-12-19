using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.CoreMessaging
{
    public class CoreMessagingLibrary
    {
        private const string CoreMessaging = "CoreMessaging.dll";

        /// <summary>
        /// 在调用方线程上创建 DispatcherQueueController 。 使用创建的 DispatcherQueueController 创建和管理 DispatcherQueue 的生存期，
        /// 以便按调度程序队列线程的优先级顺序运行排队的任务。
        /// </summary>
        /// <param name="options">创建的 DispatcherQueueController 的线程关联和 COM 单元的类型。</param>
        /// <param name="dispatcherQueueController">创建的调度程序队列控制器。DispatcherQueueController 是 WinRT 对象。</param>
        /// <returns>成功创建返回S_OK ;否则为失败代码。</returns>
        [DllImport(CoreMessaging)]
        public static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);
    }
}
