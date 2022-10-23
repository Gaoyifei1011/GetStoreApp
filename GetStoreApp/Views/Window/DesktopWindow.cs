using GetStoreApp.Extensions.DataType.Delegate;
using GetStoreApp.Extensions.DataType.Event;
using System;
using System.Runtime.InteropServices;
using WinUIEx;
using static PInvoke.User32;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 扩展Windows 应用 SDK的Window类和WinUIEx类，在Desktop类中添加Closing事件，实现关闭应用时能够弹出对话框
    /// </summary>
    public class DesktopWindow : WindowEx
    {
        private WinProc newWndProc = null;
        private IntPtr oldWndProc = IntPtr.Zero;
        private IntPtr _hwnd = IntPtr.Zero;

        public bool IsClosing { get; set; }

        public IntPtr Hwnd
        {
            get { return _hwnd; }
        }

        public event EventHandler<WindowClosingEventArgs> Closing;

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        [DllImport("user32.dll")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        public DesktopWindow()
        {
            SubClassingWin32();
        }

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, newProc);
            else
                return SetWindowLongPtr32(hWnd, nIndex, newProc);
        }

        private void SubClassingWin32()
        {
            //Get the Window's HWND
            _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            if (_hwnd == IntPtr.Zero)
            {
                throw new NullReferenceException("The Window Handle is null.");
            }
            newWndProc = new WinProc(NewWindowProc);
            oldWndProc = SetWindowLongPtr(_hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
        }

        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                case WindowMessage.WM_CLOSE:

                    //If there is a Closing event handler and the close message wasn't send via
                    //this event (that set IsClosing=true), the message is ignored.
                    if (Closing is not null)
                    {
                        if (IsClosing == false)
                        {
                            OnClosing();
                        }
                        return IntPtr.Zero;
                    }
                    break;
            }
            return CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }

        private void OnClosing()
        {
            WindowClosingEventArgs windowClosingEventArgs = new(this);
            Closing.Invoke(this, windowClosingEventArgs);
        }
    }
}
