using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Services.App
{
    /// <summary>
    /// 应用激活服务
    /// </summary>
    public class ActivationService : IActivationService
    {
        private UIElement _shell = null;

        private ActivationHandler<LaunchActivatedEventArgs> DefaultHandler { get; } = GetStoreApp.App.GetService<ActivationHandler<LaunchActivatedEventArgs>>();

        private IEnumerable<IActivationHandler> ActivationHandlers { get; } = GetStoreApp.App.GetService<IEnumerable<IActivationHandler>>();

        private IDataBaseService DataBaseService { get; } = GetStoreApp.App.GetService<IDataBaseService>();

        private IResourceService ResourceService { get; } = GetStoreApp.App.GetService<IResourceService>();

        private IBackdropService BackdropService { get; } = GetStoreApp.App.GetService<IBackdropService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = GetStoreApp.App.GetService<IDownloadOptionsService>();

        private IHistoryItemValueService HistoryItemValueService { get; } = GetStoreApp.App.GetService<IHistoryItemValueService>();

        private ILanguageService LanguageService { get; } = GetStoreApp.App.GetService<ILanguageService>();

        private ILinkFilterService LinkFilterService { get; } = GetStoreApp.App.GetService<ILinkFilterService>();

        private IRegionService RegionService { get; } = GetStoreApp.App.GetService<IRegionService>();

        private IThemeService ThemeService { get; } = GetStoreApp.App.GetService<IThemeService>();

        private IUseInstructionService UseInstructionService { get; } = GetStoreApp.App.GetService<IUseInstructionService>();

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
            Debug.WriteLine("04");
            await DownloadOptionsService.InitializeAsync();
            Debug.WriteLine("05");
            await HistoryItemValueService.InitializeHistoryItemValueAsync();
            Debug.WriteLine("06");
            await LinkFilterService.InitializeLinkFilterValueAsnyc();
            Debug.WriteLine("07");
            await RegionService.InitializeRegionAsync();
            Debug.WriteLine("08");
            await ThemeService.InitializeThemeAsync();
            Debug.WriteLine("09");
            await UseInstructionService.InitializeUseInsVisValueAsync();
            Debug.WriteLine("03");
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
