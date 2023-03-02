using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// GetStoreAppHelper 辅助程序程序管理
    /// </summary>
    public static class TaskbarProcessHelper
    {
        private static int TaskbarProcessID = default;

        /// <summary>
        /// 获取当前进程的PID信息
        /// </summary>
        public static int GetProcessID()
        {
            return TaskbarProcessID;
        }

        /// <summary>
        /// 启动任务栏通知区域进程
        /// </summary>
        public static async Task RunTaskbarProcessAsync(string fileName, [Optional, DefaultParameterValue("")] string arguments)
        {
            ProcessStartInfo TaskbarProcessInfo = new ProcessStartInfo
            {
                //设置外部程序名
                FileName = fileName,

                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,

                //最小化方式启动
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = arguments
            };

            // 启动任务栏进程，并获取进程ID号
            TaskbarProcessID = Process.Start(TaskbarProcessInfo).Id;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 应用程序关闭时，关闭任务栏通知区域进程
        /// </summary>
        public static void KillProcess(int processID)
        {
            Process[] GetStoreAppHelperProcess = Process.GetProcessesByName("GetStoreAppHelper");

            if (GetStoreAppHelperProcess.Length > 0)
            {
                IntPtr hwnd = IntPtr.Zero;
                do
                {
                    hwnd = User32Library.FindWindowEx(IntPtr.Zero, hwnd, "Mile.Xaml.ContentWindow", null);

                    if (hwnd != IntPtr.Zero)
                    {
                        User32Library.GetWindowThreadProcessId(hwnd, out int processId);

                        if (processId is not 0)
                        {
                            bool result = false;
                            foreach (Process process in GetStoreAppHelperProcess)
                            {
                                if (process.Id == processId)
                                {
                                    User32Library.SendMessage(hwnd, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.Exit), IntPtr.Zero);
                                    result = true;
                                    break;
                                }
                            }

                            if (result) break;
                        }
                    }
                }
                while (hwnd != IntPtr.Zero);
            }
        }
    }
}
