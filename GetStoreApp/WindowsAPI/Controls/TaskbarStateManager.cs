using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;
using Windows.UI.Shell;

namespace GetStoreApp.WindowsAPI.Controls
{
    /// <summary>
    /// 获取和修改任务栏的进度或状态
    /// </summary>
    public static class TaskbarStateManager
    {
        private static readonly Guid CLSID_TaskbarList = new Guid("56FDF344-FD6D-11d0-958A-006097C9A090");

        private static readonly Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        private static ITaskbarList TaskbarList;

        // 隐藏默认构造函数
        static unsafe TaskbarStateManager()
        {
            try
            {
                fixed (Guid* CLSID_TaskbarList_Ptr = &CLSID_TaskbarList, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_TaskbarList_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, IID_IUnknown_Ptr, out IntPtr obj);

                    if (obj != IntPtr.Zero)
                    {
                        TaskbarList = (ITaskbarList)Marshal.GetTypedObjectForIUnknown(obj, typeof(ITaskbarList));
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "TaskbarStateManager initialize failed.", e);
                return;
            }

            TaskbarList?.HrInit();
        }

        /// <summary>
        /// 指示当前平台是否支持此功能
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
            if (TaskbarList is not null && IsPlatformSupported)
            {
                TaskbarList.SetProgressState(windowHandle, flags);
            }
        }
    }
}
