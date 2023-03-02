using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Window;
using GetStoreApp.Views.Window;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace GetStoreApp
{
    public partial class WinUIApp : Application
    {
        public AppWindow AppWindow { get; private set; }

        public MainWindow MainWindow { get; private set; }

        public JumpList TaskbarJumpList { get; private set; }

        public AppViewModel ViewModel { get; } = new AppViewModel();

        public WinUIApp()
        {
            InitializeComponent();
            UnhandledException += ViewModel.OnUnhandledException;
        }

        /// <summary>
        /// 应用启动后执行其他操作
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            await ResourceDictionaryHelper.InitializeResourceDictionaryAsync();

            InitializeMainWindow();
            InitializeAppWindow();
            Program.IsAppLaunched = true;
            await ViewModel.ActivateAsync();

            // 启动任务栏通知区域进程
            await TaskbarService.StartTaskbarProcessAsync();
            await InitializeJumpListAsync();
            await ViewModel.StartupAsync();
            await ViewModel.HandleAppNotificationAsync();
        }

        /// <summary>
        /// 初始化应用的主窗口
        /// </summary>
        private void InitializeMainWindow()
        {
            MainWindow = new MainWindow();
            MainWindow.InitializeWindowProc();
        }

        /// <summary>
        /// 初始化应用的AppWindow
        /// </summary>
        private void InitializeAppWindow()
        {
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(MainWindow.GetMainWindowHandle());
            AppWindow = AppWindow.GetFromWindowId(windowId);
            WindowHelper.InitializePresenter(AppWindow);
        }

        /// <summary>
        /// 初始化任务栏的跳转列表
        /// </summary>
        private async Task InitializeJumpListAsync()
        {
            if (JumpList.IsSupported())
            {
                TaskbarJumpList = await JumpList.LoadCurrentAsync();
                TaskbarJumpList.SystemGroupKind = AppJumpList.GroupKind;
                ViewModel.RemoveUnusedItems();
                ViewModel.UpdateJumpListGroupName();
                await TaskbarJumpList.SaveAsync();
            }
        }
    }
}
