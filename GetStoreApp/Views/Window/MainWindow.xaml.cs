using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.System;
using WinRT.Interop;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WinUIWindow
    {
        private WindowProc newWndProc = null;
        private IntPtr oldWndProc = IntPtr.Zero;

        public MainWindow()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = WindowFrame;
        }

        public void AppTitlebarLoaded(object sender, RoutedEventArgs args)
        {
            AppTitlebar.SetTitlebarState(WindowHelper.IsWindowMaximized);
        }

        /// <summary>
        /// 获取主窗口的窗口句柄
        /// </summary>
        public IntPtr GetMainWindowHandle()
        {
            IntPtr MainWindowHandle = WindowNative.GetWindowHandle(this);

            return MainWindowHandle != IntPtr.Zero
                ? MainWindowHandle
                : throw new ApplicationException(ResourceService.GetLocalized("Resources/WindowHandleInitializeFailed"));
        }

        /// <summary>
        /// 获取主窗口的XamlRoot
        /// </summary>
        public XamlRoot GetMainWindowXamlRoot()
        {
            return Content.XamlRoot is not null
                ? Content.XamlRoot
                : throw new ApplicationException(ResourceService.GetLocalized("Resources/WindowContentInitializeFailed"));
        }

        public void InitializeWindowProc()
        {
            IntPtr MainWindowHandle = GetMainWindowHandle();
            newWndProc = new WindowProc(NewWindowProc);
            oldWndProc = SetWindowLongAuto(MainWindowHandle, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
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
                        if (MinWidth >= 0)
                        {
                            minMaxInfo.ptMinTrackSize.X = ConvertEpxToPixel(hWnd, MinWidth);
                        }
                        if (MinHeight >= 0)
                        {
                            minMaxInfo.ptMinTrackSize.Y = ConvertEpxToPixel(hWnd, MinHeight);
                        }
                        if (MaxWidth > 0)
                        {
                            minMaxInfo.ptMaxTrackSize.X = ConvertEpxToPixel(hWnd, MaxWidth);
                        }
                        if (MaxHeight > 0)
                        {
                            minMaxInfo.ptMaxTrackSize.Y = ConvertEpxToPixel(hWnd, MaxHeight);
                        }
                        Marshal.StructureToPtr(minMaxInfo, lParam, true);
                        break;
                    }
                // 窗口接收其他数据消息
                case WindowMessage.WM_COPYDATA:
                    {
                        CopyDataStruct copyDataStruct = (CopyDataStruct)Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));

                        // 没有任何命令参数，正常启动，应用可能被重复启动
                        if (copyDataStruct.dwData is 0)
                        {
                            WindowHelper.ShowAppWindow();

                            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
                            {
                                await new AppRunningDialog().ShowAsync();
                            });
                        }
                        // 获取应用的命令参数
                        else
                        {
                            string[] startupArgs = copyDataStruct.lpData.Split(' ');
                            Messenger.Default.Send(startupArgs, MessageToken.Command);
                        }
                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(wParam.ToInt32() & 0xFFF0);

                        if (sysCommand == SystemCommand.SC_MOUSEMENU || sysCommand == SystemCommand.SC_KEYMENU)
                        {
                            AppTitlebar.ShowTitlebarMenu();
                            return 0;
                        }
                        break;
                    }
                case WindowMessage.WM_SIZE:
                    {
                        if ((SizeMode)wParam == SizeMode.SIZE_MAXIMIZED)
                        {
                            AppTitlebar.SetTitlebarState(true);
                        }
                        else if ((SizeMode)wParam == SizeMode.SIZE_RESTORED)
                        {
                            AppTitlebar.SetTitlebarState(false);
                        }
                        break;
                    }
                case WindowMessage.WM_TASKBARRCLICK:
                    {
                        WindowHelper.BringToFront();
                        return 0;
                    }
            }
            return User32Library.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }

        public static int ConvertEpxToPixel(IntPtr hwnd, int effectivePixels)
        {
            float scalingFactor = GetScalingFactor(hwnd);
            return Convert.ToInt32(effectivePixels * scalingFactor);
        }

        public static float GetScalingFactor(IntPtr hwnd)
        {
            int dpi = User32Library.GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            return scalingFactor;
        }
    }
}
