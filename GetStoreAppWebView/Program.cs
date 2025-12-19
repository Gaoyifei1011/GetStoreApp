using GetStoreAppWebView.Extensions.DataType.Enums;
using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Services.Settings;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Diagnostics;
using Windows.System;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreAppWebView
{
    /// <summary>
    /// 网页浏览器
    /// </summary>
    public class Program
    {
        public static AppActivationArguments AppActivationArguments { get; private set; }

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();

            if (!RuntimeHelper.IsMSIX)
            {
                Launcher.LaunchUriAsync(new Uri("getstoreappwebview:")).Wait();
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppActivationArguments = AppInstance.GetCurrent().GetActivatedEventArgs();

            if (AppActivationArguments.Kind is ExtendedActivationKind.Protocol && RuntimeHelper.WebView2Type is WebView2Type.None)
            {
                ProtocolActivatedEventArgs protocolActivatedEventArgs = AppActivationArguments.Data.As<ProtocolActivatedEventArgs>();
                if (protocolActivatedEventArgs.Data.TryGetValue("AppLink", out object appLinkObj) && appLinkObj is string appLink && !string.IsNullOrEmpty(appLink))
                {
                    Launcher.LaunchUriAsync(new Uri(appLink)).Wait();
                }
                else
                {
                    Launcher.LaunchUriAsync(new Uri("https://apps.microsoft.com")).Wait();
                }
                return;
            }

            InitializeResourcesAsync().Wait();

            Application.Start((param) =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()));
                new WebViewApp();
            });
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private static void OnUnhandledException(object sender, System.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(LoggingLevel.Warning, nameof(GetStoreAppWebView), nameof(Program), nameof(OnUnhandledException), 1, args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeResourcesAsync()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            await DownloadOptionsService.InitializeDownloadOptionsAsync();

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
        }
    }
}
