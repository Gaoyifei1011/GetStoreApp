using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Contracts.Services.Web;
using GetStoreApp.Services.App;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.History;
using GetStoreApp.Services.Settings;
using GetStoreApp.Services.Shell;
using GetStoreApp.Services.Web;
using GetStoreApp.UI.Controls.About;
using GetStoreApp.UI.Controls.Download;
using GetStoreApp.UI.Controls.Home;
using GetStoreApp.UI.Controls.Settings;
using GetStoreApp.UI.Dialogs;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Controls.About;
using GetStoreApp.ViewModels.Controls.Download;
using GetStoreApp.ViewModels.Controls.Home;
using GetStoreApp.ViewModels.Controls.Settings;
using GetStoreApp.ViewModels.Dialogs;
using GetStoreApp.ViewModels.Notifications;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.ViewModels.Window;
using GetStoreApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.Helpers
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

        public static void InitializeIOCService()
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

                services.AddSingleton<IAria2Service, Aria2Service>();
                services.AddSingleton<IDownloadDBService, DownloadDBService>();
                services.AddSingleton<IDownloadSchedulerService, DownloadSchedulerService>();

                services.AddSingleton<IHistoryDBService, HistoryDBService>();

                services.AddSingleton<IBackdropService, BackdropService>();
                services.AddSingleton<IDownloadOptionsService, DownloadOptionsService>();
                services.AddSingleton<IHistoryLiteNumService, HistoryLiteNumService>();
                services.AddSingleton<ILanguageService, LanguageService>();
                services.AddSingleton<ILinkFilterService, LinkFilterService>();
                services.AddSingleton<INotificationService, NotificatonService>();
                services.AddSingleton<IRegionService, RegionService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<ITopMostService, TopMostService>();
                services.AddSingleton<IUseInstructionService, UseInstructionService>();

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();
                services.AddSingleton<IPageService, PageService>();

                services.AddTransient<IWebViewService, WebViewService>();

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

                // 关于页面的控件（MVVM）
                services.AddTransient<HeaderControl>();
                services.AddTransient<HeaderViewModel>();
                services.AddTransient<InstructionsControl>();
                services.AddTransient<InstructionsViewModel>();
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
                services.AddTransient<BackdropControl>();
                services.AddTransient<BackdropViewModel>();
                services.AddTransient<DownloadOptionsControl>();
                services.AddTransient<DownloadOptionsViewModel>();
                services.AddTransient<HistoryLiteConfigControl>();
                services.AddTransient<HistoryLiteConfigViewModel>();
                services.AddTransient<LauguageControl>();
                services.AddTransient<LanguageViewModel>();
                services.AddTransient<LinkFilterControl>();
                services.AddTransient<LinkFilterViewModel>();
                services.AddTransient<NotificationControl>();
                services.AddTransient<NotificationViewModel>();
                services.AddTransient<RegionControl>();
                services.AddTransient<RegionViewModel>();
                services.AddTransient<ThemeControl>();
                services.AddTransient<ThemeViewModel>();
                services.AddTransient<TopMostControl>();
                services.AddTransient<TopMostViewModel>();
                services.AddTransient<TraceCleanupControl>();
                services.AddTransient<TraceCleanupViewModel>();
                services.AddTransient<UseInstructionControl>();
                services.AddTransient<UseInstructionViewModel>();

                // 对话框（MVVM）
                services.AddTransient<RestartAppsDialog>();
                services.AddTransient<RestartAppsViewModel>();
                services.AddTransient<TraceCleanupPromptDialog>();
                services.AddTransient<TraceCleanupPromptViewModel>();

                // 应用内消息通知框（MVVM）
                services.AddTransient<DownloadCreateNotification>();
                services.AddTransient<DownloadCreateViewModel>();
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
