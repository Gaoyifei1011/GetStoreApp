using GetStoreAppInstaller.Helpers.Root;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.Services.Settings;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using Windows.Foundation.Diagnostics;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 应用安装器
    /// </summary>
    public class Program
    {
        public static AppActivationArguments AppActivationArguments { get; private set; }

        public static StrategyBasedComWrappers StrategyBasedComWrappers { get; } = new();

        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();

            if (!RuntimeHelper.IsMSIX)
            {
                Shell32Library.ShellExecute(nint.Zero, "open", "GetStoreAppInstaller.exe", null, null, WindowShowStyle.SW_SHOWNORMAL);
                return;
            }

            // 初始化应用启动参数
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppActivationArguments = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

            if (AppActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                if (RuntimeHelper.IsElevated)
                {
                    string[] argumentsArray = Environment.GetCommandLineArgs();

                    if (argumentsArray.Length > 0 && string.Equals(Path.GetExtension(argumentsArray[0]), ".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        argumentsArray[0] = argumentsArray[0].Replace(".dll", ".exe");
                    }
                }
            }
            else if (AppActivationArguments.Kind is ExtendedActivationKind.ToastNotification)
            {
                return;
            }

            InitializeResources();

            // 启动桌面程序
            Application.Start((param) =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()));
                new InstallerApp();
            });
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private static void OnUnhandledException(object sender, System.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(LoggingLevel.Warning, nameof(GetStoreAppInstaller), nameof(Program), nameof(OnUnhandledException), 1, args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeResources()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
            AppInstallService.InitializeAppInstall();
        }
    }
}
