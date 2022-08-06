using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading.Tasks;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// Aria2进程服务
    /// </summary>
    public static class Aria2ProcessHelper
    {
        private static Process process = new Process();

        /// <summary>
        /// 获取当前进程的PID信息
        /// </summary>
        /// <returns></returns>
        public static int GetProcessID()
        {
            return process.Id;
        }

        /// <summary>
        /// 启动命令提示符进程
        /// </summary>
        public static async Task RunCmdAsync()
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            await Task.CompletedTask;
        }

        /// <summary>
        /// 命令提示符进程运行后，输入Aria2 RPC方式执行命令，并让其在后台运行
        /// </summary>
        public static async Task ExecuteCmdAsync(string executeCmd)
        {
            StreamWriter ConhostWriter = process.StandardInput;
            process.BeginOutputReadLine();
            if (!string.IsNullOrEmpty(executeCmd)) ConhostWriter.WriteLine(executeCmd);
            ConhostWriter.Close();

            await process.WaitForExitAsync();
        }

        /// <summary>
        /// 应用程序关闭时，需要终止相应的Aria2进程树（暂未实现日志记录，异常捕捉用于记录日志）
        /// </summary>
        public static void KillProcessAndChildren(int processID)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + processID);
            ManagementObjectCollection moc = searcher.Get();

            foreach (ManagementObject mo in moc) KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));

            try
            {
                Process process = Process.GetProcessById(processID);
                process.Kill();
            }

            // 无法终止相关联的进程。
            catch (Win32Exception) { }

            // 不支持远程计算机上运行的进程调用 Kill()。 该方法仅对本地计算机上运行的进程可用。
            catch (NotSupportedException) { }
            // 没有与此 Process 对象关联的进程
            catch (InvalidOperationException) { }
        }
    }
}
