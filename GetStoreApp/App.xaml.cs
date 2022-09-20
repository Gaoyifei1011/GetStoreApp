using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.UI.Controls.Custom;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Win32.Foundation;
using WinUIEx;

namespace GetStoreApp
{
    public partial class App : Application
    {
        private IActivationService ActivationService { get; }

        private IResourceService ResourceService { get; }

        public IAria2Service Aria2Service { get; }

        public static WindowEx MainWindow { get; } = new MainWindow();

        // 导航页面后使用到的参数
        public static string NavigationArgs { get; set; } = string.Empty;

        public App()
        {
            InitializeComponent();

            IOCHelper.InitializeIOCService();

            ActivationService = IOCHelper.GetService<IActivationService>();
            ResourceService = IOCHelper.GetService<IResourceService>();
            Aria2Service = IOCHelper.GetService<IAria2Service>();

            UnhandledException += OnUnhandledException;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            await RunSingleInstanceAppAsync();

            await ActivationService.ActivateAsync(args);
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private async void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            await Aria2Service.CloseAria2Async();
        }

        /// <summary>
        /// 应用程序只运行单个实例
        /// </summary>
        private async Task RunSingleInstanceAppAsync()
        {
            // 获取已经激活的参数
            AppActivationArguments appArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            // 获取或注册主实例
            AppInstance mainInstance = AppInstance.FindOrRegisterForKey("Main");

            // 如果主实例不是此当前实例
            if (!mainInstance.IsCurrent)
            {
                // 将激活重定向到该实例
                await mainInstance.RedirectActivationToAsync(appArgs);

                // 然后退出实例并停止
                Process.GetCurrentProcess().Kill();
                return;
            }

            // 否则将注册激活重定向
            AppInstance.GetCurrent().Activated += AppActivated;
        }

        /// <summary>
        /// 关闭其他实例后，按照原来的状态显示已经打开的实例窗口
        /// </summary>
        private void AppActivated(object sender, AppActivationArguments e)
        {
            WindowHelper.SetAppWindow();

            // 显示提示消息对话框
            WinUIMessageBox.ShowMessageBox(
                (HWND)WinRT.Interop.WindowNative.GetWindowHandle(MainWindow),
                ResourceService.GetLocalized("AppIsRunning"),
                ResourceService.GetLocalized("AppDisplayName"),
                WinUIMessageBox.MB_OK | WinUIMessageBox.MB_ICONINFORMATION | WinUIMessageBox.MB_APPLMODAL | WinUIMessageBox.MB_TOPMOST
                );
        }
    }
}
