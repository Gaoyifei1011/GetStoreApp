using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public static unsafe List<uint> GetProcessPIDByName(string processName)
        {
            if (!processName.EndsWith(".exe"))
            {
                processName += ".exe";
            }

            List<uint> ProcessEntry32PIDList = new List<uint>();
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
                    if (Marshal.PtrToStringUni((IntPtr)ProcessEntry32.szExeFile).Equals(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        ProcessEntry32PIDList.Add(ProcessEntry32.th32ProcessID);
                    }
                }
                Kernel32Library.CloseHandle(hSnapshot);
                return ProcessEntry32PIDList;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Get process Pid list failed.", e);
                return ProcessEntry32PIDList;
            }
        }
    }
}
