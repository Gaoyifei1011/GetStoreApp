using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WinUIEx;

namespace GetStoreApp
{
    public partial class App : Application
    {
        private IActivationService ActivationService { get; }

        public IAria2Service Aria2Service { get; }

        public static WindowEx MainWindow { get; } = new MainWindow();

        public App()
        {
            InitializeComponent();

            IOCHelper.InitializeIOCService();

            ActivationService = IOCHelper.GetService<IActivationService>();
            Aria2Service = IOCHelper.GetService<IAria2Service>();

            UnhandledException += OnUnhandledException;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            await RunSingleInstanceApp();

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
        private async Task RunSingleInstanceApp()
        {
            // Get the activation args
            var appArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

            // Get or register the main instance
            var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");

            // If the main instance isn't this current instance
            if (!mainInstance.IsCurrent)
            {
                // Redirect activation to that instance
                await mainInstance.RedirectActivationToAsync(appArgs);

                // And exit our instance and stop
                Process.GetCurrentProcess().Kill();
                return;
            }
        }
    }
}
