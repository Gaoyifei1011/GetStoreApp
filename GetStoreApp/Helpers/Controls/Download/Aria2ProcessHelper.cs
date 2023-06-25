using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.Helpers.Controls.Download
{
    /// <summary>
    /// Aria2进程服务辅助类
    /// </summary>
    public static class Aria2ProcessHelper
    {
        private static STARTUPINFO Aria2ProcessStartupInfo;

        private static PROCESS_INFORMATION Aria2ProcessInformation;

        /// <summary>
        /// 让Aria2以RPC方式启动，并让其在后台运行
        /// </summary>
        public static unsafe bool RunAria2Process(string fileName, string arguments = "")
        {
            try
            {
                Kernel32Library.GetStartupInfo(out Aria2ProcessStartupInfo);
                Aria2ProcessStartupInfo.lpReserved = null;
                Aria2ProcessStartupInfo.lpDesktop = null;
                Aria2ProcessStartupInfo.lpTitle = null;
                Aria2ProcessStartupInfo.dwX = 0;
                Aria2ProcessStartupInfo.dwY = 0;
                Aria2ProcessStartupInfo.dwXSize = 0;
                Aria2ProcessStartupInfo.dwYSize = 0;
                Aria2ProcessStartupInfo.dwXCountChars = 500;
                Aria2ProcessStartupInfo.dwYCountChars = 500;
                Aria2ProcessStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                Aria2ProcessStartupInfo.wShowWindow = WindowShowStyle.SW_HIDE;
                Aria2ProcessStartupInfo.cbReserved2 = 0;
                Aria2ProcessStartupInfo.lpReserved2 = IntPtr.Zero;
                Aria2ProcessStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                return Kernel32Library.CreateProcess(null, string.Format("{0} {1}", fileName, arguments), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref Aria2ProcessStartupInfo, out Aria2ProcessInformation);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Aria2 Process create failed.", e);
                return false;
            }
        }

        /// <summary>
        /// 应用程序关闭时，关闭Aria2下载进程
        /// </summary>
        public static void KillAria2Process()
        {
            try
            {
                if (Aria2ProcessInformation.dwProcessId is not 0)
                {
                    IntPtr hProcess = Kernel32Library.OpenProcess(EDesiredAccess.PROCESS_TERMINATE, false, Aria2ProcessInformation.dwProcessId);
                    if (hProcess != IntPtr.Zero)
                    {
                        Kernel32Library.TerminateProcess(hProcess, 0);
                    }
                    Kernel32Library.CloseHandle(hProcess);
                }
                if (Aria2ProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(Aria2ProcessInformation.hProcess);
                if (Aria2ProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(Aria2ProcessInformation.hThread);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Aria2 Process kill failed.", e);
                return;
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
                    throw new Exception();
                }

                PROCESSENTRY32 ProcessEntry32 = new PROCESSENTRY32();
                ProcessEntry32.dwSize = Marshal.SizeOf(typeof(PROCESSENTRY32));

                for (bool result = Kernel32Library.Process32First(hSnapshot, ref ProcessEntry32); result; result = Kernel32Library.Process32Next(hSnapshot, ref ProcessEntry32))
                {
                    if (ProcessEntry32.th32ProcessID == Aria2ProcessInformation.dwProcessId)
                    {
                        SearchResult = true;
                    }
                }
                Kernel32Library.CloseHandle(hSnapshot);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Aria2 Process search failed.", e);
                SearchResult = false;
            }
            return SearchResult;
        }
    }
}
