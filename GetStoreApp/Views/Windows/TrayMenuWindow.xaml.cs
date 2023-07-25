using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.System;
using WinRT;
using WinRT.Interop;

namespace GetStoreApp.Views.Windows
{
    /// <summary>
    /// 应用任务栏右键菜单窗口
    /// </summary>
    public sealed partial class TrayMenuWindow : Window, INotifyPropertyChanged
    {
        private WNDPROC newWndProc = null;
        private IntPtr oldWndProc = IntPtr.Zero;

        public IntPtr Handle { get; }

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                _windowTheme = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TrayMenuWindow()
        {
            InitializeComponent();
            Handle = WindowNative.GetWindowHandle(this);

            newWndProc = new WNDPROC(NewWindowProc);
            oldWndProc = SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProc));

            unchecked { SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE, (IntPtr)WindowStyle.WS_POPUPWINDOW); }
            SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_EXSTYLE, (IntPtr)WindowStyleEx.WS_EX_TOOLWINDOW);

            // 调整窗口的大小
            AppWindow.MoveAndResize(new RectInt32(-1, -1, 1, 1));
            AppWindow.Presenter.As<OverlappedPresenter>().IsAlwaysOnTop = true;
            AppWindow.Show();
        }

        /// <summary>
        /// 打开应用的项目主页
        /// </summary>
        public async void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        }

        /// <summary>
        /// 打开应用“关于”页面
        /// </summary>
        public void OnAboutAppClicked(object sender, RoutedEventArgs args)
        {
            // 窗口置前端
            Program.ApplicationRoot.MainWindow.Show();

            if (NavigationService.GetCurrentPageType() != typeof(AboutPage))
            {
                NavigationService.NavigateTo(typeof(AboutPage));
            }
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        public void OnExitClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.Dispose();
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        public void OnSettingsClicked(object sender, RoutedEventArgs args)
        {
            // 窗口置前端
            Program.ApplicationRoot.MainWindow.Show();

            if (NavigationService.GetCurrentPageType() != typeof(SettingsPage))
            {
                NavigationService.NavigateTo(typeof(SettingsPage));
            }
        }

        /// <summary>
        /// 显示 / 隐藏窗口
        /// </summary>
        public void OnShowOrHideWindowClicked(object sender, RoutedEventArgs args)
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
        }

        /// <summary>
        /// 更改指定窗口的窗口属性
        /// </summary>
        private IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 窗口消息处理
        /// </summary>
        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 窗口大小发生更改时的消息
                case WindowMessage.WM_GETMINMAXINFO:
                    {
                        MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                        minMaxInfo.ptMinTrackSize.X = DPICalcHelper.ConvertEpxToPixel(hWnd, 0);
                        minMaxInfo.ptMinTrackSize.Y = DPICalcHelper.ConvertEpxToPixel(hWnd, 0);
                        Marshal.StructureToPtr(minMaxInfo, lParam, true);
                        break;
                    }
                // 系统设置选项发生更改时的消息
                case WindowMessage.WM_SETTINGCHANGE:
                    {
                        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                        {
                            if (ThemeService.NotifyIconMenuTheme == ThemeService.NotifyIconMenuThemeList[1])
                            {
                                WindowTheme = RegistryHelper.GetSystemUsesTheme();
                            }
                        });
                        break;
                    }
            }
            return User32Library.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }
    }
}
