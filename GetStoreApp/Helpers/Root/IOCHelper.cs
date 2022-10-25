using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Contracts.Services.Controls.History;
using GetStoreApp.Contracts.Services.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Controls.Settings.Experiment;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Shell;
using GetStoreApp.UI.Controls.About;
using GetStoreApp.UI.Controls.Download;
using GetStoreApp.UI.Controls.Home;
using GetStoreApp.UI.Controls.Settings.Advanced;
using GetStoreApp.UI.Controls.Settings.Appearance;
using GetStoreApp.UI.Controls.Settings.Common;
using GetStoreApp.UI.Controls.Settings.Experiment;
using GetStoreApp.UI.Controls.Web;
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
using GetStoreApp.ViewModels.Dialogs.About;
using GetStoreApp.ViewModels.Dialogs.Download;
using GetStoreApp.ViewModels.Dialogs.Settings;
using GetStoreApp.ViewModels.Dialogs.Web;
using GetStoreApp.ViewModels.Notifications;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.ViewModels.Shell;
using GetStoreApp.ViewModels.Window;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Shell;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 控制翻转/依赖注入
    /// </summary>
    public static class IOCHelper
    {
        public static IHost Host { get; set; }

        public static T GetService<T>() where T : class
        {
            if (Host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} 需要在IOCHelper.cs中的ConfigureServices中注册。");
            }

            return service;
        }

        public static void InitializeService()
        {
            Host = Microsoft.Extensions.Hosting.Host.
            CreateDefaultBuilder().
            UseContentRoot(AppContext.BaseDirectory).
            ConfigureServices((context, services) =>
            {
                // 应用服务
                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IAppNotificationService, AppNotificationService>();
                services.AddSingleton<IConfigStorageService, ConfigStorageService>();
                services.AddSingleton<IDataBaseService, DataBaseService>();
                services.AddSingleton<IResourceService, ResourceService>();
                services.AddSingleton<IStartupArgsService, StartupArgsService>();

                services.AddSingleton<IAria2Service, Aria2Service>();
                services.AddSingleton<IDownloadDBService, DownloadDBService>();
                services.AddSingleton<IDownloadSchedulerService, DownloadSchedulerService>();

                services.AddSingleton<IHistoryDBService, HistoryDBService>();

                services.AddSingleton<IAppExitService, AppExitService>();
                services.AddSingleton<IInstallModeService, InstallModeService>();
                services.AddSingleton<ITraceCleanupService, TraceCleanupService>();

                services.AddSingleton<IBackdropService, BackdropService>();
                services.AddSingleton<ILanguageService, LanguageService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<ITopMostService, TopMostService>();

                services.AddSingleton<IDownloadOptionsService, DownloadOptionsService>();
                services.AddSingleton<IHistoryLiteNumService, HistoryLiteNumService>();
                services.AddSingleton<ILinkFilterService, LinkFilterService>();
                services.AddSingleton<INotificationService, NotificatonService>();
                services.AddSingleton<IRegionService, RegionService>();
                services.AddSingleton<IUseInstructionService, UseInstructionService>();

                services.AddSingleton<INetWorkMonitorService, NetWorkMonitorService>();

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();
                services.AddSingleton<IPageService, PageService>();

                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // 应用窗口（MVVM）
                services.AddTransient<MainWindow>();
                services.AddTransient<MainWindowViewModel>();

                // 页面（MVVM）
                services.AddTransient<AboutPage>();
                services.AddTransient<AboutViewModel>();
                services.AddTransient<DownloadPage>();
                services.AddTransient<DownloadViewModel>();
                services.AddTransient<HistoryPage>();
                services.AddTransient<HistoryViewModel>();
                services.AddTransient<HomePage>();
                services.AddTransient<HomeViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();
                services.AddTransient<WebPage>();
                services.AddTransient<WebViewModel>();

                // 托盘控件（MVVM）
                services.AddTransient<TaskBarControl>();
                services.AddTransient<TaskBarViewModel>();

                // 关于页面的控件（MVVM）
                services.AddTransient<HeaderControl>();
                services.AddTransient<HeaderViewModel>();
                services.AddTransient<InstructionsControl>();
                services.AddTransient<InstructionsViewModel>();
                services.AddTransient<IntroductionControl>();
                services.AddTransient<IntroductionViewModel>();
                services.AddTransient<PrecautionControl>();
                services.AddTransient<PrecautionViewModel>();
                services.AddTransient<ReferenceControl>();
                services.AddTransient<ReferenceViewModel>();
                services.AddTransient<SettingsHelpControl>();
                services.AddTransient<SettingsHelpViewModel>();

                // 下载页面的控件（MVVM）
                services.AddTransient<DownloadingControl>();
                services.AddTransient<DownloadingViewModel>();
                services.AddTransient<CompletedControl>();
                services.AddTransient<CompletedViewModel>();
                services.AddTransient<UnfinishedControl>();
                services.AddTransient<UnfinishedViewModel>();

                // 历史记录页面的控件（MVVM）
                services.AddTransient<HistoryLiteControl>();
                services.AddTransient<HistoryLiteViewModel>();
                services.AddTransient<RequestControl>();
                services.AddTransient<RequestViewModel>();
                services.AddTransient<ResultControl>();
                services.AddTransient<ResultViewModel>();
                services.AddTransient<StatusBarControl>();
                services.AddTransient<StatusBarViewModel>();

                // 设置页面的控件（MVVM）
                // 高级选项
                services.AddTransient<AppExitControl>();
                services.AddTransient<AppExitViewModel>();
                services.AddTransient<ExperimentalFeaturesControl>();
                services.AddTransient<ExperimentalFeaturesViewModel>();
                services.AddTransient<InstallModeControl>();
                services.AddTransient<InstallModeViewModel>();
                services.AddTransient<TraceCleanupControl>();
                services.AddTransient<TraceCleanupViewModel>();

                // 外观
                services.AddTransient<BackdropControl>();
                services.AddTransient<BackdropViewModel>();
                services.AddTransient<LauguageControl>();
                services.AddTransient<LanguageViewModel>();
                services.AddTransient<ThemeControl>();
                services.AddTransient<ThemeViewModel>();
                services.AddTransient<TopMostControl>();
                services.AddTransient<TopMostViewModel>();

                // 常规选项
                services.AddTransient<DownloadOptionsControl>();
                services.AddTransient<DownloadOptionsViewModel>();
                services.AddTransient<HistoryLiteConfigControl>();
                services.AddTransient<HistoryLiteConfigViewModel>();
                services.AddTransient<LinkFilterControl>();
                services.AddTransient<LinkFilterViewModel>();
                services.AddTransient<NotificationControl>();
                services.AddTransient<NotificationViewModel>();
                services.AddTransient<RegionControl>();
                services.AddTransient<RegionViewModel>();
                services.AddTransient<UseInstructionControl>();
                services.AddTransient<UseInstructionViewModel>();

                // 实验性功能选项
                services.AddTransient<NetWorkMonitorControl>();
                services.AddTransient<NetWorkMonitorViewModel>();
                services.AddTransient<OpenConfigFIleControl>();
                services.AddTransient<OpenConfigFileViewModel>();

                // 网页界面控件（MVVM）
                services.AddTransient<LoadFailedControl>();
                services.AddTransient<LoadFailedViewModel>();

                // 对话框（MVVM）
                services.AddTransient<CoreWebView2FailedDialog>();
                services.AddTransient<CoreWebView2FailedViewModel>();
                services.AddTransient<DesktopStartupArgsDialog>();
                services.AddTransient<DesktopStartupArgsViewModel>();
                services.AddTransient<ExperimentalConfigDialog>();
                services.AddTransient<ExperimentalConfigViewModel>();
                services.AddTransient<FileInformationDialog>();
                services.AddTransient<FileInformationViewModel>();
                services.AddTransient<LicenseDialog>();
                services.AddTransient<LicenseViewModel>();
                services.AddTransient<RestartAppsDialog>();
                services.AddTransient<RestartAppsViewModel>();
                services.AddTransient<TraceCleanupPromptDialog>();
                services.AddTransient<TraceCleanupPromptViewModel>();

                // 应用内消息通知框（MVVM）
                services.AddTransient<DownloadCreateNotification>();
                services.AddTransient<DownloadCreateViewModel>();
                services.AddTransient<ExceptionCopyNotification>();
                services.AddTransient<ExceptionCopyViewModel>();
                services.AddTransient<FileInformationCopyNotification>();
                services.AddTransient<FileInformationCopyViewModel>();
                services.AddTransient<HistoryCopyNotification>();
                services.AddTransient<HistoryCopyViewModel>();
                services.AddTransient<LanguageChangeNotification>();
                services.AddTransient<LanguageChangeViewModel>();
                services.AddTransient<ResultContentCopyNotification>();
                services.AddTransient<ResultContentCopyViewModel>();
                services.AddTransient<ResultIDCopyNotification>();
                services.AddTransient<ResultIDCopyViewModel>();
                services.AddTransient<ResultLinkCopyNotification>();
                services.AddTransient<ResultLinkCopyViewModel>();
            })
            .Build();
        }
    }
}
