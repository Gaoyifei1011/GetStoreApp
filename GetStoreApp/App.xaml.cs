using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Messages;
using Microsoft.UI.Windowing;
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

        public static AppWindow AppWindow { get; private set; }

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
            InitializeService();

            AppWindow = WindowExtensions.GetAppWindow(MainWindow);

            if (InfoHelper.GetSystemVersion()["BuildNumber"] >= 22000)
            {
                AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            }

            await ActivationService.ActivateAsync(args);
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        private void InitializeService()
        {
            ActivationService = ContainerHelper.GetInstance<IActivationService>();
            ResourceService = ContainerHelper.GetInstance<IResourceService>();
            Aria2Service = ContainerHelper.GetInstance<IAria2Service>();
            DownloadSchedulerService = ContainerHelper.GetInstance<IDownloadSchedulerService>();
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private async void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            WeakReferenceMessenger.Default.Send(new WindowClosedMessage(true));
        }
    }
}
