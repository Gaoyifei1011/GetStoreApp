using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.Storage;
using WinRT;

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
        [STAThread]
        public static void Main()
        {
            if (!RuntimeHelper.IsMSIX)
            {
                PackageManager packageManager = new();
                foreach (Package package in packageManager.FindPackagesForUser(string.Empty))
                {
                    if (package.Id.FullName.Contains("Gaoyifei1011.GetStoreApp"))
                    {
                        IReadOnlyList<AppListEntry> appListEntryList = package.GetAppListEntries();
                        foreach (AppListEntry appListEntry in appListEntryList)
                        {
                            if (appListEntry.AppUserModelId.Equals("Gaoyifei1011.GetStoreApp_pystbwmrmew8c!GetStoreApp"))
                            {
                                appListEntry.LaunchAsync().AsTask().Wait();
                                return;
                            }
                        }
                    }
                }
                return;
            }

            InitializeResources();

            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[1])
            {
                Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");

                string systemWebView2Path = Path.Combine(SystemDataPaths.GetDefault().System, "Microsoft-Edge-WebView");

                if (Directory.Exists(systemWebView2Path))
                {
                    Environment.SetEnvironmentVariable("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER", Path.Combine(SystemDataPaths.GetDefault().System, "Microsoft-Edge-WebView"));
                }
            }
            else
            {
                if (RuntimeHelper.IsElevated)
                {
                    global::Windows.System.Launcher.LaunchUriAsync(new Uri("https://store.rg-adguard.net")).AsTask().Wait();
                    return;
                }
            }

            ComWrappersSupport.InitializeComWrappers();

            Application.Start((param) =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                _ = new WebApp();
            });
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeResources()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            WebKernelService.InitializeWebKernel();
        }
    }
}
