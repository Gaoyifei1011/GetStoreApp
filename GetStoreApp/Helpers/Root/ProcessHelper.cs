using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 进程辅助类
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// 根据进程名称获取所有的进程列表信息
        /// </summary>
        public static unsafe List<uint> GetProcessPidByName(string processName)
        {
            if (!processName.EndsWith(".exe"))
            {
                processName += ".exe";
            }

            List<uint> processEntry32PIDList = new List<uint>();
            try
            {
                IntPtr hSnapshot = Kernel32Library.CreateToolhelp32Snapshot(CREATE_TOOLHELP32_SNAPSHOT_FLAGS.TH32CS_SNAPPROCESS, 0);

                if (hSnapshot == IntPtr.Zero || hSnapshot == Kernel32Library.INVALID_HANDLE_VALUE)
                {
                    return processEntry32PIDList;
                }

                PROCESSENTRY32 processEntry32 = new PROCESSENTRY32();
                processEntry32.dwSize = Marshal.SizeOf<PROCESSENTRY32>();

                for (bool result = Kernel32Library.Process32First(hSnapshot, ref processEntry32); result; result = Kernel32Library.Process32Next(hSnapshot, ref processEntry32))
                {
                    if (new string(processEntry32.szExeFile).Equals(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        processEntry32PIDList.Add(processEntry32.th32ProcessID);
                    }
                }
                Kernel32Library.CloseHandle(hSnapshot);
                return processEntry32PIDList;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Get process pid list failed.", e);
                return processEntry32PIDList;
            }
        }

        /// <summary>
        /// 创建进程
        /// </summary>
        public static void StartProcess(string processName, string arguments, out int processid)
        {
            Kernel32Library.GetStartupInfo(out STARTUPINFO startupInfo);
            startupInfo.lpReserved = IntPtr.Zero;
            startupInfo.lpDesktop = IntPtr.Zero;
            startupInfo.lpTitle = IntPtr.Zero;
            startupInfo.dwX = 0;
            startupInfo.dwY = 0;
            startupInfo.dwXSize = 0;
            startupInfo.dwYSize = 0;
            startupInfo.dwXCountChars = 500;
            startupInfo.dwYCountChars = 500;
            startupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
            startupInfo.wShowWindow = WindowShowStyle.SW_SHOWNORMAL;
            startupInfo.cbReserved2 = 0;
            startupInfo.lpReserved2 = IntPtr.Zero;
            startupInfo.cb = Marshal.SizeOf<STARTUPINFO>();

            bool createResult = Kernel32Library.CreateProcess(null, string.Format("{0} {1}", processName, arguments), IntPtr.Zero, IntPtr.Zero, false, CREATE_PROCESS_FLAGS.None, IntPtr.Zero, null, ref startupInfo, out PROCESS_INFORMATION processInformation);

            if (createResult)
            {
                if (processInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(processInformation.hProcess);
                if (processInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(processInformation.hThread);
                processid = processInformation.dwProcessId;
            }
            else
            {
                processid = 0;
            }
        }
    }
}
