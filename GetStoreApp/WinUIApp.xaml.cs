using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Window;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;
using Windows.UI.StartScreen;

namespace GetStoreApp
{
    public partial class WinUIApp : Application, IDisposable
    {
        private bool IsDisposed;

        private IntPtr[] hIcons;

        public MainWindow MainWindow { get; private set; }

        public TrayMenuWindow TrayMenuWindow { get; private set; }

        public WindowTrayIcon TrayIcon { get; private set; }

        public JumpList TaskbarJumpList { get; private set; }

        public WinUIApp()
        {
            InitializeComponent();
            UnhandledException += OnUnhandledException;
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
            await ActivateAsync();

            await InitializeJumpListAsync();
            Startup();
        }

        /// <summary>
        /// 初始化应用的主窗口
        /// </summary>
        private void InitializeMainWindow()
        {
            MainWindow = new MainWindow();
            MainWindow.InitializeWindow();
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
            TrayIcon.DoubleClick += DoubleClick;
            TrayIcon.RightClick += RightClick;
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
                RemoveUnusedItems();
                UpdateJumpListGroupName();
                await TaskbarJumpList.SaveAsync();
            }
        }

        /// <summary>
        /// 双击任务栏图标：显示 / 隐藏窗口
        /// </summary>
        public void DoubleClick(object sender, RoutedEventArgs args)
        {
            MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                // 隐藏窗口
                if (MainWindow.Visible)
                {
                    MainWindow.AppWindow.Hide();
                }
                // 显示窗口
                else
                {
                    MainWindow.Show();
                }
            });
        }

        /// <summary>
        /// 右键任务栏图标：显示菜单窗口
        /// </summary>
        public void RightClick(object sender, RoutedEventArgs args)
        {
            unsafe
            {
                // 获取当前鼠标位置信息
                PointInt32 pt;
                User32Library.GetCursorPos(&pt);

                int x = DPICalcHelper.ConvertPixelToEpx(MainWindow.Handle, pt.X);
                int y = DPICalcHelper.ConvertPixelToEpx(MainWindow.Handle, pt.Y);

                User32Library.SetForegroundWindow(TrayMenuWindow.Handle);
                if (InfoHelper.SystemVersion.Build >= 22000)
                {
                    TrayMenuWindow.TrayMenuFlyout.ShowAt(null, new Point(x, y));
                }
                else
                {
                    // 取消托盘窗口置顶
                    IntPtr hwnd = User32Library.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "NotifyIconOverflowWindow", null);

                    if (hwnd != IntPtr.Zero)
                    {
                        User32Library.SetWindowPos(hwnd, -2, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE);

                        do
                        {
                            hwnd = User32Library.FindWindowEx(hwnd, IntPtr.Zero, "ToolbarWindow32", null);

                            if (hwnd != IntPtr.Zero)
                            {
                                User32Library.SetWindowPos(hwnd, -2, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE);
                            }
                        } while (hwnd != IntPtr.Zero);
                    }

                    TrayMenuWindow.TrayMenuFlyout.ShowAt(null, new Point(pt.X, pt.Y));
                }
            }
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        public void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            // 系统背景色弹出的异常，不进行处理
            if (args.Exception.HResult == -2147024809 && args.Exception.StackTrace.Contains("SystemBackdropConfiguration"))
            {
                LogService.WriteLog(LogType.WARNING, "System backdrop config warning.", args.Exception);
                return;
            }
            // 处理其他异常
            else
            {
                LogService.WriteLog(LogType.ERROR, "Unknown unhandled exception.", args.Exception);

                // 退出应用
                Dispose();
            }
        }

        /// <summary>
        /// 窗口激活前进行配置
        /// </summary>
        public async Task ActivateAsync()
        {
            bool? IsWindowMaximized = await ConfigService.ReadSettingAsync<bool?>(ConfigKey.IsWindowMaximizedKey);
            int? WindowWidth = await ConfigService.ReadSettingAsync<int?>(ConfigKey.WindowWidthKey);
            int? WindowHeight = await ConfigService.ReadSettingAsync<int?>(ConfigKey.WindowHeightKey);
            int? WindowPositionXAxis = await ConfigService.ReadSettingAsync<int?>(ConfigKey.WindowPositionXAxisKey);
            int? WindowPositionYAxis = await ConfigService.ReadSettingAsync<int?>(ConfigKey.WindowPositionYAxisKey);

            if (IsWindowMaximized.HasValue && IsWindowMaximized.Value == true)
            {
                MainWindow.Presenter.Maximize();
            }
            else
            {
                if (WindowWidth.HasValue && WindowHeight.HasValue && WindowPositionXAxis.HasValue && WindowPositionYAxis.HasValue)
                {
                    MainWindow.AppWindow.MoveAndResize(new RectInt32(
                        WindowPositionXAxis.Value,
                        WindowPositionYAxis.Value,
                        WindowWidth.Value,
                        WindowHeight.Value
                        ));
                }
            }

            SetAppIcon();
            MainWindow.AppWindow.Show();
        }

        /// <summary>
        /// 窗口激活后配置其他设置
        /// </summary>
        public void Startup()
        {
            // 设置应用主题
            ThemeService.SetWindowTheme();
            ThemeService.SetTrayWindowTheme();

            // 设置应用背景色
            BackdropService.SetAppBackdrop();

            // 设置应用置顶状态
            TopMostService.SetAppTopMost();

            // 初始化 WinGet 程序包安装信息
            WinGetService.InitializeService();

            // 初始化下载监控服务
            DownloadSchedulerService.InitializeDownloadScheduler();

            // 初始化Aria2配置文件信息
            Aria2Service.InitializeAria2Conf();

            // 启动Aria2下载服务
            Aria2Service.StartAria2Process();
        }

        /// <summary>
        /// 移除用户主动删除的条目
        /// </summary>
        public void RemoveUnusedItems()
        {
            // 如果某一条目被用户主动删除，应用初始化时则自动删除该条目
            for (int index = 0; index < TaskbarJumpList.Items.Count; index++)
            {
                if (TaskbarJumpList.Items[index].RemovedByUser)
                {
                    TaskbarJumpList.Items.RemoveAt(index);
                    index--;
                }
            }
        }

        /// <summary>
        /// 更新跳转列表组名
        /// </summary>
        public void UpdateJumpListGroupName()
        {
            foreach (JumpListItem jumpListItem in TaskbarJumpList.Items)
            {
                jumpListItem.GroupName = AppJumpList.GroupName;
            }
        }

        /// <summary>
        /// 设置应用窗口图标
        /// </summary>
        private void SetAppIcon()
        {
            // 选中文件中的图标总数
            int iconTotalCount = User32Library.PrivateExtractIcons(string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), "GetStoreApp.exe"), 0, 0, 0, null, null, 0, 0);

            // 用于接收获取到的图标指针
            hIcons = new IntPtr[iconTotalCount];

            // 对应的图标id
            int[] ids = new int[iconTotalCount];

            // 成功获取到的图标个数
            int successCount = User32Library.PrivateExtractIcons(string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), "GetStoreApp.exe"), 0, 256, 256, hIcons, ids, iconTotalCount, 0);

            // GetStoreApp.exe 应用程序只有一个图标
            if (successCount >= 1 && hIcons[0] != IntPtr.Zero)
            {
                MainWindow.AppWindow.SetIcon(Win32Interop.GetIconIdFromIcon(hIcons[0]));
            }
        }

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        private async Task CloseAppAsync()
        {
            await SaveWindowInformationAsync();
            DownloadSchedulerService.CloseDownloadScheduler();
            Aria2Service.CloseAria2();
            TrayIcon.Dispose();
            TrayIcon.DoubleClick -= DoubleClick;
            TrayIcon.RightClick -= RightClick;
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }

        /// <summary>
        /// 关闭窗口时保存窗口的大小和位置信息
        /// </summary>
        private async Task SaveWindowInformationAsync()
        {
            await ConfigService.SaveSettingAsync(ConfigKey.IsWindowMaximizedKey, MainWindow.AppTitlebar.IsWindowMaximized);
            await ConfigService.SaveSettingAsync(ConfigKey.WindowWidthKey, MainWindow.AppWindow.Size.Width);
            await ConfigService.SaveSettingAsync(ConfigKey.WindowHeightKey, MainWindow.AppWindow.Size.Height);
            await ConfigService.SaveSettingAsync(ConfigKey.WindowPositionXAxisKey, MainWindow.AppWindow.Position.X);
            await ConfigService.SaveSettingAsync(ConfigKey.WindowPositionYAxisKey, MainWindow.AppWindow.Position.Y);
        }

        /// <summary>
        /// 重启应用
        /// </summary>
        public void Restart()
        {
            MainWindow.AppWindow.Hide();
            unsafe
            {
                Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetProcessStartupInfo);
                WinGetProcessStartupInfo.lpReserved = null;
                WinGetProcessStartupInfo.lpDesktop = null;
                WinGetProcessStartupInfo.lpTitle = null;
                WinGetProcessStartupInfo.dwX = 0;
                WinGetProcessStartupInfo.dwY = 0;
                WinGetProcessStartupInfo.dwXSize = 0;
                WinGetProcessStartupInfo.dwYSize = 0;
                WinGetProcessStartupInfo.dwXCountChars = 500;
                WinGetProcessStartupInfo.dwYCountChars = 500;
                WinGetProcessStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                WinGetProcessStartupInfo.wShowWindow = WindowShowStyle.SW_SHOWNORMAL;
                WinGetProcessStartupInfo.cbReserved2 = 0;
                WinGetProcessStartupInfo.lpReserved2 = IntPtr.Zero;
                WinGetProcessStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                bool createResult = Kernel32Library.CreateProcess(null, "GetStoreApp.exe Restart", IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.None, IntPtr.Zero, null, ref WinGetProcessStartupInfo, out PROCESS_INFORMATION WinGetProcessInformation);

                if (createResult)
                {
                    if (WinGetProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hProcess);
                    if (WinGetProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hThread);
                }
            }
            Dispose();
        }

        /// <summary>
        /// 释放对象。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // 此对象将由 Dispose 方法清理。因此，您应该调用 GC.SuppressFinalize() 将此对象从终结队列中删除，并防止此对象的终结代码再次执行。
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 仅当 <see cref="Dispose()"/> 方法未被调用时，此析构函数才会运行。这使此基类有机会完成。
        /// <para>注意： 不要在从此类派生的类型中提供析构函数。</para>
        /// </summary>
        ~WinUIApp()
        {
            Dispose(false);
        }

        /// <summary>
        /// 删除接收窗口消息的窗口挂钩并关闭基础帮助程序窗口。
        /// </summary>
        private void Dispose(bool disposing)
        {
            // 如果组件已释放，则不执行任何操作
            if (IsDisposed)
            {
                return;
            };

            if (disposing)
            {
                // 始终销毁非托管句柄（即使从 GC 调用）
                if (hIcons is not null)
                {
                    foreach (IntPtr hIcon in hIcons)
                    {
                        User32Library.DestroyIcon(hIcon);
                    }
                }
                CloseAppAsync().Wait();
            }
            IsDisposed = true;
        }
    }
}
