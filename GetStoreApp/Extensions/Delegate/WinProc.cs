using System;
using static PInvoke.User32;

namespace GetStoreApp.Extensions.Delegate
{
    public delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);
}
