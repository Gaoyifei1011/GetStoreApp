namespace GetStoreAppInstaller.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 指示光标热点的位置
    /// </summary>
    public enum HITTEST
    {
        /// <summary>
        /// 在屏幕背景上或窗口之间的分割线上（与 HTNOWHERE 相同，只是 DefWindowProc 函数会生成系统蜂鸣音以指示错误）。
        /// </summary>
        HTERROR = -2,

        /// <summary>
        /// 在同一线程当前由另一个窗口覆盖的窗口中（消息将发送到同一线程中的基础窗口，直到其中一个窗口返回不是 HTTRANSPARENT 的代码）。
        /// </summary>
        HTTRANSPARENT = -1,

        /// <summary>
        /// 在屏幕背景上，或在窗口之间的分隔线上。
        /// </summary>
        HTNOWHERE = 0,

        /// <summary>
        /// 在工作区中。
        /// </summary>
        HTCLIENT = 1,

        /// <summary>
        /// 在标题栏中。
        /// </summary>
        HTCAPTION = 2,

        /// <summary>
        /// 在窗口菜单或子窗口的关闭按钮中。
        /// </summary>
        HTSYSMENU = 3,

        /// <summary>
        /// 在大小框中（与 HTSIZE 相同）。
        /// </summary>
        HTGROWBOX = 4,

        /// <summary>
        /// 在菜单中。
        /// </summary>
        HTMENU = 5,

        /// <summary>
        /// 在水平滚动条中。
        /// </summary>
        HTHSCROLL = 6,

        /// <summary>
        /// 在垂直滚动条中。
        /// </summary>
        HTVSCROLL = 7,

        /// <summary>
        /// 在“最小化”按钮中。
        /// </summary>
        HTMINBUTTON = 8,

        /// <summary>
        /// 在“最大化”按钮中。
        /// </summary>
        HTMAXBUTTON = 9,

        /// <summary>
        /// 在可调整大小的窗口的左边框中（用户可以单击鼠标以水平调整窗口大小）。
        /// </summary>
        HTLEFT = 10,

        /// <summary>
        /// 在可调整大小的窗口的右左边框中（用户可以单击鼠标以水平调整窗口大小）。
        /// </summary>
        HTRIGHT = 11,

        /// <summary>
        /// 在窗口的上水平边框中。
        /// </summary>
        HTTOP = 12,

        /// <summary>
        /// 在窗口边框的左上角。
        /// </summary>
        HTTOPLEFT = 13,

        /// <summary>
        /// 在窗口边框的右上角。
        /// </summary>
        HTTOPRIGHT = 14,

        /// <summary>
        /// 在可调整大小的窗口的下水平边框中（用户可以单击鼠标以垂直调整窗口大小）。
        /// </summary>
        HTBOTTOM = 15,

        /// <summary>
        /// 在可调整大小的窗口的边框左下角（用户可以单击鼠标以对角线调整窗口大小）。
        /// </summary>
        HTBOTTOMLEFT = 16,

        /// <summary>
        /// 在可调整大小的窗口的边框右下角（用户可以单击鼠标以对角线调整窗口大小）。
        /// </summary>
        HTBOTTOMRIGHT = 17,

        /// <summary>
        /// 在没有大小调整边框的窗口边框中。
        /// </summary>
        HTBORDER = 18,

        HTOBJECT = 19,

        /// <summary>
        /// 在“关闭”按钮中。
        /// </summary>
        HTCLOSE = 20,

        /// <summary>
        /// 在“帮助”按钮中。
        /// </summary>
        HTHELP = 21,
    }
}
