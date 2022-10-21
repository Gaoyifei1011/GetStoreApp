using System;
using static PInvoke.User32;

namespace GetStoreApp.Extensions.DataType.Delegate
{
    public delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);
}
