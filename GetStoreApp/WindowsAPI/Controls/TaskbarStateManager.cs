using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using Windows.UI.Shell;

namespace GetStoreApp.WindowsAPI.Controls
{
    /// <summary>
    /// 获取和修改任务栏的进度或状态
    /// </summary>
    public static class TaskbarStateManager
    {
        private static ITaskbarList TaskbarList { get; }

        // 隐藏默认构造函数
        static TaskbarStateManager()
        {
            TaskbarList = (ITaskbarList)new CTaskbarList();
            TaskbarList.HrInit();
        }

        /// <summary>
        /// 指示当前平台是否支持此功能。
        /// </summary>
        public static bool IsPlatformSupported { get; } = TaskbarManager.GetDefault().IsSupported;

        /// <summary>
        /// 显示或更新主应用程序窗口的任务栏按钮中承载的进度条，以显示完整操作的特定完成百分比。
        /// </summary>
        /// <param name="currentValue">一个应用程序定义的值，指示在调用方法时已完成的操作的比例。</param>
        /// <param name="maximumValue">一个应用程序定义的值，它指定操作完成时当前值将具有的值。</param>
        public static void SetProgressValue(int currentValue, int maximumValue, IntPtr windowHandle)
        {
            if (IsPlatformSupported)
            {
                TaskbarList.SetProgressValue(windowHandle, currentValue, maximumValue);
            }
        }

        /// <summary>
        /// 设置显示在主应用程序窗口的任务栏按钮上的进度指示器的类型和状态。
        /// </summary>
        /// <param name="flags">进度按钮的标志</param>
        public static void SetProgressState(TBPFLAG flags, IntPtr windowHandle)
        {
            if (IsPlatformSupported)
            {
                TaskbarList.SetProgressState(windowHandle, flags);
            }
        }
    }
}
