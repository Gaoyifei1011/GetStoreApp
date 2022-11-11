using GetStoreApp.Extensions.DataType.Events;
using GetStoreAppWindowsAPI.PInvoke.User32;
using System;
using WinUIEx;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 为WindowEx窗口添加正在关闭窗口事件
    /// </summary>
    public class WASDKWindow : WindowEx
    {
        private WinProc newWndProc = null;
        private IntPtr oldWndProc = IntPtr.Zero;

        /// <summary>
        /// 是否扩展内容到标题栏
        /// </summary>
        public new bool ExtendsContentIntoTitleBar
        {
            get => base.ExtendsContentIntoTitleBar;
            set => base.ExtendsContentIntoTitleBar = value;
        }

        /// <summary>
        /// 窗口是否关闭的标志
        /// </summary>
        public bool IsClosing { get; set; }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        private IntPtr Hwnd { get; set; } = IntPtr.Zero;

        /// <summary>
        /// 窗口正在关闭事件
        /// </summary>
        public event EventHandler<WindowClosingEventArgs> Closing;

        public WASDKWindow()
        {
            // 获取窗口的句柄
            Hwnd = WindowExtensions.GetWindowHandle(this);
            if (Hwnd == IntPtr.Zero)
            {
                throw new NullReferenceException("The Window Handle is null.");
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
                return DllFunctions.SetWindowLongPtr64(hWnd, nIndex, newProc);
            else
                return DllFunctions.SetWindowLongPtr32(hWnd, nIndex, newProc);
        }

        /// <summary>
        /// 新窗口处理事件
        /// </summary>
        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                case WindowMessage.WM_CLOSE:
                    {
                        if (Closing is not null)
                        {
                            if (IsClosing == false)
                            {
                                WindowClosingEventArgs windowClosingEventArgs = new(this);
                                Closing.Invoke(this, windowClosingEventArgs);
                            }
                            return IntPtr.Zero;
                        }
                        break;
                    }
            }
            return DllFunctions.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }
    }
}
