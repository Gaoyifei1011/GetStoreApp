using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Controls;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Windows.System;
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

        public ViewModelBase DataContext { get; set; }

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

        public ICommand ActivatedCommand
        {
            get { return (ICommand)Content.GetValue(ActivatedCommandProperty); }

            set { Content.SetValue(ActivatedCommandProperty, value); }
        }

        public static readonly DependencyProperty ActivatedCommandProperty = DependencyProperty.Register("ActivatedCommand", typeof(ICommand), typeof(NavigationViewMenuItem), new PropertyMetadata(null));

        public ICommand ClosedCommand
        {
            get { return (ICommand)Content.GetValue(ClosedCommandProperty); }

            set { Content.SetValue(ClosedCommandProperty, value); }
        }

        public static readonly DependencyProperty ClosedCommandProperty = DependencyProperty.Register("ClosedCommand", typeof(ICommand), typeof(NavigationViewMenuItem), new PropertyMetadata(null));

        public ICommand SizeChangedCommand
        {
            get { return (ICommand)Content.GetValue(SizeChangedCommandProperty); }

            set { Content.SetValue(SizeChangedCommandProperty, value); }
        }

        public static readonly DependencyProperty SizeChangedCommandProperty = DependencyProperty.Register("SizeChangedCommand", typeof(ICommand), typeof(NavigationViewMenuItem), new PropertyMetadata(null));

        public WASDKWindow()
        {
            Activated += OnActivated;
            Closed += OnClosed;
            SizeChanged += OnSizeChanged;

            // 获取窗口的句柄
            Hwnd = WindowNative.GetWindowHandle(this);
            if (Hwnd == IntPtr.Zero)
            {
                throw new NullReferenceException(ResourceService.GetLocalized("Resources/WindowHandleInitializeFailed"));
            }
            newWndProc = new WinProc(NewWindowProc);
            oldWndProc = SetWindowLongPtr(Hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
        }

        ~WASDKWindow()
        {
            Activated -= OnActivated;
        }

        public void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            ActivatedCommand?.Execute(args);
        }

        public void OnClosed(object sender, WindowEventArgs args)
        {
            ClosedCommand?.Execute(args);
        }

        public void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            SizeChangedCommand?.Execute(args);
        }

        /// <summary>
        /// 更改指定窗口的属性
        /// </summary>
        private IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
        {
            if (InfoHelper.GetPackageArchitecture() == ProcessorArchitecture.X64)
            {
                return User32Library.SetWindowLongPtr(hWnd, nIndex, newProc);
            }
            else if (InfoHelper.GetPackageArchitecture() == ProcessorArchitecture.X86)
            {
                return User32Library.SetWindowLong(hWnd, nIndex, newProc);
            }
            else if (InfoHelper.GetPackageArchitecture() == ProcessorArchitecture.Arm64)
            {
                return User32Library.SetWindowLongPtr(hWnd, nIndex, newProc);
            }
            else
            {
                return IntPtr.Zero;
            }
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
                        Messenger.Default.Send(true, MessageToken.SystemSettingsChanged);
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

                            if (!Program.ApplicationRoot.IsDialogOpening)
                            {
                                Program.ApplicationRoot.IsDialogOpening = true;
                                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
                                {
                                    await new AppRunningDialog().ShowAsync();
                                    Program.ApplicationRoot.IsDialogOpening = false;
                                });
                            }
                        }
                        // 获取应用的命令参数
                        else
                        {
                            string[] startupArgs = copyDataStruct.lpData.Split(' ');
                            Messenger.Default.Send(startupArgs, MessageToken.Command);
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
            User32Library.GetWindowRect(hwnd, out RECT rc);
            return rc.right - rc.left;
        }

        /// <summary>
        /// 获取Win32窗口高度
        /// </summary>
        private int GetHeightWin32(IntPtr hwnd)
        {
            //Get the width
            User32Library.GetWindowRect(hwnd, out RECT rc);
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
