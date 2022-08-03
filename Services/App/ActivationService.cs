using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
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
        private readonly ActivationHandler<LaunchActivatedEventArgs> DefaultHandler;
        private readonly IEnumerable<IActivationHandler> ActivationHandlers;
        private readonly IDataBaseService DataBaseService;
        private readonly IResourceService ResourceService;
        private readonly IBackdropService BackdropService;
        private readonly IDownloadOptionsService DownloadOptionsService;
        private readonly IHistoryItemValueService HistoryItemValueService;
        private readonly ILanguageService LanguageService;
        private readonly ILinkFilterService LinkFilterService;
        private readonly IRegionService RegionService;
        private readonly IThemeService ThemeService;
        private readonly IUseInstructionService UseInstructionService;
        private UIElement _shell = null;

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers)
        {
            DefaultHandler = defaultHandler;
            ActivationHandlers = activationHandlers;

            DataBaseService = GetStoreApp.App.GetService<IDataBaseService>();
            ResourceService = GetStoreApp.App.GetService<IResourceService>();
            BackdropService = GetStoreApp.App.GetService<IBackdropService>();
            DownloadOptionsService = GetStoreApp.App.GetService<IDownloadOptionsService>();
            HistoryItemValueService = GetStoreApp.App.GetService<IHistoryItemValueService>();
            LanguageService = GetStoreApp.App.GetService<ILanguageService>();
            LinkFilterService = GetStoreApp.App.GetService<ILinkFilterService>();
            RegionService = GetStoreApp.App.GetService<IRegionService>();
            ThemeService = GetStoreApp.App.GetService<IThemeService>();
            UseInstructionService = GetStoreApp.App.GetService<IUseInstructionService>();
        }

        public async Task ActivateAsync(object activationArgs)
        {
            // 在应用窗口激活前配置应用的设置
            await InitializeAsync();

            // 新建导航视图的Frame窗口
            if (GetStoreApp.App.MainWindow.Content == null)
            {
                _shell = GetStoreApp.App.GetService<ShellPage>();
                GetStoreApp.App.MainWindow.Content = _shell ?? new Frame();
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

            // 初始化应用配置信息
            await BackdropService.InitializeBackdropAsync();
            await DownloadOptionsService.InitializeAsync();
            await HistoryItemValueService.InitializeHistoryItemValueAsync();
            await LinkFilterService.InitializeLinkFilterValueAsnyc();
            await RegionService.InitializeRegionAsync();
            await ThemeService.InitializeThemeAsync();
            await UseInstructionService.InitializeUseInsVIsValueAsync();
        }

        /// <summary>
        /// 根据activationArgs的ActivationHandlers或DefaultActivationHandler将导航到第一个页面
        /// </summary>
        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = ActivationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null) await activationHandler.HandleAsync(activationArgs);

            if (DefaultHandler.CanHandle(activationArgs)) await DefaultHandler.HandleAsync(activationArgs);
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

            // 设置应用标题名称
            GetStoreApp.App.MainWindow.Title = ResourceService.GetLocalized("AppDisplayName");
        }
    }
}
