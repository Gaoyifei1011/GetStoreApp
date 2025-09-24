using GetStoreAppInstaller.Helpers.Root;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.Services.Settings;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Foundation.Diagnostics;
using Windows.System;
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

        public static AppWindow MainAppWindow { get; private set; }

        public static AppWindow CoreAppWindow { get; set; }

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

            // 使用 MSIX 动态依赖包 API，强行修改静态包图的依赖顺序，解决 WinUI 3 桌面应用程序加载时错误加载成 WinUI 2 程序集，导致程序启动失败的问题
            IReadOnlyList<Package> dependencyPackageList = Package.Current.Dependencies;
            PackageDependencyProcessorArchitectures packageDependencyProcessorArchitectures = PackageDependencyProcessorArchitectures.None;

            switch (Package.Current.Id.Architecture)
            {
                case ProcessorArchitecture.X86:
                    {
                        packageDependencyProcessorArchitectures = PackageDependencyProcessorArchitectures.X86;
                        break;
                    }
                case ProcessorArchitecture.X64:
                    {
                        packageDependencyProcessorArchitectures = PackageDependencyProcessorArchitectures.X64;
                        break;
                    }
                case ProcessorArchitecture.Arm64:
                    {
                        packageDependencyProcessorArchitectures = PackageDependencyProcessorArchitectures.Arm64;
                        break;
                    }
                case ProcessorArchitecture.X86OnArm64:
                    {
                        packageDependencyProcessorArchitectures = PackageDependencyProcessorArchitectures.X86OnArm64;
                        break;
                    }
                case ProcessorArchitecture.Neutral:
                    {
                        packageDependencyProcessorArchitectures = PackageDependencyProcessorArchitectures.Neutral;
                        break;
                    }
                case ProcessorArchitecture.Unknown:
                    {
                        packageDependencyProcessorArchitectures = PackageDependencyProcessorArchitectures.None;
                        break;
                    }
            }

            foreach (Package dependencyPacakge in dependencyPackageList)
            {
                // 系统版本大于 Windows 11（24H1，26100），使用 Windows App SDK 包装好的 API。不是则直接调用系统的 API
                if (dependencyPacakge.DisplayName.Contains("WindowsAppRuntime"))
                {
                    PackageDependency packageDependency = PackageDependency.Create(dependencyPacakge.Id.FamilyName, new Windows.ApplicationModel.PackageVersion(), new Microsoft.Windows.ApplicationModel.DynamicDependency.CreatePackageDependencyOptions()
                    {
                        Architectures = packageDependencyProcessorArchitectures,
                        LifetimeArtifact = string.Empty,
                        LifetimeArtifactKind = PackageDependencyLifetimeArtifactKind.Process,
                        VerifyDependencyResolution = false
                    });

                    if (packageDependency is not null)
                    {
                        PackageDependencyContext packageDependencyContext = packageDependency.Add(new Microsoft.Windows.ApplicationModel.DynamicDependency.AddPackageDependencyOptions()
                        {
                            Rank = 0,
                            PrependIfRankCollision = true,
                        });

                        if (packageDependencyContext is null)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            // 启动桌面程序
            Application.Start((param) =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()));
                new InstallerApp();
            });
        }

        /// <summary>
        /// 处理控制台应用程序未知异常处理
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
            ResourceService.LocalizeReosurce();

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
            AppInstallService.InitializeAppInstall();
        }
    }
}
