using Autofac;
using GetStoreApp.Contracts.Controls.Download;
using GetStoreApp.Contracts.Controls.History;
using GetStoreApp.Contracts.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Controls.Settings.Common;
using GetStoreApp.Contracts.Controls.Settings.Experiment;
using GetStoreApp.Contracts.Root;
using GetStoreApp.Contracts.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Controls.About;
using GetStoreApp.UI.Controls.Download;
using GetStoreApp.UI.Controls.Home;
using GetStoreApp.UI.Controls.Settings.Advanced;
using GetStoreApp.UI.Controls.Settings.Appearance;
using GetStoreApp.UI.Controls.Settings.Common;
using GetStoreApp.UI.Controls.Settings.Experiment;
using GetStoreApp.UI.Controls.Web;
using GetStoreApp.UI.Controls.Window;
using GetStoreApp.UI.Dialogs.ContentDialogs.About;
using GetStoreApp.UI.Dialogs.ContentDialogs.Download;
using GetStoreApp.UI.Dialogs.ContentDialogs.Settings;
using GetStoreApp.UI.Dialogs.ContentDialogs.Web;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Controls.About;
using GetStoreApp.ViewModels.Controls.Download;
using GetStoreApp.ViewModels.Controls.Home;
using GetStoreApp.ViewModels.Controls.Settings.Advanced;
using GetStoreApp.ViewModels.Controls.Settings.Appearance;
using GetStoreApp.ViewModels.Controls.Settings.Common;
using GetStoreApp.ViewModels.Controls.Settings.Experiment;
using GetStoreApp.ViewModels.Controls.Web;
using GetStoreApp.ViewModels.Controls.Window;
using GetStoreApp.ViewModels.Dialogs.About;
using GetStoreApp.ViewModels.Dialogs.Download;
using GetStoreApp.ViewModels.Dialogs.Settings;
using GetStoreApp.ViewModels.Dialogs.Web;
using GetStoreApp.ViewModels.Notifications;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.ViewModels.Window;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Window;
using GetStoreAppWindowsAPI.PInvoke.User32;
using System;
using WinRT.Interop;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 控制翻转/依赖注入
    /// </summary>
    public static class ContainerHelper
    {
        private static IContainer Container { get; set; }

        /// <summary>
        /// 获取已经在IOC容器注册类的实例
        /// </summary>
        public static T GetInstance<T>() where T : class
        {
            if (!Container.IsRegistered<T>())
            {
                MessageBoxResult Result = User32Library.MessageBox(
                    WindowNative.GetWindowHandle(App.MainWindow),
                    $"应用启动失败。\n{typeof(T)} 需要在ContainerHelper.cs中的InitializeContainer()方法中注册。",
                    "获取商店应用",
                    MessageBoxOptions.MB_OK | MessageBoxOptions.MB_ICONERROR | MessageBoxOptions.MB_APPLMODAL | MessageBoxOptions.MB_TOPMOST
                    );

                if (Result == MessageBoxResult.IDOK)
                {
                    Environment.Exit(Convert.ToInt32(AppExitCode.Failed));
                }
            }

            return Container.Resolve<T>();
        }

        /// <summary>
        /// 初始化IOC容器
        /// </summary>
        public static void InitializeContainer()
        {
            ContainerBuilder Builder = new ContainerBuilder();

            // 应用服务
            Builder.RegisterType<ActivationService>().As<IActivationService>().SingleInstance();
            Builder.RegisterType<AppNotificationService>().As<IAppNotificationService>().SingleInstance();
            Builder.RegisterType<DataBaseService>().As<IDataBaseService>().SingleInstance();
            Builder.RegisterType<ResourceService>().As<IResourceService>().SingleInstance();
            Builder.RegisterType<StartupService>().As<IStartupService>().SingleInstance();

            Builder.RegisterType<Aria2Service>().As<IAria2Service>().SingleInstance();
            Builder.RegisterType<DownloadDBService>().As<IDownloadDBService>().SingleInstance();
            Builder.RegisterType<DownloadSchedulerService>().As<IDownloadSchedulerService>().SingleInstance();

            Builder.RegisterType<HistoryDBService>().As<IHistoryDBService>().SingleInstance();

            Builder.RegisterType<AppExitService>().As<IAppExitService>().SingleInstance();
            Builder.RegisterType<InstallModeService>().As<IInstallModeService>().SingleInstance();
            Builder.RegisterType<TraceCleanupService>().As<ITraceCleanupService>().SingleInstance();

            Builder.RegisterType<AlwaysShowBackdropService>().As<IAlwaysShowBackdropService>().SingleInstance();
            Builder.RegisterType<BackdropService>().As<IBackdropService>().SingleInstance();
            Builder.RegisterType<LanguageService>().As<ILanguageService>().SingleInstance();
            Builder.RegisterType<ThemeService>().As<IThemeService>().SingleInstance();
            Builder.RegisterType<TopMostService>().As<ITopMostService>().SingleInstance();

            Builder.RegisterType<DownloadOptionsService>().As<IDownloadOptionsService>().SingleInstance();
            Builder.RegisterType<HistoryLiteNumService>().As<IHistoryLiteNumService>().SingleInstance();
            Builder.RegisterType<LinkFilterService>().As<ILinkFilterService>().SingleInstance();
            Builder.RegisterType<NotificatonService>().As<INotificationService>().SingleInstance();
            Builder.RegisterType<RegionService>().As<IRegionService>().SingleInstance();
            Builder.RegisterType<UseInstructionService>().As<IUseInstructionService>().SingleInstance();

            Builder.RegisterType<NetWorkMonitorService>().As<INetWorkMonitorService>().SingleInstance();

            Builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();

            // 应用窗口（MVVM）
            Builder.RegisterType<MainWindow>().InstancePerDependency();
            Builder.RegisterType<MainWindowViewModel>().InstancePerDependency();

            // 页面（MVVM）
            Builder.RegisterType<AboutPage>().InstancePerDependency();
            Builder.RegisterType<AboutViewModel>().InstancePerDependency();
            Builder.RegisterType<DownloadPage>().InstancePerDependency();
            Builder.RegisterType<DownloadViewModel>().InstancePerDependency();
            Builder.RegisterType<HistoryPage>().InstancePerDependency();
            Builder.RegisterType<HistoryViewModel>().InstancePerDependency();
            Builder.RegisterType<HomePage>().InstancePerDependency();
            Builder.RegisterType<HomeViewModel>().InstancePerDependency();
            Builder.RegisterType<SettingsPage>().InstancePerDependency();
            Builder.RegisterType<SettingsViewModel>().InstancePerDependency();
            Builder.RegisterType<WebPage>().InstancePerDependency();
            Builder.RegisterType<WebViewModel>().InstancePerDependency();

            // 托盘控件（MVVM）
            Builder.RegisterType<TaskBarControl>().InstancePerDependency();
            Builder.RegisterType<TaskBarViewModel>().InstancePerDependency();

            // 关于页面控件（MVVM）
            Builder.RegisterType<HeaderControl>().InstancePerDependency();
            Builder.RegisterType<HeaderViewModel>().InstancePerDependency();
            Builder.RegisterType<InstructionsControl>().InstancePerDependency();
            Builder.RegisterType<InstructionsViewModel>().InstancePerDependency();
            Builder.RegisterType<IntroductionControl>().InstancePerDependency();
            Builder.RegisterType<IntroductionViewModel>().InstancePerDependency();
            Builder.RegisterType<PrecautionControl>().InstancePerDependency();
            Builder.RegisterType<PrecautionViewModel>().InstancePerDependency();
            Builder.RegisterType<ReferenceControl>().InstancePerDependency();
            Builder.RegisterType<ReferenceViewModel>().InstancePerDependency();
            Builder.RegisterType<SettingsHelpControl>().InstancePerDependency();
            Builder.RegisterType<SettingsHelpViewModel>().InstancePerDependency();
            Builder.RegisterType<ThanksControl>().InstancePerDependency();
            Builder.RegisterType<ThanksViewModel>().InstancePerDependency();

            // 下载页面控件（MVVM）
            Builder.RegisterType<DownloadingControl>().InstancePerDependency();
            Builder.RegisterType<DownloadingViewModel>().InstancePerDependency();
            Builder.RegisterType<CompletedControl>().InstancePerDependency();
            Builder.RegisterType<CompletedViewModel>().InstancePerDependency();
            Builder.RegisterType<UnfinishedControl>().InstancePerDependency();
            Builder.RegisterType<UnfinishedViewModel>().InstancePerDependency();

            // 主页面控件（MVVM）
            Builder.RegisterType<HistoryLiteControl>().InstancePerDependency();
            Builder.RegisterType<HistoryLiteViewModel>().InstancePerDependency();
            Builder.RegisterType<RequestControl>().InstancePerDependency();
            Builder.RegisterType<RequestViewModel>().InstancePerDependency();
            Builder.RegisterType<ResultControl>().InstancePerDependency();
            Builder.RegisterType<ResultViewModel>().InstancePerDependency();
            Builder.RegisterType<StatusBarControl>().InstancePerDependency();
            Builder.RegisterType<StatusBarViewModel>().InstancePerDependency();

            // 设置页面控件（MVVM）
            // 高级选项
            Builder.RegisterType<AppExitControl>().InstancePerDependency();
            Builder.RegisterType<AppExitViewModel>().InstancePerDependency();
            Builder.RegisterType<ExperimentalFeaturesControl>().InstancePerDependency();
            Builder.RegisterType<ExperimentalFeaturesViewModel>().InstancePerDependency();
            Builder.RegisterType<InstallModeControl>().InstancePerDependency();
            Builder.RegisterType<InstallModeViewModel>().InstancePerDependency();
            Builder.RegisterType<TraceCleanupControl>().InstancePerDependency();
            Builder.RegisterType<TraceCleanupViewModel>().InstancePerDependency();

            // 外观
            Builder.RegisterType<AlwaysShowBackdropControl>().InstancePerDependency();
            Builder.RegisterType<AlwaysShowBackdropViewModel>().InstancePerDependency();
            Builder.RegisterType<BackdropControl>().InstancePerDependency();
            Builder.RegisterType<BackdropViewModel>().InstancePerDependency();
            Builder.RegisterType<LauguageControl>().InstancePerDependency();
            Builder.RegisterType<LanguageViewModel>().InstancePerDependency();
            Builder.RegisterType<ThemeControl>().InstancePerDependency();
            Builder.RegisterType<ThemeViewModel>().InstancePerDependency();
            Builder.RegisterType<TopMostControl>().InstancePerDependency();
            Builder.RegisterType<TopMostViewModel>().InstancePerDependency();

            // 常规选项
            Builder.RegisterType<DownloadOptionsControl>().InstancePerDependency();
            Builder.RegisterType<DownloadOptionsViewModel>().InstancePerDependency();
            Builder.RegisterType<HistoryLiteConfigControl>().InstancePerDependency();
            Builder.RegisterType<HistoryLiteConfigViewModel>().InstancePerDependency();
            Builder.RegisterType<LinkFilterControl>().InstancePerDependency();
            Builder.RegisterType<LinkFilterViewModel>().InstancePerDependency();
            Builder.RegisterType<NotificationControl>().InstancePerDependency();
            Builder.RegisterType<NotificationViewModel>().InstancePerDependency();
            Builder.RegisterType<RegionControl>().InstancePerDependency();
            Builder.RegisterType<RegionViewModel>().InstancePerDependency();
            Builder.RegisterType<UseInstructionControl>().InstancePerDependency();
            Builder.RegisterType<UseInstructionViewModel>().InstancePerDependency();

            // 实验性功能选项
            Builder.RegisterType<NetWorkMonitorControl>().InstancePerDependency();
            Builder.RegisterType<NetWorkMonitorViewModel>().InstancePerDependency();
            Builder.RegisterType<OpenConfigFIleControl>().InstancePerDependency();
            Builder.RegisterType<OpenConfigFileViewModel>().InstancePerDependency();

            // 标题栏控件（MVVM）
            Builder.RegisterType<AppTitleBarControl>().InstancePerDependency();
            Builder.RegisterType<AppTitleBarViewModel>().InstancePerDependency();

            // 网页界面控件（MVVM）
            Builder.RegisterType<LoadFailedControl>().InstancePerDependency();
            Builder.RegisterType<LoadFailedViewModel>().InstancePerDependency();

            // 对话框（MVVM）
            Builder.RegisterType<CoreWebView2FailedDialog>().InstancePerDependency();
            Builder.RegisterType<CoreWebView2FailedViewModel>().InstancePerDependency();
            Builder.RegisterType<DesktopStartupArgsDialog>().InstancePerDependency();
            Builder.RegisterType<DesktopStartupArgsViewModel>().InstancePerDependency();
            Builder.RegisterType<ExperimentalConfigDialog>().InstancePerDependency();
            Builder.RegisterType<ExperimentalConfigViewModel>().InstancePerDependency();
            Builder.RegisterType<FileInformationDialog>().InstancePerDependency();
            Builder.RegisterType<FileInformationViewModel>().InstancePerDependency();
            Builder.RegisterType<LicenseDialog>().InstancePerDependency();
            Builder.RegisterType<LicenseViewModel>().InstancePerDependency();
            Builder.RegisterType<RestartAppsDialog>().InstancePerDependency();
            Builder.RegisterType<RestartAppsViewModel>().InstancePerDependency();
            Builder.RegisterType<TraceCleanupPromptDialog>().InstancePerDependency();
            Builder.RegisterType<TraceCleanupPromptViewModel>().InstancePerDependency();

            // 应用内消息通知框（MVVM）
            Builder.RegisterType<DownloadCreateNotification>().InstancePerDependency();
            Builder.RegisterType<DownloadCreateViewModel>().InstancePerDependency();
            Builder.RegisterType<ExceptionCopyNotification>().InstancePerDependency();
            Builder.RegisterType<ExceptionCopyViewModel>().InstancePerDependency();
            Builder.RegisterType<FileInformationCopyNotification>().InstancePerDependency();
            Builder.RegisterType<FileInformationCopyViewModel>().InstancePerDependency();
            Builder.RegisterType<HistoryCopyNotification>().InstancePerDependency();
            Builder.RegisterType<HistoryCopyViewModel>().InstancePerDependency();
            Builder.RegisterType<LanguageChangeNotification>().InstancePerDependency();
            Builder.RegisterType<LanguageChangeViewModel>().InstancePerDependency();
            Builder.RegisterType<ResultContentCopyNotification>().InstancePerDependency();
            Builder.RegisterType<ResultContentCopyViewModel>().InstancePerDependency();
            Builder.RegisterType<ResultIDCopyNotification>().InstancePerDependency();
            Builder.RegisterType<ResultIDCopyViewModel>().InstancePerDependency();
            Builder.RegisterType<ResultLinkCopyNotification>().InstancePerDependency();
            Builder.RegisterType<ResultLinkCopyViewModel>().InstancePerDependency();

            Container = Builder.Build();
        }
    }
}
