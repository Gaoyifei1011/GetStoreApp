using System;
using static PInvoke.User32;

namespace GetStoreApp.Extensions.DataType.Delegates
{
    public delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);
}
