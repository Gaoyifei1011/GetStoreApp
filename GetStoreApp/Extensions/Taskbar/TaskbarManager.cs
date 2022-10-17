using GetStoreApp.Extensions.Collection;
using GetStoreApp.Extensions.Enum;
using System;
using System.Diagnostics;

namespace GetStoreApp.Extensions.Taskbar
{
    /// <summary>
    /// 表示窗口任务栏的实例
    /// </summary>
    public class TaskbarManager
    {
        // 隐藏默认构造函数
        private TaskbarManager()
        {
        }

        // 最佳做法建议定义要锁定的私有对象
        private static object _syncLock = new object();

        private static TaskbarManager _instance;

        /// <summary>
        /// 表示窗口任务栏的实例
        /// </summary>
        public static TaskbarManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                            _instance = new TaskbarManager();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 指示当前平台是否支持此功能。
        /// </summary>
        public static bool IsPlatformSupported
        {
            get { return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.CompareTo(new Version(6, 1)) >= 0; }
        }

        /// <summary>
        /// 显示或更新主应用程序窗口的任务栏按钮中承载的进度条，以显示完整操作的特定完成百分比。
        /// </summary>
        /// <param name="currentValue">一个应用程序定义的值，指示在调用方法时已完成的操作的比例。</param>
        /// <param name="maximumValue">一个应用程序定义的值，它指定操作完成时当前值将具有的值。</param>
        public void SetProgressValue(int currentValue, int maximumValue)
        {
            if (IsPlatformSupported)
                TaskbarList.Instance.SetProgressValue(
                    OwnerHandle,
                    Convert.ToUInt32(currentValue),
                    Convert.ToUInt32(maximumValue));
        }

        /// <summary>
        /// 显示或更新驻留在给定窗口句柄的任务栏按钮中的进度条，以显示完整操作的特定完成百分比。
        /// </summary>
        /// <param name="windowHandle">其关联的任务栏按钮用作进度指示器的窗口的句柄。
        /// 此窗口属于与按钮的应用程序相关联的调用进程，并且必须已经加载。</param>
        /// <param name="currentValue">一个应用程序定义的值，指示在调用方法时已完成的操作的比例。</param>
        /// <param name="maximumValue">一个应用程序定义的值，它指定操作完成时当前值将具有的值。</param>
        public void SetProgressValue(int currentValue, int maximumValue, IntPtr windowHandle)
        {
            if (IsPlatformSupported)
                TaskbarList.Instance.SetProgressValue(
                    windowHandle,
                    Convert.ToUInt32(currentValue),
                    Convert.ToUInt32(maximumValue));
        }

        /// <summary>
        /// 设置显示在主应用程序窗口的任务栏按钮上的进度指示器的类型和状态。
        /// </summary>
        /// <param name="state">进度按钮的进度状态</param>
        public void SetProgressState(TaskbarProgressBarState state)
        {
            if (IsPlatformSupported)
                TaskbarList.Instance.SetProgressState(OwnerHandle, (TaskbarProgressBarStatus)state);
        }

        /// <summary>
        /// 设置给定窗口句柄的任务栏按钮上显示的进度指示器的类型和状态
        /// </summary>
        /// <param name="windowHandle">其关联的任务栏按钮用作进度指示器的窗口的句柄。
        /// 此窗口属于与按钮的应用程序关联的调用进程，并且必须已加载。</param>
        /// <param name="state">进度按钮的进度状态</param>
        public void SetProgressState(TaskbarProgressBarState state, IntPtr windowHandle)
        {
            if (IsPlatformSupported)
                TaskbarList.Instance.SetProgressState(windowHandle, (TaskbarProgressBarStatus)state);
        }

        private IntPtr _ownerHandle;

        /// <summary>
        /// 设置窗口的句柄，其任务栏按钮将用于显示进度。
        /// </summary>
        internal IntPtr OwnerHandle
        {
            get
            {
                if (_ownerHandle == IntPtr.Zero)
                {
                    Process currentProcess = Process.GetCurrentProcess();

                    if (currentProcess != null && currentProcess.MainWindowHandle != IntPtr.Zero)
                        _ownerHandle = currentProcess.MainWindowHandle;
                }

                return _ownerHandle;
            }
        }
    }
}
