using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// Aria2进程服务
    /// </summary>
    public static class Aria2ProcessHelper
    {
        // Aria2下载进程的ID号
        private static int Aria2ProcessID = default;

        /// <summary>
        /// 获取当前进程的PID信息
        /// </summary>
        public static int GetProcessID()
        {
            return Aria2ProcessID;
        }

        /// <summary>
        /// 让Aria2以RPC方式启动，并让其在后台运行
        /// </summary>
        public static async Task RunAria2Async(string fileName, string arguments)
        {
            //设置启动程序的信息
            ProcessStartInfo Aria2Info = new ProcessStartInfo
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

            // 启动Aria2下载进程，并设置进程ID号
            Aria2ProcessID = Process.Start(Aria2Info).Id;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 应用程序关闭时，需要终止相应的Aria2进程树
        /// </summary>
        public static void KillProcessAndChildren(int processID)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + processID);
            ManagementObjectCollection moc = searcher.Get();

            foreach (ManagementBaseObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }

            try
            {
                Process process = Process.GetProcessById(processID);
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }

            // 无法终止相关联的进程。
            catch (Win32Exception)
            {
                return;
            }

            // 不支持远程计算机上运行的进程调用 Kill()。 该方法仅对本地计算机上运行的进程可用。
            catch (NotSupportedException)
            {
                return;
            }
            // 没有与此 Process 对象关联的进程
            catch (InvalidOperationException)
            {
                return;
            }
        }

        /// <summary>
        /// 判断Aria2进程是否存活
        /// </summary>
        public static bool IsAria2ProcessExisted()
        {
            try
            {
                if (GetProcessID() != default)
                {
                    Process.GetProcessById(GetProcessID());
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // 无法终止相关联的进程。
            catch (Win32Exception)
            {
                return false;
            }

            // 不支持远程计算机上运行的进程调用 Kill()。 该方法仅对本地计算机上运行的进程可用。
            catch (NotSupportedException)
            {
                return false;
            }

            // 没有与此 Process 对象关联的进程
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
