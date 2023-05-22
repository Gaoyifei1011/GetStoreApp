using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Window;
using GetStoreApp.Views.Window;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace GetStoreApp
{
    public partial class WinUIApp : Application
    {
        public MainWindow MainWindow { get; private set; }

        public TrayMenuWindow TrayMenuWindow { get; private set; }

        public WindowTrayIcon TrayIcon { get; private set; }

        public JumpList TaskbarJumpList { get; private set; }

        public WinUIAppViewModel ViewModel { get; } = new WinUIAppViewModel();

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
            ResourceDictionaryHelper.InitializeResourceDictionary();

            InitializeMainWindow();
            InitializeTrayMenuWindow();
            InitializeTrayIcon();
            Program.IsAppLaunched = true;
            await ViewModel.ActivateAsync();

            await InitializeJumpListAsync();
            ViewModel.Startup();
            await ViewModel.HandleAppNotificationAsync();
        }

        /// <summary>
        /// 初始化应用的主窗口
        /// </summary>
        private void InitializeMainWindow()
        {
            MainWindow = new MainWindow();
            MainWindow.InitializeWindowProc();
            WindowHelper.InitializePresenter(Program.ApplicationRoot.MainWindow.AppWindow);
        }

        /// <summary>
        /// 初始化应用的托盘右键菜单窗口
        /// </summary>
        private void InitializeTrayMenuWindow()
        {
            TrayMenuWindow = new TrayMenuWindow();
            TrayMenuWindow.InitializeWindow();
        }

        /// <summary>
        /// 初始化应用的托盘图标
        /// </summary>
        private void InitializeTrayIcon()
        {
            TrayIcon = new WindowTrayIcon(ResourceService.GetLocalized("Resources/AppDisplayName"));
            TrayIcon.DoubleClick += ViewModel.DoubleClick;
            TrayIcon.RightClick += ViewModel.RightClick;
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
