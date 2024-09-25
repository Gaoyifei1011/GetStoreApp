using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
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

            List<uint> processEntry32PIDList = [];

            try
            {
                IntPtr hSnapshot = Kernel32Library.CreateToolhelp32Snapshot(CREATE_TOOLHELP32_SNAPSHOT_FLAGS.TH32CS_SNAPPROCESS, 0);

                if (hSnapshot == IntPtr.Zero || hSnapshot == Kernel32Library.INVALID_HANDLE_VALUE)
                {
                    return processEntry32PIDList;
                }

                PROCESSENTRY32 processEntry32 = new()
                {
                    dwSize = sizeof(PROCESSENTRY32)
                };

                for (bool result = Kernel32Library.Process32First(hSnapshot, ref processEntry32); result; result = Kernel32Library.Process32Next(hSnapshot, ref processEntry32))
                {
                    if (new string(processEntry32.szExeFile).Equals(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        processEntry32PIDList.Add(processEntry32.th32ProcessID);
                    }
                }

                new SafeWaitHandle(hSnapshot, true).Dispose();
                return processEntry32PIDList;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Get process pid list failed.", e);
                return processEntry32PIDList;
            }
        }
    }
}
