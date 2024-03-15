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
using Windows.Management.Deployment;
using WinRT;

namespace GetStoreApp
{
    /// <summary>
    /// 获取商店应用
    /// </summary>
    public class Program
    {
        public static bool IsNeedAppLaunch { get; set; } = true;

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (!RuntimeHelper.IsMSIX)
            {
                PackageManager packageManager = new PackageManager();
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

            // Win32 基于 HWND 的传统桌面应用 / 控制台应用
            if (RuntimeHelper.AppWindowingModel is AppPolicyWindowingModel.AppPolicyWindowingModel_ClassicDesktop)
            {
                bool isDesktopProgram = args.Length is 0 || !args[0].Equals("Console", StringComparison.OrdinalIgnoreCase);

                InitializeResourcesAsync(isDesktopProgram).Wait();

                // 以桌面应用程序方式正常启动
                if (isDesktopProgram)
                {
                    DesktopLaunchService.InitializeLaunchAsync(args).Wait();

                    // 启动桌面程序
                    if (IsNeedAppLaunch)
                    {
                        ComWrappersSupport.InitializeComWrappers();

                        Application.Start((param) =>
                        {
                            DispatcherQueueSynchronizationContext context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                            SynchronizationContext.SetSynchronizationContext(context);
                            new WinUIApp();
                        });
                    }
                }
                // 以控制台程序方式启动
                else
                {
                    bool AttachResult = Kernel32Library.AttachConsole();

                    if (!AttachResult)
                    {
                        Kernel32Library.AllocConsole();
                    }
                    ConsoleLaunchService.InitializeLaunchAsync(args).Wait();
                    Kernel32Library.FreeConsole();
                }
            }
            // UWP CoreApplication 应用
            else if (RuntimeHelper.AppWindowingModel is AppPolicyWindowingModel.AppPolicyWindowingModel_Universal)
            {
                // WinUI 3 UWP 模式下，必须需要使用多线程公寓模型启动，所以必须卸载 STAThreadAttribute 属性默认初始化的单线程公寓模型，然后将其初始化为多线程公寓模型
                Ole32Library.CoUninitialize();
                Ole32Library.CoInitializeEx(IntPtr.Zero, COINIT.COINIT_MULTITHREADED);
                InitializeResourcesAsync(false).Wait();
                CoreApplication.Run(new Views.Windows.FrameworkViewSource());
                Ole32Library.CoUninitialize();
            }
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeResourcesAsync(bool isDesktopProgram)
        {
            // 初始化应用资源，应用使用的语言信息和启动参数
            await LogService.InitializeAsync();
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            ResultService.Initialize();

            // 初始化通用设置选项（桌面应用程序和控制台应用程序）
            if (RuntimeHelper.AppWindowingModel is AppPolicyWindowingModel.AppPolicyWindowingModel_ClassicDesktop)
            {
                ResourceService.LocalizeReosurce();
                LinkFilterService.InitializeLinkFilterValue();
                await DownloadOptionsService.InitializeAsync();
            }

            // 初始化其他设置信息（桌面应用程序）
            if (isDesktopProgram)
            {
                // 初始化存储数据信息
                await XmlStorageService.InitializeXmlFileAsync();
                HistoryStorageService.Initialize();
                await DownloadXmlService.InitializeDownloadXmlAsync();

                // 初始化应用配置信息
                InstallModeService.InitializeInstallMode();

                AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
                BackdropService.InitializeBackdrop();
                ThemeService.InitializeTheme();
                TopMostService.InitializeTopMostValue();

                WebKernelService.InitializeWebKernel();
                NotificationService.InitializeNotification();
                WinGetConfigService.InitializeWinGetConfig();

                // 实验功能设置配置
                NetWorkMonitorService.InitializeNetWorkMonitorValue();
            }
        }
    }
}
