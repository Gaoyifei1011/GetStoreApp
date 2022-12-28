using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.Window;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用激活服务
    /// </summary>
    public static class ActivationService
    {
        public static async Task ActivateAsync(LaunchActivatedEventArgs activationArgs)
        {
            App.MainWindow = new MainWindow();

            // 激活应用窗口
            App.MainWindow.Activate();

            nint WindowHandle = MainWindow.GetMainWindowHandle();
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(WindowHandle);

            App.AppWindow = AppWindow.GetFromWindowId(windowId);
            App.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            App.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/GetStoreApp.ico"));

            // 窗口激活后配置其他设置
            await StartupAsync();
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
