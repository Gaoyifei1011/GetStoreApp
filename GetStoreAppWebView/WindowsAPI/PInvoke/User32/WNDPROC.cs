using System;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 一个回调函数，可在应用程序中定义，用于处理发送到窗口的消息。 WNDPROC 类型定义指向此回调函数的指针。 WndProc 名称是应用程序中定义的函数名称的占位符。
    /// </summary>
    /// <param name="hWnd">窗口的句柄。</param>
    /// <param name="msg">有关系统提供的消息的列表，请参阅 系统定义的消息。</param>
    /// <param name="wParam">其他消息信息。 此参数通常名为 wParam。wParam 参数的内容取决于 uMsg 参数的值。</param>
    /// <param name="lParam">其他消息信息。 此参数通常名为 lParam。lParam 参数的内容取决于 uMsg 参数的值。</param>
    /// <returns>返回值是消息处理的结果，取决于发送的消息。</returns>
    public delegate IntPtr WNDPROC(IntPtr hWnd, WindowMessage msg, UIntPtr wParam, IntPtr lParam);
}
