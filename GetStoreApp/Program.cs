using GetStoreApp.Helpers.Root;
using GetStoreApp.Properties;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using WinRT;

namespace GetStoreApp
{
    /// <summary>
    /// 获取商店应用 桌面程序
    /// </summary>
    public class Program
    {
        private static bool IsDesktopProgram { get; set; } = true;

        public static bool IsNeedAppLaunch { get; set; } = true;

        // 应用程序实例
        public static App ApplicationRoot { get; private set; }

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Resources.Culture = CultureInfo.CurrentCulture.Parent;
            if (!RuntimeHelper.IsMSIX)
            {
                return;
            }

            IsDesktopProgram = GetAppExecuteMode(args);

            InitializeProgramResourcesAsync().Wait();

            // 以桌面应用程序方式正常启动
            if (IsDesktopProgram)
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
                        ApplicationRoot = new App();
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

        /// <summary>
        /// 检查命令参数是否以桌面方式启动
        /// </summary>
        private static bool GetAppExecuteMode(string[] args)
        {
            return args.Length is 0 || !args[0].Equals("Console", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeProgramResourcesAsync()
        {
            // 初始化应用资源，应用使用的语言信息和启动参数
            await LogService.InitializeAsync();
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);

            // 初始化通用设置选项（桌面应用程序和控制台应用程序）
            ResourceService.LocalizeReosurce();
            LinkFilterService.InitializeLinkFilterValue();
            await DownloadOptionsService.InitializeAsync();

            if (IsDesktopProgram)
            {
                // 初始化存储数据信息
                await XmlStorageService.InitializeXmlFileAsync();
                await DownloadXmlService.InitializeDownloadXmlAsync();

                // 初始化应用配置信息
                InstallModeService.InitializeInstallMode();

                AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
                BackdropService.InitializeBackdrop();
                ThemeService.InitializeTheme();
                TopMostService.InitializeTopMostValue();

                HistoryRecordService.InitializeHistoryRecord();
                NotificationService.InitializeNotification();
                WinGetConfigService.InitializeWinGetConfig();

                // 实验功能设置配置
                NetWorkMonitorService.InitializeNetWorkMonitorValue();
            }
        }
    }
}
