using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.History;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Diagnostics;
using Windows.System;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreApp
{
    /// <summary>
    /// 获取商店应用
    /// </summary>
    public class Program
    {
        public static Microsoft.Windows.AppLifecycle.AppInstance AppInstance { get; private set; }

        public static StrategyBasedComWrappers StrategyBasedComWrappers { get; } = new();

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();
            Ole32Library.CoInitializeSecurity(0, -1, 0, 0, 0, 3, 0, 32, 0);

            if (!RuntimeHelper.IsMSIX)
            {
                Launcher.LaunchUriAsync(new Uri("getstoreapp:")).Wait();
                return;
            }

            // 初始化应用启动参数
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey(nameof(GetStoreApp));
            AppActivationArguments appActivationArguments = AppInstance.GetActivatedEventArgs();

            // 检查是否是当前实例应用
            if (AppInstance.IsCurrent)
            {
                AppInstance.Activated += OnActivated;
            }
            else
            {
                try
                {
                    AppInstance.RedirectActivationToAsync(appActivationArguments).Wait();
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }

                return;
            }

            InitializeResourcesAsync().Wait();
            DownloadSchedulerService.InitializeDownloadScheduler();
            DesktopLaunchService.InitializeLaunchAsync(appActivationArguments, false).Wait();

            // 启动桌面程序
            Application.Start((param) =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()));
                new MainApp();
            });
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private static void OnUnhandledException(object sender, System.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(LoggingLevel.Warning, nameof(GetStoreApp), nameof(Program), nameof(OnUnhandledException), 1, args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 重定向激活后触发的事件
        /// </summary>
        private static async void OnActivated(object sender, AppActivationArguments args)
        {
            // 若应用程序已启动，将启动信息重定向到已启动的应用中
            await DesktopLaunchService.InitializeLaunchAsync(args, true);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeResourcesAsync()
        {
            // 初始化应用资源，应用使用的语言信息和启动参数
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            StoreRegionService.InitializeStoreRegion();
            LinkFilterService.InitializeLinkFilter();
            QueryLinksModeService.InitializeQueryLinksMode();
            SearchAppsModeService.InitializeSearchAppsMode();
            AppLinkOpenModeService.InitializeAppLinkOpenMode();
            await DownloadOptionsService.InitializeDownloadOptionsAsync();
            DownloadStorageService.Initialize();
            AppInstallService.InitializeAppInstall();

            // 初始化存储数据信息
            HistoryStorageService.Initialize();

            // 初始化应用配置信息
            InstallModeService.InitializeInstallMode();

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
            TopMostService.InitializeTopMost();

            CancelAutoUpdateService.InitializeCancelAutoUpdate();
            ShellMenuService.InitializeShellMenu();
            NotificationService.InitializeNotification();
            await WinGetConfigService.InitializeWinGetConfigAsync();
        }
    }
}
