using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Services.Settings;
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
        private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private readonly IDataBaseService _dataBaseService;
        private readonly IBackdropService _backdropService;
        private readonly IHistoryItemValueService _historyItemValueService;
        private readonly ILinkFilterService _linkFilterService;
        private readonly IRegionService _regionService;
        private readonly IThemeService _themeService;
        private readonly IUseInstructionService _useInstructionService;
        private UIElement _shell = null;

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, IDataBaseService dataBaseService, IBackdropService backdropService,IHistoryItemValueService historyItemValueService,ILinkFilterService linkFilterService, IRegionService regionService, IThemeService themeService,IUseInstructionService useInstructionService)
        {
            _defaultHandler = defaultHandler;
            _activationHandlers = activationHandlers;
            _dataBaseService = dataBaseService;
            _backdropService = backdropService;
            _historyItemValueService = historyItemValueService;
            _linkFilterService = linkFilterService;
            _regionService = regionService;
            _themeService = themeService;
            _useInstructionService = useInstructionService;
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
            await _dataBaseService.InitializeDataBaseAsync();

            await _backdropService.InitializeBackdropAsync();
            await _historyItemValueService.InitializeHistoryItemValueAsync();
            await _linkFilterService.InitializeLinkFilterValueAsnyc();
            await _regionService.InitializeRegionAsync();
            await _themeService.InitializeThemeAsync();
            await _useInstructionService.InitializeUseInsVIsValueAsync();
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (_defaultHandler.CanHandle(activationArgs))
            {
                await _defaultHandler.HandleAsync(activationArgs);
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
            await _themeService.SetAppThemeAsync();

            // 设置应用背景色
            await _backdropService.SetAppBackdropAsync();
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
