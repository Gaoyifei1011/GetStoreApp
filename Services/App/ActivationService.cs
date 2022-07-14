﻿using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Views;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace GetStoreApp.Services.App
{
    public class ActivationService : IActivationService
    {
        private readonly ActivationHandler<LaunchActivatedEventArgs> DefaultHandler;
        private readonly IEnumerable<IActivationHandler> ActivationHandlers;
        private readonly IDataBaseService DataBaseService;
        private readonly IResourceService ResourceService;
        private readonly IBackdropService BackdropService;
        private readonly IHistoryItemValueService HistoryItemValueService;
        private readonly ILanguageService LanguageService;
        private readonly ILinkFilterService LinkFilterService;
        private readonly IRegionService RegionService;
        private readonly IThemeService ThemeService;
        private readonly IUseInstructionService UseInstructionService;
        private UIElement _shell = null;

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, IDataBaseService dataBaseService, IResourceService resourceService, IBackdropService backdropService, IHistoryItemValueService historyItemValueService, ILanguageService languageService, ILinkFilterService linkFilterService, IRegionService regionService, IThemeService themeService, IUseInstructionService useInstructionService)
        {
            DefaultHandler = defaultHandler;
            ActivationHandlers = activationHandlers;
            DataBaseService = dataBaseService;
            ResourceService = resourceService;
            BackdropService = backdropService;
            HistoryItemValueService = historyItemValueService;
            LanguageService = languageService;
            LinkFilterService = linkFilterService;
            RegionService = regionService;
            ThemeService = themeService;
            UseInstructionService = useInstructionService;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            // Execute tasks before activation.
            await InitializeAsync();

            if (GetStoreApp.App.MainWindow.Content == null)
            {
                _shell = GetStoreApp.App.GetService<ShellPage>();
                GetStoreApp.App.MainWindow.Content = _shell ?? new Frame();
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);

            // Ensure the current window is active
            GetStoreApp.App.MainWindow.Activate();

            // Tasks after activation
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
            await HistoryItemValueService.InitializeHistoryItemValueAsync();
            await LinkFilterService.InitializeLinkFilterValueAsnyc();
            await RegionService.InitializeRegionAsync();
            await ThemeService.InitializeThemeAsync();
            await UseInstructionService.InitializeUseInsVIsValueAsync();
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = ActivationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

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
        /// 应用启动时进行的操作
        /// </summary>
        private async Task StartupAsync()
        {
            GetStoreApp.App.MainWindow.ExtendsContentIntoTitleBar = true;

            await SetAppTitleBarIconAsync();

            // 设置应用主题
            await ThemeService.SetAppThemeAsync();

            // 设置应用背景色
            await BackdropService.SetAppBackdropAsync(ThemeService.AppTheme, BackdropService.AppBackdrop);
        }

        /// <summary>
        /// 修改标题栏的应用名称
        /// </summary>
        private async Task SetAppTitleBarIconAsync()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(GetStoreApp.App.MainWindow);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(myWndId);

            appWindow.SetIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Assets/Logo/GetStoreApp.ico"));

            await Task.CompletedTask;
        }
    }
}
