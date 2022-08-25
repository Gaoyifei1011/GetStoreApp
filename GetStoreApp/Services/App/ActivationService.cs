using GetStoreApp.Activation;
using GetStoreApp.Contracts.Activation;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Services.App
{
    /// <summary>
    /// 应用激活服务
    /// </summary>
    public class ActivationService : IActivationService
    {
        private UIElement Shell = null;

        private ActivationHandler<LaunchActivatedEventArgs> DefaultHandler { get; } = IOCHelper.GetService<ActivationHandler<LaunchActivatedEventArgs>>();

        private IEnumerable<IActivationHandler> ActivationHandlers { get; } = IOCHelper.GetService<IEnumerable<IActivationHandler>>();

        private IDataBaseService DataBaseService { get; } = IOCHelper.GetService<IDataBaseService>();

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        private IDownloadDBService DownloadDatatService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private IBackdropService BackdropService { get; } = IOCHelper.GetService<IBackdropService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private IHistoryItemValueService HistoryItemValueService { get; } = IOCHelper.GetService<IHistoryItemValueService>();

        private ILanguageService LanguageService { get; } = IOCHelper.GetService<ILanguageService>();

        private ILinkFilterService LinkFilterService { get; } = IOCHelper.GetService<ILinkFilterService>();

        private IRegionService RegionService { get; } = IOCHelper.GetService<IRegionService>();

        private IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        private ITopMostService TopMostService { get; } = IOCHelper.GetService<ITopMostService>();

        private IUseInstructionService UseInstructionService { get; } = IOCHelper.GetService<IUseInstructionService>();

        public async Task ActivateAsync(object activationArgs)
        {
            // 在应用窗口激活前配置应用的设置
            await InitializeAsync();

            // 新建导航视图的Frame窗口
            if (GetStoreApp.App.MainWindow.Content == null)
            {
                Shell = IOCHelper.GetService<ShellPage>();
                GetStoreApp.App.MainWindow.Content = Shell ?? new Frame();
            }

            // 根据activationArgs的ActivationHandlers或DefaultActivationHandler将导航到第一个页面
            await HandleActivationAsync(activationArgs);

            // 激活应用窗口
            GetStoreApp.App.MainWindow.Activate();

            // 窗口激活后配置其他设置
            await StartupAsync();
        }

        /// <summary>
        /// 在应用窗口激活前配置应用的设置
        /// </summary>
        private async Task InitializeAsync()
        {
            // 初始化应用资源及应用使用的语言信息
            await LanguageService.InitializeLanguageAsync();
            await LanguageService.SetAppLanguageAsync();
            await ResourceService.InitializeResourceAsync(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);

            // 初始化数据库信息
            await DataBaseService.InitializeDataBaseAsync();
            await DownloadDatatService.InitializeDownloadDBAsync();

            // 初始化应用配置信息
            await BackdropService.InitializeBackdropAsync();
            await DownloadOptionsService.InitializeAsync();
            await HistoryItemValueService.InitializeHistoryItemValueAsync();
            await LinkFilterService.InitializeLinkFilterValueAsnyc();
            await RegionService.InitializeRegionAsync();
            await ThemeService.InitializeThemeAsync();
            await TopMostService.InitializeTopMostValueAsync();
            await UseInstructionService.InitializeUseInsVisValueAsync();
        }

        /// <summary>
        /// 根据activationArgs的ActivationHandlers或DefaultActivationHandler将导航到第一个页面
        /// </summary>
        private async Task HandleActivationAsync(object activationArgs)
        {
            IActivationHandler activationHandler = ActivationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (DefaultHandler.CanHandle(activationArgs))
            {
                await DefaultHandler.HandleAsync(activationArgs);
            }
        }

        /// <summary>
        /// 窗口激活后配置其他设置
        /// </summary>
        private async Task StartupAsync()
        {
            GetStoreApp.App.MainWindow.ExtendsContentIntoTitleBar = true;

            // 设置应用主题
            await ThemeService.SetAppThemeAsync();

            // 设置应用背景色
            await BackdropService.SetAppBackdropAsync();

            // 设置应用置顶状态
            await TopMostService.SetAppTopMostAsync();

            // 设置应用标题名称
            GetStoreApp.App.MainWindow.Title = ResourceService.GetLocalized("AppDisplayName");

            // 初始化下载监控服务
            await DownloadSchedulerService.InitializeDownloadMonitorAsync();

            // 启动Aria2下载服务（该服务会在后台长时间运行）
            await Aria2Service.InitializeAria2Async();
        }
    }
}
