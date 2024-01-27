using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Helpers.Controls.Download
{
    /// <summary>
    /// Aria2进程服务辅助类
    /// </summary>
    public static class Aria2ProcessHelper
    {
        private static PROCESS_INFORMATION Aria2Information;

        /// <summary>
        /// 让Aria2以RPC方式启动，并让其在后台运行
        /// </summary>
        public static bool RunAria2Process(string fileName, string arguments = "")
        {
            try
            {
                Kernel32Library.GetStartupInfo(out STARTUPINFO Aria2StartupInfo);
                Aria2StartupInfo.lpReserved = IntPtr.Zero;
                Aria2StartupInfo.lpDesktop = IntPtr.Zero;
                Aria2StartupInfo.lpTitle = IntPtr.Zero;
                Aria2StartupInfo.dwX = 0;
                Aria2StartupInfo.dwY = 0;
                Aria2StartupInfo.dwXSize = 0;
                Aria2StartupInfo.dwYSize = 0;
                Aria2StartupInfo.dwXCountChars = 500;
                Aria2StartupInfo.dwYCountChars = 500;
                Aria2StartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                Aria2StartupInfo.wShowWindow = WindowShowStyle.SW_HIDE;
                Aria2StartupInfo.cbReserved2 = 0;
                Aria2StartupInfo.lpReserved2 = IntPtr.Zero;
                Aria2StartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                return Kernel32Library.CreateProcess(null, string.Format("{0} {1}", fileName, arguments), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref Aria2StartupInfo, out Aria2Information);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Aria2 Process create failed.", e);
                return false;
            }
        }

        /// <summary>
        /// 判断Aria2进程是否存活
        /// </summary>
        public static bool IsAria2ProcessExisted()
        {
            bool SearchResult = false;
            try
            {
                IntPtr hSnapshot = Kernel32Library.CreateToolhelp32Snapshot(CreateToolhelp32SnapshotFlags.TH32CS_SNAPPROCESS, 0);

                if (hSnapshot == IntPtr.Zero || hSnapshot == Kernel32Library.INVALID_HANDLE_VALUE)
                {
                    return SearchResult;
                }

                PROCESSENTRY32 ProcessEntry32 = new PROCESSENTRY32();
                ProcessEntry32.dwSize = Marshal.SizeOf(typeof(PROCESSENTRY32));

                for (bool result = Kernel32Library.Process32First(hSnapshot, ref ProcessEntry32); result; result = Kernel32Library.Process32Next(hSnapshot, ref ProcessEntry32))
                {
                    if (ProcessEntry32.th32ProcessID == Aria2Information.dwProcessId)
                    {
                        SearchResult = true;
                    }
                }
                Kernel32Library.CloseHandle(hSnapshot);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Aria2 Process search failed.", e);
                SearchResult = false;
            }
            return SearchResult;
        }
    }
}
