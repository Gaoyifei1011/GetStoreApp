using GetStoreApp.Helpers.Root;
using GetStoreApp.Properties;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WinRT;

namespace GetStoreApp
{
    /// <summary>
    /// 获取商店应用
    /// </summary>
    public class Program
    {
        private static bool IsDesktopProgram = true;

        public static bool IsNeedAppLaunch { get; set; } = true;

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Resources.Culture = CultureInfo.CurrentCulture.Parent;
            if (!RuntimeHelper.IsMSIX)
            {
                Kernel32Library.GetStartupInfo(out STARTUPINFO getStoreAppStartupInfo);
                getStoreAppStartupInfo.lpReserved = IntPtr.Zero;
                getStoreAppStartupInfo.lpDesktop = IntPtr.Zero;
                getStoreAppStartupInfo.lpTitle = IntPtr.Zero;
                getStoreAppStartupInfo.dwX = 0;
                getStoreAppStartupInfo.dwY = 0;
                getStoreAppStartupInfo.dwXSize = 0;
                getStoreAppStartupInfo.dwYSize = 0;
                getStoreAppStartupInfo.dwXCountChars = 500;
                getStoreAppStartupInfo.dwYCountChars = 500;
                getStoreAppStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                getStoreAppStartupInfo.wShowWindow = WindowShowStyle.SW_SHOWNORMAL;
                getStoreAppStartupInfo.cbReserved2 = 0;
                getStoreAppStartupInfo.lpReserved2 = IntPtr.Zero;
                getStoreAppStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                bool createResult = Kernel32Library.CreateProcess(null, "explorer.exe shell:AppsFolder\\Gaoyifei1011.GetStoreApp_pystbwmrmew8c!GetStoreApp", IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.None, IntPtr.Zero, null, ref getStoreAppStartupInfo, out PROCESS_INFORMATION getStoreAppInformation);

                if (createResult)
                {
                    if (getStoreAppInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(getStoreAppInformation.hProcess);
                    if (getStoreAppInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(getStoreAppInformation.hThread);
                }
                return;
            }

            IsDesktopProgram = GetAppExecuteMode(args);

            InitializeResourcesAsync().Wait();

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
                        WinUIApp app = new WinUIApp();
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
        /// 检查应用启动方式
        /// </summary>
        private static bool GetAppExecuteMode(string[] args)
        {
            return args.Length is 0 || !args[0].Equals("Console", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeResourcesAsync()
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
                HistoryService.Initialize();
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
