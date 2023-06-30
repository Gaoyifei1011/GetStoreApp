using GetStoreApp.Helpers.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.CustomControls.MenusAndToolbars;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Graphics;

namespace GetStoreApp.ViewModels.Controls.Window
{
    /// <summary>
    /// 应用标题栏用户控件视图模型
    /// </summary>
    public sealed class AppTitleBarViewModel : ViewModelBase
    {
        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                _isWindowMaximized = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        public void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.Close();
        }

        /// <summary>
        /// 初始化自定义标题栏
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            Grid appTitleBar = sender as Grid;
            if (appTitleBar is not null)
            {
                SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);
            }
            IsWindowMaximized = WindowHelper.IsWindowMaximized;
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        public void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            if (WindowHelper.IsWindowMaximized)
            {
                WindowHelper.RestoreAppWindow();
            }
            else
            {
                WindowHelper.MaximizeAppWindow();
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        public void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            WindowHelper.MinimizeAppWindow();
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        public void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((AdaptiveMenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), WindowMessage.WM_SYSCOMMAND, 0xF010, 0);
            }
        }

        /// <summary>
        /// 窗口还原
        /// </summary>
        public void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            WindowHelper.RestoreAppWindow();
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        public void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((AdaptiveMenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), WindowMessage.WM_SYSCOMMAND, 0xF000, 0);
            }
        }

        /// <summary>
        /// 控件大小发生变化时，修改拖动区域
        /// </summary>
        public void OnSizeChanged(object sender, RoutedEventArgs args)
        {
            Grid appTitleBar = sender as Grid;
            if (appTitleBar is not null)
            {
                SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);
            }
        }

        /// <summary>
        /// 设置标题栏的可拖动区域
        /// </summary>
        public void SetDragRectangles(int leftMargin, double actualWidth, double actualHeight)
        {
            Program.ApplicationRoot.MainWindow.AppWindow.TitleBar.SetDragRectangles(new RectInt32[] { new RectInt32(
                leftMargin,
                0,
                DPICalcHelper.ConvertEpxToPixel(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), Convert.ToInt32(actualWidth)),
                DPICalcHelper.ConvertEpxToPixel(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), Convert.ToInt32(actualHeight))
                )});
        }
    }
}
