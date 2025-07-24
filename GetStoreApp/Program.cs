using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.History;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.WindowsAPI.PInvoke.KernelBase;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Diagnostics;
using Windows.System;
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
        public static StrategyBasedComWrappers StrategyBasedComWrappers { get; } = new();

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();
            Ole32Library.CoInitializeSecurity(0, -1, 0, 0, 0, 3, 0, 32, 0);

            if (!RuntimeHelper.IsMSIX)
            {
                Launcher.LaunchUriAsync(new Uri("getstoreapp:")).Wait();
                return;
            }

            // 初始化应用启动参数
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppActivationArguments appActivationArguments = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            InitializeResourcesAsync().Wait();
            DownloadSchedulerService.InitializeDownloadScheduler();
            DesktopLaunchService.InitializeLaunchAsync(appActivationArguments).Wait();

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
                    if (InfoHelper.IsWindows11_24H1OrGreater)
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
                    else
                    {
                        if (KernelBaseLibrary.TryCreatePackageDependency(nint.Zero, dependencyPacakge.Id.FamilyName, new Windows.ApplicationModel.PackageVersion(), packageDependencyProcessorArchitectures, PackageDependencyLifetimeArtifactKind.Process, string.Empty, WindowsAPI.PInvoke.KernelBase.CreatePackageDependencyOptions.CreatePackageDependencyOptions_None, out string packageDependencyId) is 0)
                        {
                            if (KernelBaseLibrary.AddPackageDependency(packageDependencyId, 0, WindowsAPI.PInvoke.KernelBase.AddPackageDependencyOptions.AddPackageDependencyOptions_PrependIfRankCollision, out _, out _) is 0)
                            {
                                break;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }

            // 启动桌面程序
            Application.Start((param) =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()));
                new WinUIApp();
            });
        }

        /// <summary>
        /// 处理控制台应用程序未知异常处理
        /// </summary>
        private static void OnUnhandledException(object sender, System.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(LoggingLevel.Warning, nameof(GetStoreApp), nameof(Program), nameof(OnUnhandledException), 1, args.ExceptionObject as Exception);
            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeResourcesAsync()
        {
            // 初始化应用资源，应用使用的语言信息和启动参数
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            ResultService.Initialize();
            StoreRegionService.InitializeStoreRegion();
            LinkFilterService.InitializeLinkFilter();
            QueryLinksModeService.InitializeQueryLinksMode();
            await DownloadOptionsService.InitializeDownloadOptionsAsync();
            DownloadStorageService.Initialize();
            AppInstallService.InitializeAppInstall();

            // 初始化存储数据信息
            HistoryStorageService.Initialize();

            // 初始化应用配置信息
            InstallModeService.InitializeInstallMode();

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
            TopMostService.InitializeTopMost();

            CancelAutoUpdateService.InitializeCancelAutoUpdate();
            WebKernelService.InitializeWebKernel();
            ShellMenuService.InitializeShellMenu();
            NotificationService.InitializeNotification();
            await WinGetConfigService.InitializeWinGetConfigAsync();
        }
    }
}
