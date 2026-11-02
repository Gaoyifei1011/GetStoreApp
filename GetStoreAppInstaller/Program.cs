using GetStoreAppInstaller.Helpers.Root;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.Services.Settings;
using GetStoreAppInstaller.Views.Windows;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using Microsoft.Windows.AppLifecycle;
using System;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Diagnostics;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 应用包安装器
    /// </summary>
    internal class Program
    {
        internal static AppActivationArguments AppActivationArguments { get; private set; }

        /// <summary>
        /// 从文件激活处启动时携带的文件路径
        /// </summary>
        internal static string InitialFileName { get; private set; }

        internal static StrategyBasedComWrappers StrategyBasedComWrappers { get; } = new();

        [STAThread]
        [DynamicWindowsRuntimeCast(typeof(FileActivatedEventArgs))]
        internal static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();

            if (!RuntimeHelper.IsMSIX)
            {
                Shell32Library.ShellExecute(nint.Zero, "open", "GetStoreAppInstaller.exe", null, null, SHOWWINDOWCMD.SW_SHOWNORMAL);
                return;
            }

            // 初始化应用启动参数
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppActivationArguments = AppInstance.GetCurrent().GetActivatedEventArgs();

            // 文件激活参数必须在 XAML 启动前读取，在窗口加载阶段读取会重入尚未完成的激活调用并导致进程 fail-fast
            if (AppActivationArguments.Kind is ExtendedActivationKind.File)
            {
                FileActivatedEventArgs fileActivatedEventArgs = AppActivationArguments.Data as FileActivatedEventArgs;

                // 只解析读取到的第一个文件
                if (fileActivatedEventArgs.Files.Count > 0)
                {
                    InitialFileName = fileActivatedEventArgs.Files[0].Path;
                }
            }

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
            XamlGeneratedProgram.XamlGeneratedMain();
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

            WindowPlacementService.Initialize();
            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
            AppInstallService.InitializeAppInstall();
        }
    }
}
