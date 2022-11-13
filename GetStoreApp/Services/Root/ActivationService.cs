using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Contracts.Services.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Controls.Settings.Experiment;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用激活服务
    /// </summary>
    public class ActivationService : IActivationService
    {
        private UIElement Shell = null;

        private IAppNotificationService AppNotificationService { get; } = ContainerHelper.GetInstance<IAppNotificationService>();

        private IDataBaseService DataBaseService { get; } = ContainerHelper.GetInstance<IDataBaseService>();

        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private IStartupService StartupService { get; } = ContainerHelper.GetInstance<IStartupService>();

        private IAria2Service Aria2Service { get; } = ContainerHelper.GetInstance<IAria2Service>();

        private IDownloadDBService DownloadDBService { get; } = ContainerHelper.GetInstance<IDownloadDBService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = ContainerHelper.GetInstance<IDownloadSchedulerService>();

        private IAppExitService AppExitService { get; } = ContainerHelper.GetInstance<IAppExitService>();

        private IInstallModeService InstallModeService { get; } = ContainerHelper.GetInstance<IInstallModeService>();

        private IBackdropService BackdropService { get; } = ContainerHelper.GetInstance<IBackdropService>();

        private ILanguageService LanguageService { get; } = ContainerHelper.GetInstance<ILanguageService>();

        private IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        private ITopMostService TopMostService { get; } = ContainerHelper.GetInstance<ITopMostService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = ContainerHelper.GetInstance<IDownloadOptionsService>();

        private IHistoryLiteNumService HistoryLiteNumService { get; } = ContainerHelper.GetInstance<IHistoryLiteNumService>();

        private ILinkFilterService LinkFilterService { get; } = ContainerHelper.GetInstance<ILinkFilterService>();

        private INotificationService NotificationService { get; } = ContainerHelper.GetInstance<INotificationService>();

        private IRegionService RegionService { get; } = ContainerHelper.GetInstance<IRegionService>();

        private IUseInstructionService UseInstructionService { get; } = ContainerHelper.GetInstance<IUseInstructionService>();

        private INetWorkMonitorService NetWorkMonitorService { get; } = ContainerHelper.GetInstance<INetWorkMonitorService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public async Task ActivateAsync(LaunchActivatedEventArgs activationArgs)
        {
            // 在应用窗口激活前配置应用的设置
            await InitializeAsync();

            // 新建导航视图的Frame窗口
            if (App.MainWindow.Content == null)
            {
                Shell = ContainerHelper.GetInstance<ShellPage>();
                App.MainWindow.Content = Shell ?? new Frame();
            }

            NavigationService.NavigateTo(typeof(HomeViewModel).FullName, activationArgs.Arguments, null, false);

            await StartupService.InitializeStartupAsync();

            // 激活应用窗口
            App.MainWindow.Activate();

            // 窗口激活后配置其他设置
            await StartupAsync();
        }

        /// <summary>
        /// 在应用窗口激活前配置应用的设置
        /// </summary>
        private async Task InitializeAsync()
        {
            // 初始化应用通知服务
            //AppNotificationService.Initialize();

            // 初始化应用资源，应用使用的语言信息和启动参数
            await LanguageService.InitializeLanguageAsync();
            await ResourceService.InitializeResourceAsync(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);

            // 初始化数据库信息
            await DataBaseService.InitializeDataBaseAsync();
            await DownloadDBService.InitializeDownloadDBAsync();

            // 初始化应用配置信息
            await AppExitService.InitializeAppExitAsync();
            await InstallModeService.InitializeInstallModeAsync();

            await BackdropService.InitializeBackdropAsync();
            await ThemeService.InitializeThemeAsync();
            await TopMostService.InitializeTopMostValueAsync();

            await DownloadOptionsService.InitializeAsync();
            await HistoryLiteNumService.InitializeHistoryLiteNumAsync();
            await LinkFilterService.InitializeLinkFilterValueAsnyc();
            await NotificationService.InitializeNotificationAsync();
            await RegionService.InitializeRegionAsync();
            await UseInstructionService.InitializeUseInsVisValueAsync();

            // 实验功能设置配置
            await NetWorkMonitorService.InitializeNetWorkMonitorValueAsync();
        }

        /// <summary>
        /// 窗口激活后配置其他设置
        /// </summary>
        private async Task StartupAsync()
        {
            // 设置应用主题
            await ThemeService.SetAppThemeAsync();

            // 设置应用背景色
            await BackdropService.SetAppBackdropAsync();

            // 设置应用置顶状态
            await TopMostService.SetAppTopMostAsync();

            // 初始化下载监控服务
            await DownloadSchedulerService.InitializeDownloadSchedulerAsync();

            // 初始化Aria2配置文件信息
            await Aria2Service.InitializeAria2ConfAsync();

            // 启动Aria2下载服务（该服务会在后台长时间运行）
            await Aria2Service.StartAria2Async();
        }
    }
}
