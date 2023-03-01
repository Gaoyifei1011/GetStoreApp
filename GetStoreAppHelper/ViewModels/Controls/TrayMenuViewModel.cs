using GetStoreAppHelper.Contracts.Command;
using GetStoreAppHelper.Extensions.Command;
using GetStoreAppHelper.Helpers;
using GetStoreAppHelper.Services;
using GetStoreAppHelper.WindowsAPI.PInvoke.User32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppHelper.ViewModels.Controls
{
    public class TrayMenuViewModel
    {
        // 项目主页
        public IRelayCommand ProjectDescriptionCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        });

        // 打开应用“关于”页面
        public IRelayCommand AboutAppCommand => new RelayCommand(() =>
        {
            bool FindResult = false;
            Process[] GetStoreAppProcess = Process.GetProcessesByName("GetStoreApp");

            if (GetStoreAppProcess.Length > 0)
            {
                foreach (Process process in GetStoreAppProcess)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        User32Library.SendMessage(process.MainWindowHandle, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.AboutApp), IntPtr.Zero);
                        FindResult = true;
                    }
                }

                if (!FindResult)
                {
                    IntPtr hwnd = FindWindow(null, "获取商店");
                    User32Library.SendMessage(hwnd, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.AboutApp), IntPtr.Zero);
                }
            }
        });

        // 显示 / 隐藏窗口
        public IRelayCommand ShowOrHideWindowCommand => new RelayCommand(() =>
        {
            bool FindResult = false;
            Process[] GetStoreAppProcess = Process.GetProcessesByName("GetStoreApp");

            if (GetStoreAppProcess.Length > 0)
            {
                foreach (Process process in GetStoreAppProcess)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        User32Library.SendMessage(process.MainWindowHandle, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.ShowOrHideWindow), IntPtr.Zero);
                    }
                }

                if (!FindResult)
                {
                    IntPtr hwnd = FindWindow(null, "获取商店");
                    User32Library.SendMessage(hwnd, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.ShowOrHideWindow), IntPtr.Zero);
                }
            }
        });

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // 打开设置
        public IRelayCommand SettingsCommand => new RelayCommand(() =>
        {
            bool FindResult = false;
            Process[] GetStoreAppProcess = Process.GetProcessesByName("GetStoreApp");

            if (GetStoreAppProcess.Length > 0)
            {
                foreach (Process process in GetStoreAppProcess)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        User32Library.SendMessage(process.MainWindowHandle, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.Settings), IntPtr.Zero);
                        FindResult = true;
                    }
                }

                if (!FindResult)
                {
                    IntPtr hwnd = FindWindow(null, "获取商店");
                    User32Library.SendMessage(hwnd, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.Settings), IntPtr.Zero);
                }
            }
        });

        // 退出应用
        public IRelayCommand ExitCommand => new RelayCommand(() =>
        {
            bool FindResult = false;
            Process[] GetStoreAppProcess = Process.GetProcessesByName("GetStoreApp");

            if (GetStoreAppProcess.Length > 0)
            {
                foreach (Process process in GetStoreAppProcess)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        User32Library.SendMessage(process.MainWindowHandle, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.Exit), IntPtr.Zero);
                    }
                }

                if (!FindResult)
                {
                    IntPtr hwnd = FindWindow(null, "获取商店");
                    User32Library.SendMessage(hwnd, WindowMessage.WM_PROCESSCOMMUNICATION, Convert.ToInt32(CommunicationFlags.Exit), IntPtr.Zero);
                }
            }
        });

        public void OnOpened(object sender, object args)
        {
            if (ThemeService.AppTheme == Convert.ToString(ElementTheme.Default))
            {
                if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                {
                    (sender as MenuFlyout).MenuFlyoutPresenterStyle = ResourceDictionaryHelper.MenuFlyoutResourceDict["AdaptiveFlyoutLightPresenter"] as Style;
                }
                else
                {
                    (sender as MenuFlyout).MenuFlyoutPresenterStyle = ResourceDictionaryHelper.MenuFlyoutResourceDict["AdaptiveFlyoutDarkPresenter"] as Style;
                }
            }
            else if (ThemeService.AppTheme == Convert.ToString(ElementTheme.Light))
            {
                (sender as MenuFlyout).MenuFlyoutPresenterStyle = ResourceDictionaryHelper.MenuFlyoutResourceDict["AdaptiveFlyoutLightPresenter"] as Style;
            }
            else if (ThemeService.AppTheme == Convert.ToString(ElementTheme.Dark))
            {
                (sender as MenuFlyout).MenuFlyoutPresenterStyle = ResourceDictionaryHelper.MenuFlyoutResourceDict["AdaptiveFlyoutDarkPresenter"] as Style;
            }
        }
    }
}
