using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
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
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            ComWrappersSupport.InitializeComWrappers();
            Ole32Library.CoInitializeSecurity(0, -1, 0, 0, 0, 3, 0, 0x20, 0);

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

            bool isDesktopProgram = args.Length is 0 || !args[0].Equals("Console", StringComparison.OrdinalIgnoreCase);
            InitializeResourcesAsync(isDesktopProgram).Wait();

            // 以桌面应用程序方式正常启动
            if (isDesktopProgram)
            {
                DownloadSchedulerService.InitializeDownloadScheduler(true);
                DesktopLaunchService.InitializeLaunchAsync(args).Wait();

                // 启动桌面程序
                Application.Start((param) =>
                {
                    SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                    _ = new WinUIApp();
                });
            }
            // 以控制台程序方式启动
            else
            {
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                bool AttachResult = Kernel32Library.AttachConsole();

                if (!AttachResult)
                {
                    Kernel32Library.AllocConsole();
                }
                ConsoleLaunchService.InitializeLaunchAsync(args).Wait();
                Kernel32Library.FreeConsole();
            }
        }

        /// <summary>
        /// 处理控制台应用程序未知异常处理
        /// </summary>
        private static void OnUnhandledException(object sender, System.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(LoggingLevel.Error, "Unknown unhandled exception.", args.ExceptionObject as Exception);
            Environment.Exit(0);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeResourcesAsync(bool isDesktopProgram)
        {
            // 初始化应用资源，应用使用的语言信息和启动参数
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            ResultService.Initialize();
            ResourceService.LocalizeReosurce();
            StoreRegionService.InitializeStoreRegion();
            LinkFilterService.InitializeLinkFilterValue();
            QueryLinksModeService.InitializeQueryLinksMode();
            await DownloadOptionsService.InitializeDownloadAsync();
            DownloadStorageService.Initialize();

            // 初始化其他设置信息（桌面应用程序）
            if (isDesktopProgram)
            {
                // 初始化存储数据信息
                HistoryStorageService.Initialize();

                // 初始化应用配置信息
                InstallModeService.InitializeInstallMode();

                AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
                BackdropService.InitializeBackdrop();
                ThemeService.InitializeTheme();
                TopMostService.InitializeTopMostValue();

                WebKernelService.InitializeWebKernel();
                ShellMenuService.InitializeShellMenu();
                NotificationService.InitializeNotification();
                WinGetConfigService.InitializeWinGetConfig();
            }
        }
    }
}
