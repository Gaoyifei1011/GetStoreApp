using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using WinRT;

namespace GetStoreApp.UI.Controls.Window
{
    /// <summary>
    /// 应用标题栏控件
    /// </summary>
    public sealed partial class AppTitleBarControl : Grid, INotifyPropertyChanged
    {
        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                _isWindowMaximized = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWindowMaximized)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AppTitleBarControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        public void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.Dispose();
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        public void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.MaximizeOrRestore();
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        public void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.Minimize();
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        public void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender.As<MenuFlyoutItem>();
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage(Program.ApplicationRoot.MainWindow.Handle, WindowMessage.WM_SYSCOMMAND, 0xF010, 0);
            }
        }

        /// <summary>
        /// 窗口还原
        /// </summary>
        public void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.MaximizeOrRestore();
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        public void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender.As<MenuFlyoutItem>();
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage(Program.ApplicationRoot.MainWindow.Handle, WindowMessage.WM_SYSCOMMAND, 0xF000, 0);
            }
        }
    }
}
