using GetStoreApp.Helpers.Root;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 任务栏辅助进程服务
    /// </summary>
    public static class TaskbarService
    {
        private static string TaskbarProcessFilePath { get; } = string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), @"GetStoreAppHelper.exe");

        /// <summary>
        /// 初始化运行任务栏辅助进程
        /// </summary>
        public static async Task StartTaskbarProcessAsync()
        {
            TaskbarProcessHelper.RunTaskbarProcess(TaskbarProcessFilePath, string.Empty);
            await Task.CompletedTask;
        }
    }
}
