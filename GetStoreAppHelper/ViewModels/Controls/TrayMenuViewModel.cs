using GetStoreAppHelper.Contracts.Command;
using GetStoreAppHelper.Extensions.Command;
using GetStoreAppHelper.Helpers;
using GetStoreAppHelper.Services;
using GetStoreAppHelper.ViewModels.Base;
using GetStoreAppHelper.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace GetStoreAppHelper.ViewModels.Controls
{
    /// <summary>
    /// 任务栏辅助部分：任务栏右键菜单浮出控件视图模型
    /// </summary>
    public class TrayMenuViewModel : ViewModelBase
    {
        private ElementTheme _commandBarTheme;

        public ElementTheme CommandBarTheme
        {
            get { return _commandBarTheme; }

            set
            {
                _commandBarTheme = value;
                OnPropertyChanged();
            }
        }

        // 项目主页
        public IRelayCommand ProjectDescriptionCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        });

        // 打开应用“关于”页面
        public IRelayCommand AboutAppCommand => new RelayCommand(() =>
        {
            Process[] GetStoreAppProcess = Process.GetProcessesByName("GetStoreApp");

            if (GetStoreAppProcess.Length > 0)
            {
                IntPtr hwnd = IntPtr.Zero;
                do
                {
                    hwnd = User32Library.FindWindowEx(IntPtr.Zero, hwnd, "WinUIDesktopWin32WindowClass", null);

                    if (hwnd != IntPtr.Zero)
                    {
                        User32Library.GetWindowThreadProcessId(hwnd, out int processId);

                        if (processId is not 0)
                        {
                            bool result = false;
                            foreach (Process process in GetStoreAppProcess)
                            {
                                if (process.Id == processId)
                                {
                                    User32Library.PostMessage(hwnd, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.AboutApp), IntPtr.Zero);
                                    result = true;
                                    break;
                                }
                            }

                            if (result) break;
                        }
                    }
                }
                while (hwnd != IntPtr.Zero);
            }
        });

        // 显示 / 隐藏窗口
        public IRelayCommand ShowOrHideWindowCommand => new RelayCommand(() =>
        {
            Process[] GetStoreAppProcess = Process.GetProcessesByName("GetStoreApp");

            if (GetStoreAppProcess.Length > 0)
            {
                IntPtr hwnd = IntPtr.Zero;
                do
                {
                    hwnd = User32Library.FindWindowEx(IntPtr.Zero, hwnd, "WinUIDesktopWin32WindowClass", null);

                    if (hwnd != IntPtr.Zero)
                    {
                        User32Library.GetWindowThreadProcessId(hwnd, out int processId);

                        if (processId is not 0)
                        {
                            bool result = false;
                            foreach (Process process in GetStoreAppProcess)
                            {
                                if (process.Id == processId)
                                {
                                    User32Library.PostMessage(hwnd, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.ShowOrHideWindow), IntPtr.Zero);
                                    result = true;
                                    break;
                                }
                            }

                            if (result) break;
                        }
                    }
                }
                while (hwnd != IntPtr.Zero);
            }
        });

        // 打开设置
        public IRelayCommand SettingsCommand => new RelayCommand(() =>
        {
            Process[] GetStoreAppProcess = Process.GetProcessesByName("GetStoreApp");

            if (GetStoreAppProcess.Length > 0)
            {
                IntPtr hwnd = IntPtr.Zero;
                do
                {
                    hwnd = User32Library.FindWindowEx(IntPtr.Zero, hwnd, "WinUIDesktopWin32WindowClass", null);

                    if (hwnd != IntPtr.Zero)
                    {
                        User32Library.GetWindowThreadProcessId(hwnd, out int processId);

                        if (processId is not 0)
                        {
                            bool result = false;
                            foreach (Process process in GetStoreAppProcess)
                            {
                                if (process.Id == processId)
                                {
                                    User32Library.PostMessage(hwnd, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.Settings), IntPtr.Zero);
                                    result = true;
                                    break;
                                }
                            }

                            if (result) break;
                        }
                    }
                }
                while (hwnd != IntPtr.Zero);
            }
        });

        // 退出应用
        public IRelayCommand ExitCommand => new RelayCommand(() =>
        {
            Process[] GetStoreAppProcess = Process.GetProcessesByName("GetStoreApp");

            if (GetStoreAppProcess.Length > 0)
            {
                IntPtr hwnd = IntPtr.Zero;
                do
                {
                    hwnd = User32Library.FindWindowEx(IntPtr.Zero, hwnd, "WinUIDesktopWin32WindowClass", null);

                    if (hwnd != IntPtr.Zero)
                    {
                        User32Library.GetWindowThreadProcessId(hwnd, out int processId);

                        if (processId is not 0)
                        {
                            bool result = false;
                            foreach (Process process in GetStoreAppProcess)
                            {
                                if (process.Id == processId)
                                {
                                    User32Library.PostMessage(hwnd, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.Exit), IntPtr.Zero);
                                    result = true;
                                    break;
                                }
                            }

                            if (result) break;
                        }
                    }
                }
                while (hwnd != IntPtr.Zero);
            }
            else
            {
                Program.ApplicationRoot.CloseApp();
            }
        });

        /// <summary>
        /// 命令栏控件在即将打开的时候获取应用设置相应的主题值
        /// </summary>
        public async void OnOpened(object sender, object args)
        {
            await ThemeService.LoadNotifyIconMenuThemeAsync();

            if (ThemeService.NotifyIconMenuTheme == ThemeService.NotifyIconMenuThemeList[0])
            {
                await ThemeService.LoadThemeAsync();
                if (ThemeService.AppTheme == Convert.ToString(ElementTheme.Default))
                {
                    if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                    {
                        CommandBarTheme = ElementTheme.Light;
                    }
                    else
                    {
                        CommandBarTheme = ElementTheme.Dark;
                    }
                }
                else if (ThemeService.AppTheme == Convert.ToString(ElementTheme.Light))
                {
                    CommandBarTheme = ElementTheme.Light;
                }
                else if (ThemeService.AppTheme == Convert.ToString(ElementTheme.Dark))
                {
                    CommandBarTheme = ElementTheme.Dark;
                }
            }
            else
            {
                if (RegistryHelper.GetRegistrySystemTheme() == ElementTheme.Light)
                {
                    CommandBarTheme = ElementTheme.Light;
                }
                else
                {
                    CommandBarTheme = ElementTheme.Dark;
                }
            }
        }

        /// <summary>
        /// 命令栏显示时设置相应的主题
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            IReadOnlyList<Popup> PopupRootList = VisualTreeHelper.GetOpenPopupsForXamlRoot(Program.ApplicationRoot.MainWindow.Content.XamlRoot);

            foreach (Popup popupItem in PopupRootList)
            {
                if (popupItem.Child is FlyoutPresenter)
                {
                    (popupItem.Child as FlyoutPresenter).RequestedTheme = CommandBarTheme;
                }
            }
        }
    }
}
