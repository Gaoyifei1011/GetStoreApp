using GetStoreAppInstaller.Helpers.Root;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.Services.Settings;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
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
        internal static string FileName { get; private set; }

        internal static StrategyBasedComWrappers StrategyBasedComWrappers { get; } = new();

        [STAThread, DynamicWindowsRuntimeCast(typeof(FileActivatedEventArgs)), DynamicWindowsRuntimeCast(typeof(FrameworkElement)), DynamicWindowsRuntimeCast(typeof(ShareTargetActivatedEventArgs))]
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
            AppActivationArguments appActivationArguments = AppInstance.GetCurrent().GetActivatedEventArgs();

            if (appActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                if (RuntimeHelper.IsElevated)
                {
                    string[] argumentsArray = Environment.GetCommandLineArgs();
                    string executableFileName = Path.GetFileName(Environment.ProcessPath);

                    if (argumentsArray.Length > 0 && string.Equals(Path.GetExtension(argumentsArray[0]), ".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        argumentsArray[0] = argumentsArray[0].Replace(".dll", ".exe");
                    }

                    if (argumentsArray.Length >= 2 && argumentsArray[0].Contains(executableFileName))
                    {
                        FileName = argumentsArray[1];
                    }
                }
            }
            // 从文件处启动
            else if (appActivationArguments.Kind is ExtendedActivationKind.File)
            {
                FileActivatedEventArgs fileActivatedEventArgs = appActivationArguments.Data as FileActivatedEventArgs;

                // 只解析读取到的第一个文件
                if (fileActivatedEventArgs.Files.Count > 0)
                {
                    FileName = fileActivatedEventArgs.Files[0].Path;
                }
            }
            // 从共享目标处启动
            else if (appActivationArguments.Kind is ExtendedActivationKind.ShareTarget)
            {
                ShareTargetActivatedEventArgs shareTargetActivatedEventArgs = appActivationArguments.Data as ShareTargetActivatedEventArgs;
                ShareOperation shareOperation = shareTargetActivatedEventArgs.ShareOperation;
                shareOperation.ReportCompleted();

                if (shareOperation.Data.Contains(StandardDataFormats.StorageItems) && shareOperation.Data.GetStorageItemsAsync().GetResults() is IReadOnlyList<IStorageItem> sharedFilesList && sharedFilesList.Count > 0)
                {
                    FileName = sharedFilesList[0].Path;
                }
            }
            else if (appActivationArguments.Kind is ExtendedActivationKind.ToastNotification)
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
