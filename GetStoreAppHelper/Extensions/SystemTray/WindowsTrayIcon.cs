using GetStoreAppHelper.Helpers.Root;
using GetStoreAppHelper.Services;
using GetStoreAppHelper.UI.Controls;
using GetStoreAppHelper.WindowsAPI.PInvoke.User32;
using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.Graphics;

namespace GetStoreAppHelper.Extensions.SystemTray
{
    /// <summary>
    /// 应用程序的托盘图标
    /// </summary>
    public class WindowsTrayIcon : IDisposable
    {
        private NotifyIcon WindowsNotifyIcon { get; set; } = new NotifyIcon();

        private IntPtr[] hIcons;

        private bool isDisposed;

        public WindowsTrayIcon()
        {
            WindowsNotifyIcon.Text = ResourceService.GetLocalized("HelperResources/AppName");
            WindowsNotifyIcon.Icon = Icon.FromHandle(LoadIcon(string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), "GetStoreApp.exe")));

            WindowsNotifyIcon.MouseDoubleClick += NotifyIconDoubleClick;
            WindowsNotifyIcon.MouseClick += NotifyIconClick;
        }

        /// <summary>
        /// 托盘图标的左键/右键单击事件
        /// </summary>
        private void NotifyIconClick(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Right)
            {
                if (Program.ApplicationRoot.MainWindow.IsWindowCreated && Program.ApplicationRoot.MainWindow.Content is not null)
                {
                    User32Library.GetCursorPos(out PointInt32 CurrentPoint);
                    User32Library.SetForegroundWindow(Program.ApplicationRoot.MainWindow.Handle);

                    (Program.ApplicationRoot.MainWindow.Content as TrayMenuControl).SetXamlRoot(Program.ApplicationRoot.MainWindow.Content.XamlRoot);
                    (Program.ApplicationRoot.MainWindow.Content as TrayMenuControl).ShowMenuFlyout(CurrentPoint);
                }
            }
        }

        /// <summary>
        /// 托盘图标的左键双击单击事件
        /// </summary>
        private void NotifyIconDoubleClick(object sender, MouseEventArgs args)
        {
            if (Program.ApplicationRoot.MainWindow.IsWindowCreated && Program.ApplicationRoot.MainWindow.Content is not null)
            {
                (Program.ApplicationRoot.MainWindow.Content as TrayMenuControl).ViewModel.ShowOrHideWindowCommand.Execute(null);
            }
        }

        /// <summary>
        /// 设置托盘图标的显示状态
        /// </summary>
        public void SetState(bool visable)
        {
            WindowsNotifyIcon.Visible = visable;
        }

        /// <summary>
        /// 从exe应用程序中加载图标文件
        /// </summary>
        private IntPtr LoadIcon(string exeFile)
        {
            // 选中文件中的图标总数
            int iconTotalCount = User32Library.PrivateExtractIcons(exeFile, 0, 0, 0, null, null, 0, 0);

            // 用于接收获取到的图标指针
            hIcons = new IntPtr[iconTotalCount];

            // 对应的图标id
            int[] ids = new int[iconTotalCount];

            // 成功获取到的图标个数
            int successCount = User32Library.PrivateExtractIcons(exeFile, 0, 16, 16, hIcons, ids, iconTotalCount, 0);

            // GetStoreApp.exe 应用程序只有一个图标，返回该应用程序的图标句柄
            if (successCount >= 1 && hIcons[0] != IntPtr.Zero)
            {
                return hIcons[0];
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// 删除托盘图标，释放资源
        /// </summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (WindowsNotifyIcon.Visible)
                    {
                        WindowsNotifyIcon.Visible = false;
                    }
                    WindowsNotifyIcon.Dispose();
                    foreach (IntPtr icon in hIcons)
                    {
                        User32Library.DestroyIcon(icon);
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                isDisposed = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~WindowsTrayIcon()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }
    }
}
