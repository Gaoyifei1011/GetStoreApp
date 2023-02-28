using System;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// Windows API 用于提交窗口消息的回调委托。
    /// </summary>
    public delegate IntPtr WindowProcedureHandler(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
}
