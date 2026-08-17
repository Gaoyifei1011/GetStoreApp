using System;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用进程内全局通知
    /// </summary>
    internal static class GlobalNotificationService
    {
        /// <summary>
        /// 应用程序退出时发生的事件
        /// </summary>
        internal static event Action ApplicationExit;

        /// <summary>
        /// 发送消息通知
        /// </summary>
        internal static void SendNotification()
        {
            ApplicationExit?.Invoke();
        }
    }
}
