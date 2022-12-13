using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.Views.Window;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Threading.Tasks;
using WinRT.Interop;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用激活服务
    /// </summary>
    public static class ActivationService
    {
        public static async Task ActivateAsync(LaunchActivatedEventArgs activationArgs)
        {
            // 在应用窗口激活前配置应用的设置
            await InitializeAsync();

            App.MainWindow = new MainWindow();

            await StartupService.InitializeStartupAsync();

            // 激活应用窗口
            App.MainWindow.Activate();

            nint WindowHandle = WindowNative.GetWindowHandle(App.MainWindow);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(WindowHandle);

            App.AppWindow = AppWindow.GetFromWindowId(windowId);
            App.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            App.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Logo/GetStoreApp.ico"));

            // 窗口激活后配置其他设置
            await StartupAsync();
        }

        /// <summary>
        /// 在应用窗口激活前配置应用的设置
        /// </summary>
        private static async Task InitializeAsync()
        {
            ResourceService.InitializeResourceList();

            // 初始化数据库信息
            await DataBaseService.InitializeDataBaseAsync();
            await DownloadDBService.InitializeDownloadDBAsync();

            // 初始化应用配置信息
            await AppExitService.InitializeAppExitAsync();
            await InstallModeService.InitializeInstallModeAsync();

            await AlwaysShowBackdropService.InitializeAlwaysShowBackdropAsync();
            await BackdropService.InitializeBackdropAsync();
            await ThemeService.InitializeThemeAsync();
            await TopMostService.InitializeTopMostValueAsync();

            await DownloadOptionsService.InitializeAsync();
            await HistoryLiteNumService.InitializeHistoryLiteNumAsync();
            await LinkFilterService.InitializeLinkFilterValueAsnyc();
            await NotificationService.InitializeNotificationAsync();
            await RegionService.InitializeRegionAsync();
            await UseInstructionService.InitializeUseInsVisValueAsync();

            // 实验功能设置配置
            await NetWorkMonitorService.InitializeNetWorkMonitorValueAsync();
        }

        /// <summary>
        /// 窗口激活后配置其他设置
        /// </summary>
        private static async Task StartupAsync()
        {
            // 设置应用主题
            await ThemeService.SetAppThemeAsync();

            // 设置应用背景色
            await BackdropService.SetAppBackdropAsync();

            // 设置应用置顶状态
            await TopMostService.SetAppTopMostAsync();

            // 初始化下载监控服务
            await DownloadSchedulerService.InitializeDownloadSchedulerAsync();

            // 初始化Aria2配置文件信息
            await Aria2Service.InitializeAria2ConfAsync();

            // 启动Aria2下载服务（该服务会在后台长时间运行）
            await Aria2Service.StartAria2Async();
        }
    }
}
