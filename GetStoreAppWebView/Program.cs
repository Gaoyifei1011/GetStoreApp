using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Services.Settings;
using System;
using System.IO;
using System.Threading;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
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
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [MTAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();

            if (!RuntimeHelper.IsMSIX)
            {
                Launcher.LaunchUriAsync(new Uri("getstoreapp:")).Wait();
                return;
            }

            InitializeResources();

            if (string.Equals(WebKernelService.WebKernel, WebKernelService.WebKernelList[1]))
            {
                Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");

                string systemWebView2Path = Path.Combine(SystemDataPaths.GetDefault().System, "msedgewebview2.exe");

                if (Directory.Exists(systemWebView2Path) && File.Exists(Path.Combine(systemWebView2Path, "WebView2.exe")))
                {
                    Environment.SetEnvironmentVariable("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER", Path.Combine(SystemDataPaths.GetDefault().System, "Microsoft-Edge-WebView"));
                }
            }

            Application.Start((param) =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                new App();
            });
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeResources()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            ResourceService.LocalizeReosurce();

            AppLaunchService.Initialize();
            ThemeService.InitializeTheme();
            WebKernelService.InitializeWebKernel();
        }
    }
}
