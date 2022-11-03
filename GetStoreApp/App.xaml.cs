using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Messages;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Popups;
using WinUIEx;

namespace GetStoreApp
{
    public partial class App : Application
    {
        private IActivationService ActivationService { get; set; }

        private IResourceService ResourceService { get; set; }

        public IAria2Service Aria2Service { get; set; }

        public IDownloadSchedulerService DownloadSchedulerService { get; set; }

        public static WindowEx MainWindow { get; } = new MainWindow();

        // 标志内容对话框是否处于正在打开状态。若是，则不再打开其他内容对话框，防止造成应用异常
        public static bool IsDialogOpening { get; set; } = false;

        // 导航页面后使用到的参数
        public static AppNaviagtionArgs NavigationArgs { get; set; } = AppNaviagtionArgs.None;

        public App()
        {
            InitializeComponent();
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 处理应用启动
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            ContainerHelper.InitializeContainer();

            ActivationService = ContainerHelper.GetInstance<IActivationService>();
            ResourceService = ContainerHelper.GetInstance<IResourceService>();
            Aria2Service = ContainerHelper.GetInstance<IAria2Service>();
            DownloadSchedulerService = ContainerHelper.GetInstance<IDownloadSchedulerService>();

            await RunSingleInstanceAppAsync();
            await ActivationService.ActivateAsync(args);
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private async void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            WeakReferenceMessenger.Default.Send(new TrayIconDisposeMessage(true));
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
            AppInstance.GetCurrent().Activated += OnAppActivated;
        }

        /// <summary>
        /// 关闭其他实例后，按照原来的状态显示已经打开的实例窗口
        /// </summary>
        private async void OnAppActivated(object sender, AppActivationArguments args)
        {
            WindowHelper.ShowAppWindow();

            MessageDialog dialog = new MessageDialog(ResourceService.GetLocalized("AppIsRunning"), ResourceService.GetLocalized("AppDisplayName"));
            dialog.Commands.Add(new UICommand(ResourceService.GetLocalized("OK")));
            dialog.DefaultCommandIndex = 0;
            WinRT.Interop.InitializeWithWindow.Initialize(dialog, WinRT.Interop.WindowNative.GetWindowHandle(MainWindow));
            await dialog.ShowAsync();
        }
    }
}
