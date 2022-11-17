using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Messages;
using GetStoreApp.UI.Dialogs.ContentDialogs.Common;
using GetStoreAppWindowsAPI.PInvoke.User32;
using GetStoreAppWindowsAPI.PInvoke.WindowsCore;
using Microsoft.UI.Dispatching;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// Windows 应用 SDK窗口的扩展
    /// </summary>
    public class WASDKWindow : Microsoft.UI.Xaml.Window
    {
        private WinProc newWndProc = null;
        private IntPtr oldWndProc = IntPtr.Zero;

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// 窗口宽度
        /// </summary>
        public int Width
        {
            get { return ConvertPixelToEpx(Hwnd, GetWidthWin32(Hwnd)); }
            set { SetWindowWidthWin32(Hwnd, ConvertEpxToPixel(Hwnd, value)); }
        }

        /// <summary>
        /// 窗口高度
        /// </summary>
        public int Height
        {
            get { return ConvertPixelToEpx(Hwnd, GetHeightWin32(Hwnd)); }
            set { SetWindowHeightWin32(Hwnd, ConvertEpxToPixel(Hwnd, value)); }
        }

        /// <summary>
        /// 窗口标题
        /// </summary>
        public new string Title
        {
            get => base.Title;
            set => base.Title = value;
        }

        /// <summary>
        /// 是否扩展内容到标题栏
        /// </summary>
        public new bool ExtendsContentIntoTitleBar
        {
            get => base.ExtendsContentIntoTitleBar;
            set => base.ExtendsContentIntoTitleBar = value;
        }

        /// <summary>
        /// 窗口最小宽度
        /// </summary>
        public int MinWidth { get; set; } = -1;

        /// <summary>
        /// 窗口最小高度
        /// </summary>
        public int MinHeight { get; set; } = -1;

        /// <summary>
        /// 窗口最大宽度
        /// </summary>
        public int MaxWidth { get; set; } = -1;

        /// <summary>
        /// 窗口最大高度
        /// </summary>
        public int MaxHeight { get; set; } = -1;

        /// <summary>
        /// 窗口是否关闭的标志
        /// </summary>
        public bool IsClosing { get; set; }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        private IntPtr Hwnd { get; set; } = IntPtr.Zero;

        public WASDKWindow()
        {
            // 获取窗口的句柄
            Hwnd = WindowNative.GetWindowHandle(this);
            if (Hwnd == IntPtr.Zero)
            {
                throw new NullReferenceException("窗口句柄不能为空");
            }
            newWndProc = new WinProc(NewWindowProc);
            oldWndProc = SetWindowLongPtr(Hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
        }

        /// <summary>
        /// 更改指定窗口的属性
        /// </summary>
        private IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
        {
            if (IntPtr.Size == 8)
                return User32Library.SetWindowLongPtr64(hWnd, nIndex, newProc);
            else
                return User32Library.SetWindowLongPtr32(hWnd, nIndex, newProc);
        }

        /// <summary>
        /// 窗口消息处理
        /// </summary>
        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 系统设置发生更改时的消息
                case WindowMessage.WM_SETTINGCHANGE:
                    {
                        WeakReferenceMessenger.Default.Send(new SystemSettingsChnagedMessage(true));
                        break;
                    }
                case WindowMessage.WM_GETMINMAXINFO:
                    {
                        MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                        if (MinWidth >= 0)
                        {
                            minMaxInfo.ptMinTrackSize.x = ConvertEpxToPixel(hWnd, MinWidth);
                        }
                        if (MinHeight >= 0)
                        {
                            minMaxInfo.ptMinTrackSize.y = ConvertEpxToPixel(hWnd, MinHeight);
                        }
                        if (MaxWidth > 0)
                        {
                            minMaxInfo.ptMaxTrackSize.x = ConvertEpxToPixel(hWnd, MaxWidth);
                        }
                        if (MaxHeight > 0)
                        {
                            minMaxInfo.ptMaxTrackSize.y = ConvertEpxToPixel(hWnd, MaxHeight);
                        }
                        Marshal.StructureToPtr(minMaxInfo, lParam, true);
                        break;
                    }
                // 窗口接受其他数据消息
                case WindowMessage.WM_COPYDATA:
                    {
                        CopyDataStruct copyDataStruct = (CopyDataStruct)Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));

                        // 没有任何命令参数，正常启动，应用可能被重复启动
                        if (copyDataStruct.dwData == 0)
                        {
                            WindowHelper.ShowAppWindow();

                            if (!App.IsDialogOpening)
                            {
                                App.IsDialogOpening = true;
                                dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
                                {
                                    await new AppRunningDialog().ShowAsync();
                                    App.IsDialogOpening = false;
                                });
                            }
                        }
                        // 获取应用的命令参数
                        else
                        {
                            string[] startupArgs = copyDataStruct.lpData.Split(' ');
                            WeakReferenceMessenger.Default.Send(new CommandMessage(startupArgs));
                        }
                        break;
                    }
            }
            return User32Library.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 获取Win32窗口宽度
        /// </summary>
        private int GetWidthWin32(IntPtr hwnd)
        {
            //Get the width
            RECT rc;
            User32Library.GetWindowRect(hwnd, out rc);
            return rc.right - rc.left;
        }

        /// <summary>
        /// 获取Win32窗口高度
        /// </summary>
        private int GetHeightWin32(IntPtr hwnd)
        {
            //Get the width
            RECT rc;
            User32Library.GetWindowRect(hwnd, out rc);
            return rc.bottom - rc.top;
        }

        /// <summary>
        /// 设置Win32窗口宽度
        /// </summary>
        private void SetWindowWidthWin32(IntPtr hwnd, int width)
        {
            int currentHeightInPixels = GetHeightWin32(hwnd);

            User32Library.SetWindowPos(hwnd, SpecialWindowHandles.HWND_TOP,
                                        0, 0, width, currentHeightInPixels,
                                        SetWindowPosFlags.SWP_NOMOVE |
                                        SetWindowPosFlags.SWP_NOACTIVATE);
        }

        /// <summary>
        /// 设置Win32窗口高度
        /// </summary>
        private void SetWindowHeightWin32(IntPtr hwnd, int height)
        {
            int currentWidthInPixels = GetWidthWin32(hwnd);

            User32Library.SetWindowPos(hwnd, SpecialWindowHandles.HWND_TOP,
                                        0, 0, currentWidthInPixels, height,
                                        SetWindowPosFlags.SWP_NOMOVE |
                                        SetWindowPosFlags.SWP_NOACTIVATE);
        }

        public static int ConvertEpxToPixel(IntPtr hwnd, int effectivePixels)
        {
            float scalingFactor = GetScalingFactor(hwnd);
            return (int)(effectivePixels * scalingFactor);
        }

        public static int ConvertPixelToEpx(IntPtr hwnd, int pixels)
        {
            float scalingFactor = GetScalingFactor(hwnd);
            return (int)(pixels / scalingFactor);
        }

        public static float GetScalingFactor(IntPtr hwnd)
        {
            int dpi = User32Library.GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            return scalingFactor;
        }
    }
}
