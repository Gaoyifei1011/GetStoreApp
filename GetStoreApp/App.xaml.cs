using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Window;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

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
            
            AppNotificationService.Initialize();
        }

        /// <summary>
        /// 处理应用启动
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            await ActivationService.ActivateAsync(args);
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            Messenger.Default.Send(true, MessageToken.WindowClosed);
        }
    }
}
