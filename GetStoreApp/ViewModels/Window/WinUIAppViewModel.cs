using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.UI.StartScreen;

namespace GetStoreApp.ViewModels.Window
{
    /// <summary>
    /// 应用类数据模型
    /// </summary>
    public sealed class WinUIAppViewModel : IDisposable
    {
        private bool IsDisposed;

        private IntPtr[] hIcons;

        /// <summary>
        /// 双击任务栏图标：显示 / 隐藏窗口
        /// </summary>
        public void DoubleClick(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                // 隐藏窗口
                if (Program.ApplicationRoot.MainWindow.Visible)
                {
                    Program.ApplicationRoot.MainWindow.AppWindow.Hide();
                }
                // 显示窗口
                else
                {
                    Program.ApplicationRoot.MainWindow.Show();
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

                // 获取当前状态栏信息
                APPBARDATA appbarData = new APPBARDATA();
                Shell32Library.SHAppBarMessage(AppBarMessage.ABM_GETTASKBARPOS, ref appbarData);

                // 获取屏幕信息
                IntPtr hMonitor = User32Library.MonitorFromWindow(Program.ApplicationRoot.TrayMenuWindow.Handle, MonitorFlags.MONITOR_DEFAULTTONEAREST);
                MONITORINFO monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                User32Library.GetMonitorInfo(hMonitor, out monitorInfo);

                // 调整窗口的大小
                Program.ApplicationRoot.TrayMenuWindow.SetWindowSize();
                // 计算窗口应该具体显示的位置，防止超出屏幕边界
                Program.ApplicationRoot.TrayMenuWindow.SetWindowPosition(appbarData, monitorInfo, pt);

                Program.ApplicationRoot.TrayMenuWindow.AppWindow.Show();
                User32Library.SetForegroundWindow(Program.ApplicationRoot.TrayMenuWindow.Handle);
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
                Program.ApplicationRoot.MainWindow.Presenter.Maximize();
            }
            else
            {
                if (WindowWidth.HasValue && WindowHeight.HasValue && WindowPositionXAxis.HasValue && WindowPositionYAxis.HasValue)
                {
                    Program.ApplicationRoot.MainWindow.AppWindow.MoveAndResize(new RectInt32(
                        WindowPositionXAxis.Value,
                        WindowPositionYAxis.Value,
                        WindowWidth.Value,
                        WindowHeight.Value
                        ));
                }
            }

            SetAppIcon();
            Program.ApplicationRoot.MainWindow.AppWindow.Show();
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
            for (int index = 0; index < Program.ApplicationRoot.TaskbarJumpList.Items.Count; index++)
            {
                if (Program.ApplicationRoot.TaskbarJumpList.Items[index].RemovedByUser)
                {
                    Program.ApplicationRoot.TaskbarJumpList.Items.RemoveAt(index);
                    index--;
                }
            }
        }

        /// <summary>
        /// 更新跳转列表组名
        /// </summary>
        public void UpdateJumpListGroupName()
        {
            foreach (JumpListItem jumpListItem in Program.ApplicationRoot.TaskbarJumpList.Items)
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
                Program.ApplicationRoot.MainWindow.AppWindow.SetIcon(Win32Interop.GetIconIdFromIcon(hIcons[0]));
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
            Program.ApplicationRoot.TrayIcon.Dispose();
            Program.ApplicationRoot.TrayIcon.DoubleClick -= DoubleClick;
            Program.ApplicationRoot.TrayIcon.RightClick -= RightClick;
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }

        /// <summary>
        /// 关闭窗口时保存窗口的大小和位置信息
        /// </summary>
        private async Task SaveWindowInformationAsync()
        {
            await ConfigService.SaveSettingAsync(ConfigKey.IsWindowMaximizedKey, Program.ApplicationRoot.MainWindow.AppTitlebar.ViewModel.IsWindowMaximized);
            await ConfigService.SaveSettingAsync(ConfigKey.WindowWidthKey, Program.ApplicationRoot.MainWindow.AppWindow.Size.Width);
            await ConfigService.SaveSettingAsync(ConfigKey.WindowHeightKey, Program.ApplicationRoot.MainWindow.AppWindow.Size.Height);
            await ConfigService.SaveSettingAsync(ConfigKey.WindowPositionXAxisKey, Program.ApplicationRoot.MainWindow.AppWindow.Position.X);
            await ConfigService.SaveSettingAsync(ConfigKey.WindowPositionYAxisKey, Program.ApplicationRoot.MainWindow.AppWindow.Position.Y);
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
        ~WinUIAppViewModel()
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
