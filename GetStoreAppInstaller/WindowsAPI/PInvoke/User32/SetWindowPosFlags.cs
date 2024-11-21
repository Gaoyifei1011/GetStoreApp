using System;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum SetWindowPosFlags : uint
    {
        /// <summary>
        /// 如果调用线程和拥有窗口的线程附加到不同的输入队列，系统会将请求发布到拥有该窗口的线程。
        /// 这可以防止调用线程阻止其执行，而其他线程处理请求。
        /// </summary>
        SWP_ASYNCWINDOWPOS = 0x4000,

        /// <summary>
        /// 阻止生成 WM_SYNCPAINT 消息。
        /// </summary>
        SWP_DEFERERASE = 0x2000,

        /// <summary>
        /// 绘制在窗口的类说明中定义的框架 () 窗口周围。
        /// </summary>
        SWP_DRAWFRAME = 0x0020,

        /// <summary>
        /// 使用 SetWindowLong 函数应用设置的新框架样式。 将 WM_NCCALCSIZE 消息发送到窗口，即使窗口的大小未更改也是如此。
        /// 如果未指定此标志，则仅当窗口的大小发生更改时， 才会发送WM_NCCALCSIZE 。
        /// </summary>
        SWP_FRAMECHANGED = 0x0020,

        /// <summary>
        /// 隐藏窗口。
        /// </summary>
        SWP_HIDEWINDOW = 0x0080,

        /// <summary>
        /// 不激活窗口。 如果未设置此标志，则会激活窗口，并根据 hWndInsertAfter 参数) 的设置 (将窗口移到最顶部或最顶层组的顶部。
        /// </summary>
        SWP_NOACTIVATE = 0x0010,

        /// <summary>
        /// 丢弃工作区的整个内容。 如果未指定此标志，则会在调整或重新定位窗口后保存并复制回工作区的有效内容。
        /// </summary>
        SWP_NOCOPYBITS = 0x0100,

        /// <summary>
        /// 保留当前位置 (忽略 X 和 Y 参数) 。
        /// </summary>
        SWP_NOMOVE = 0x0002,

        /// <summary>
        /// 不更改 Z 顺序中的所有者窗口位置。
        /// </summary>
        SWP_NOOWNERZORDER = 0x0200,

        /// <summary>
        /// 不重绘更改。 如果设置了此标志，则不执行任何形式的重绘。 这适用于工作区、非工作区 (，包括标题栏和滚动条) ，以及由于移动窗口而发现的父窗口的任何部分。
        /// 设置此标志后，应用程序必须显式失效或重新绘制需要重绘的窗口和父窗口的任何部分。
        /// </summary>
        SWP_NOREDRAW = 0x0008,

        /// <summary>
        /// 与 SWP_NOOWNERZORDER 标志相同。
        /// </summary>
        SWP_NOREPOSITION = 0x0200,

        /// <summary>
        /// 与 SWP_NOOWNERZORDER 标志相同。
        /// </summary>
        SWP_NOSENDCHANGING = 0x0400,

        /// <summary>
        /// 保留当前大小 (忽略 cx 和 cy 参数) 。
        /// </summary>
        SWP_NOSIZE = 0x0001,

        /// <summary>
        /// 保留当前 Z 顺序 (忽略 hWndInsertAfter 参数) 。
        /// </summary>
        SWP_NOZORDER = 0x0004,

        /// <summary>
        /// 显示“接收端口跟踪选项” 窗口。
        /// </summary>
        SWP_SHOWWINDOW = 0x0040,
    }
}
