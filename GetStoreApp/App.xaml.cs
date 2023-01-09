using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Window;
using GetStoreApp.Views.Window;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace GetStoreApp
{
    public partial class App : Application
    {
        public AppWindow AppWindow { get; set; }

        public MainWindow MainWindow { get; set; }

        public WindowsTrayIcon TrayIcon { get; set; }

        public JumpList TaskbarJumpList { get; set; }

        public AppViewModel ViewModel { get; } = new AppViewModel();

        // 标志内容对话框是否处于正在打开状态。若是，则不再打开其他内容对话框，防止造成应用异常
        public bool IsDialogOpening { get; set; } = false;

        // 导航页面后使用到的参数
        public AppNaviagtionArgs NavigationArgs { get; set; } = AppNaviagtionArgs.None;

        public App()
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

            InitializeMainWindow();
            InitializeAppWindow();
            Program.IsAppLaunched = true;
            await ViewModel.ActivateAsync();

            InitializeTrayIcon();
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
        }

        /// <summary>
        /// 初始化应用的AppWindow
        /// </summary>
        private void InitializeAppWindow()
        {
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(MainWindow.GetMainWindowHandle());
            AppWindow = AppWindow.GetFromWindowId(windowId);
        }

        /// <summary>
        /// 初始化应用的托盘图标
        /// </summary>
        private void InitializeTrayIcon()
        {
            TrayIcon = new WindowsTrayIcon(
                Path.Combine(AppContext.BaseDirectory, "Assets/GetStoreApp.ico"),
                ResourceService.GetLocalized("Resources/AppDisplayName")
            );

            TrayIcon.InitializeTrayMenu();
            TrayIcon.AddMenuItemText(1, ResourceService.GetLocalized("Resources/ShowOrHideWindow"));
            TrayIcon.AddMenuItemText(2, ResourceService.GetLocalized("Resources/Settings"));
            TrayIcon.AddMenuItemSeperator();
            TrayIcon.AddMenuItemText(3, ResourceService.GetLocalized("Resources/Exit"));

            TrayIcon.DoubleClick = () =>
            {
                MainWindow.DispatcherQueue.TryEnqueue(() => { ViewModel.ShowOrHideWindowCommand.Execute(null); });
            };
            TrayIcon.RightClick = () =>
            {
                MainWindow.DispatcherQueue.TryEnqueue(() => { TrayIcon.ShowContextMenu(); });
            };
            TrayIcon.MenuCommand = (menuid) =>
            {
                switch (menuid)
                {
                    case 1:
                        {
                            MainWindow.DispatcherQueue.TryEnqueue(() => { ViewModel.ShowOrHideWindowCommand.Execute(null); });
                            break;
                        }
                    case 2:
                        {
                            MainWindow.DispatcherQueue.TryEnqueue(() => { ViewModel.SettingsCommand.Execute(null); });
                            break;
                        }
                    case 3:
                        {
                            MainWindow.DispatcherQueue.TryEnqueue(() => { ViewModel.ExitCommand.Execute(null); });
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            };
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
