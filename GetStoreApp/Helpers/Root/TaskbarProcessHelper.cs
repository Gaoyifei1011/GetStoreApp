using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// GetStoreAppHelper 辅助程序程序管理
    /// </summary>
    public static class TaskbarProcessHelper
    {
        private static STARTUPINFO TaskbarProcessStartupInfo;

        /// <summary>
        /// 启动任务栏通知区域进程
        /// </summary>
        public static unsafe bool RunTaskbarProcess(string fileName, [Optional, DefaultParameterValue("")] string arguments)
        {
            try
            {
                Kernel32Library.GetStartupInfo(out TaskbarProcessStartupInfo);
                TaskbarProcessStartupInfo.lpReserved = null;
                TaskbarProcessStartupInfo.lpDesktop = null;
                TaskbarProcessStartupInfo.lpTitle = null;
                TaskbarProcessStartupInfo.dwX = 0;
                TaskbarProcessStartupInfo.dwY = 0;
                TaskbarProcessStartupInfo.dwXSize = 0;
                TaskbarProcessStartupInfo.dwYSize = 0;
                TaskbarProcessStartupInfo.dwXCountChars = 500;
                TaskbarProcessStartupInfo.dwYCountChars = 500;
                TaskbarProcessStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                TaskbarProcessStartupInfo.wShowWindow = 0;
                TaskbarProcessStartupInfo.cbReserved2 = 0;
                TaskbarProcessStartupInfo.lpReserved2 = IntPtr.Zero;
                TaskbarProcessStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                return Kernel32Library.CreateProcess(null, string.Format("{0} {1}", fileName, arguments), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref TaskbarProcessStartupInfo, out PROCESS_INFORMATION TaskbarProcessInformation);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
