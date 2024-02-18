using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Shell;

namespace GetStoreApp.Views.Windows
{
    /// <summary>
    /// 表示应用程序的 UI 窗口
    /// </summary>
    public sealed class FrameworkView : IFrameworkView
    {
        /// <summary>
        /// 初始化视图
        /// </summary>
        public void Initialize(CoreApplicationView applicationView)
        { }

        /// <summary>
        /// 将窗口与视图相关联
        /// </summary>
        public void SetWindow(CoreWindow window)
        { }

        /// <summary>
        /// 加载视图
        /// </summary>
        public void Load(string entryPoint)
        { }

        /// <summary>
        /// 将执行传递给视图提供程序
        /// </summary>
        public void Run()
        {
            CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
            coreWindow.Activate();
            coreWindow.DispatcherQueue.TryEnqueue(async () =>
            {
                string taskbarInfo = ResultService.ReadResult<string>(ConfigKey.TaskbarPinInfoKey);
                if (!string.IsNullOrEmpty(taskbarInfo))
                {
                    try
                    {
                        string[] taskbarInfoContents = taskbarInfo.Split(' ');

                        if (taskbarInfoContents.Length is 2)
                        {
                            PackageManager packageManager = new PackageManager();
                            Package package = packageManager.FindPackageForUser(string.Empty, taskbarInfoContents[0]);

                            if (package is not null)
                            {
                                foreach (AppListEntry applistItem in package.GetAppListEntries())
                                {
                                    if (applistItem.AppUserModelId.Equals(taskbarInfoContents[1]))
                                    {
                                        bool pinResult = await TaskbarManager.GetDefault().RequestPinAppListEntryAsync(applistItem);
                                        ResultService.SaveResult(ConfigKey.TaskbarPinnedResultKey, pinResult);
                                        ApplicationData.Current.SignalDataChanged();
                                    }
                                }
                            }
                        }

                        ResultService.SaveResult(ConfigKey.TaskbarPinInfoKey, string.Empty);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Pin app to taskbar failed", e);
                        ResultService.SaveResult(ConfigKey.TaskbarPinnedResultKey, false);
                        ApplicationData.Current.SignalDataChanged();
                    }
                }

                Environment.Exit(Environment.ExitCode);
            });
            coreWindow.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
        }

        /// <summary>
        /// 将视图返回到未初始化状态
        /// </summary>
        public void Uninitialize()
        { }
    }
}
