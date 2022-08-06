using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// Aria2下载服务
    /// </summary>
    public class Aria2Service : IAria2Service
    {
        private string Aria2Path { get; } = Path.Combine(AppContext.BaseDirectory, "Aria2\\aria2c.exe");

        private string Aria2ConfPath = Path.Combine(AppContext.BaseDirectory, "Aria2\\Config\\aria2.conf");

        /// <summary>
        /// 初始化运行Aria2下载进程
        /// </summary>
        public async Task InitializeAria2Async()
        {
            string Aria2ExecuteCmd = string.Format("{0} --conf-path=\"{1}\" -D", Aria2Path, Aria2ConfPath);

            await Aria2ProcessHelper.RunCmdAsync();
            await Aria2ProcessHelper.ExecuteCmdAsync(Aria2ExecuteCmd);
        }

        /// <summary>
        /// 关闭Aria2进程
        /// </summary>
        public async Task CloseAria2Async()
        {
            int ProcessID = Aria2ProcessHelper.GetProcessID();
            Aria2ProcessHelper.KillProcessAndChildren(ProcessID);
            await Task.CompletedTask;
        }
    }
}
