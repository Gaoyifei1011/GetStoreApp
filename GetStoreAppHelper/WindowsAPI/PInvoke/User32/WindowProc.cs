using System;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 事件处理函数是一个回调函数，Windows在窗体中发生须要处理的事件时，在主时间循环中调用该函数。
    /// 时间处理函数可处理不论什么想处理的事件，其它的事件传递给Windows进行处理。
    /// </summary>
    /// <param name="hWnd">发送方的窗体句柄</param>
    /// <param name="Msg">消息id，即消息的种类</param>
    /// <param name="wParam">具体信息</param>
    /// <param name="lParam">具体信息</param>
    public delegate IntPtr WindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);
}
