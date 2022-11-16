using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Views.Window;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp
{
    public partial class App : Application
    {
        public static AppWindow AppWindow { get; set; }

        public static WASDKWindow MainWindow { get; set; }

        // 标志内容对话框是否处于正在打开状态。若是，则不再打开其他内容对话框，防止造成应用异常
        public static bool IsDialogOpening { get; set; } = false;

        // 导航页面后使用到的参数
        public static AppNaviagtionArgs NavigationArgs { get; set; } = AppNaviagtionArgs.None;

        public App()
        {
            InitializeComponent();
            UnhandledException += OnUnhandledException;

            ContainerHelper.InitializeContainer();
            ContainerHelper.GetInstance<IAppNotificationService>().Initialize();
        }

        /// <summary>
        /// 处理应用启动
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            await ContainerHelper.GetInstance<IActivationService>().ActivateAsync(args);
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private async void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            await ContainerHelper.GetInstance<IDownloadSchedulerService>().CloseDownloadSchedulerAsync();
            await ContainerHelper.GetInstance<IAria2Service>().CloseAria2Async();
            WeakReferenceMessenger.Default.Send(new WindowClosedMessage(true));
        }
    }
}
