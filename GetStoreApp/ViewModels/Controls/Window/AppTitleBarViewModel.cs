using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.CustomControls.MenusAndToolbars;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.Graphics;

namespace GetStoreApp.ViewModels.Controls.Window
{
    /// <summary>
    /// 应用标题栏用户控件视图模型
    /// </summary>
    public sealed class AppTitleBarViewModel : ViewModelBase
    {
        private bool isWindowMoving = false;
        private int nX = 0;
        private int nY = 0;
        private int nXWindow = 0;
        private int nYWindow = 0;

        public string MaximizeToolTip { get; } = ResourceService.GetLocalized("Window/Maximize");

        public string RestoreToolTip { get; } = ResourceService.GetLocalized("Window/Restore");

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

        private string _isWindowMaximizedToolTip;

        public string IsWindowMaximizedToolTip
        {
            get { return _isWindowMaximizedToolTip; }

            set
            {
                _isWindowMaximizedToolTip = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 双击标题栏对应的事件（窗口最大化或还原窗口）
        /// </summary>
        public void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs args)
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
        /// 鼠标移动时对应的事件
        /// </summary>
        public void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            PointerPointProperties properties = args.GetCurrentPoint((Grid)sender).Properties;

            if (properties.IsLeftButtonPressed)
            {
                unsafe
                {
                    PointInt32 pt = new PointInt32();
                    User32Library.GetCursorPos(&pt);

                    if (isWindowMoving)
                    {
                        if (!WindowHelper.IsWindowMaximized)
                        {
                            User32Library.SetWindowPos(
                                Program.ApplicationRoot.MainWindow.GetMainWindowHandle(),
                                IntPtr.Zero,
                                nXWindow + (pt.X - nX),
                                nYWindow + (pt.Y - nY),
                                0,
                                0,
                                SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER
                                );
                        }
                    }
                }

                args.Handled = true;
            }
        }

        /// <summary>
        /// 鼠标点击时对应的事件
        /// </summary>
        public void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            PointerPointProperties properties = args.GetCurrentPoint((Grid)sender).Properties;

            if (properties.IsLeftButtonPressed)
            {
                ((Grid)sender).CapturePointer(args.Pointer);
                nXWindow = Program.ApplicationRoot.MainWindow.AppWindow.Position.X;
                nYWindow = Program.ApplicationRoot.MainWindow.AppWindow.Position.Y;

                unsafe
                {
                    PointInt32 pt = new PointInt32() { X = 0, Y = 0 };
                    User32Library.GetCursorPos(&pt);
                    nX = pt.X;
                    nY = pt.Y;
                    if (!WindowHelper.IsWindowMaximized)
                    {
                        isWindowMoving = true;
                    }
                }
            }
        }

        /// <summary>
        /// 鼠标释放时对应的事件
        /// </summary>
        public void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            ((Grid)sender).ReleasePointerCaptures();
            isWindowMoving = false;
        }

        /// <summary>
        /// 窗口还原
        /// </summary>
        public void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            WindowHelper.RestoreAppWindow();
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        public void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            if (button.Tag is not null)
            {
                ((AdaptiveMenuFlyout)button.Tag).Hide();
                User32Library.PostMessage(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), WindowMessage.WM_SYSCOMMAND, (int)SystemCommand.SC_MOVE, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        public void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            if (button.Tag is not null)
            {
                ((AdaptiveMenuFlyout)button.Tag).Hide();
                User32Library.PostMessage(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), WindowMessage.WM_SYSCOMMAND, (int)SystemCommand.SC_SIZE, 0);
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
        /// 窗口关闭
        /// </summary>
        public void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.Close();
        }
    }
}
