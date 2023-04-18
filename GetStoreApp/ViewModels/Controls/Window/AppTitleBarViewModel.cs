using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.CustomControls.MenusAndToolbars;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
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

        // 窗口还原
        public IRelayCommand RestoreCommand => new RelayCommand(WindowHelper.RestoreAppWindow);

        // 窗口移动
        public IRelayCommand MoveCommand => new RelayCommand<AdaptiveMenuFlyout>((flyout) =>
        {
            if (flyout is not null)
            {
                flyout.Hide();
                User32Library.PostMessage(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), WindowMessage.WM_SYSCOMMAND, (int)SystemCommand.SC_MOVE, 0);
            }
        });

        // 窗口大小
        public IRelayCommand SizeCommand => new RelayCommand<AdaptiveMenuFlyout>((flyout) =>
        {
            if (flyout is not null)
            {
                flyout.Hide();
                User32Library.PostMessage(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), WindowMessage.WM_SYSCOMMAND, (int)SystemCommand.SC_SIZE, 0);
            }
        });

        // 窗口最小化
        public IRelayCommand MinimizeCommand => new RelayCommand(WindowHelper.MinimizeAppWindow);

        // 窗口最大化
        public IRelayCommand MaximizeCommand => new RelayCommand(() =>
        {
            if (WindowHelper.IsWindowMaximized)
            {
                WindowHelper.RestoreAppWindow();
            }
            else
            {
                WindowHelper.MaximizeAppWindow();
            }
        });

        // 窗口关闭
        public IRelayCommand CloseCommand => new RelayCommand(Program.ApplicationRoot.MainWindow.Close);

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
                    PointInt32 pointInt32 = new PointInt32() { X = 0, Y = 0 };
                    PointInt32* pointPtr = &pointInt32;
                    User32Library.GetCursorPos(pointPtr);

                    if (isWindowMoving)
                    {
                        if (!WindowHelper.IsWindowMaximized)
                        {
                            User32Library.SetWindowPos(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), 0, nXWindow + (pointInt32.X - nX), nYWindow + (pointInt32.Y - nY), 0, 0, SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
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
                    PointInt32 pointInt32 = new PointInt32() { X = 0, Y = 0 };
                    PointInt32* pointPtr = &pointInt32;
                    User32Library.GetCursorPos(pointPtr);
                    nX = pointInt32.X;
                    nY = pointInt32.Y;
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
    }
}
