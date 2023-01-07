namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// Windows 消息
    /// </summary>
    public enum WindowMessage : int
    {
        WM_DESTROY = 0x0002,
        WM_CLOSE = 0x0010,

        /// <summary>
        /// 当 SystemParametersInfo 函数更改系统范围设置或策略设置发生更改时，将发送到所有顶级窗口的消息
        /// </summary>
        WM_SETTINGCHANGE = 0x001A,

        /// <summary>
        /// 当窗口的大小或位置即将更改时，发送到窗口。 应用程序可以使用此消息替代窗口的默认最大化大小和位置，或者默认的最小或最大跟踪大小。
        /// </summary>
        WM_GETMINMAXINFO = 0x0024,

        /// <summary>
        /// 应用程序将 WM_COPYDATA 消息发送到另一个应用程序。
        /// </summary>
        WM_COPYDATA = 0x004A,

        WM_COMMAND = 0x0111,

        /// <summary>
        /// 当光标移动时，WM_MOUSEMOVE消息被发送到窗口。如果没有捕获鼠标，则将消息发送到包含光标的窗口。否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MOUSEMOVE = 0x0200,

        /// <summary>
        /// WM_LBUTTONDOWN消息是当用户按下鼠标左键，而光标位于窗口的客户端区域时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_LBUTTONDOWN = 0x0201,

        /// <summary>
        /// WM_LBUTTONUP消息是在用户释放鼠标左键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_LBUTTONUP = 0x0202,

        /// <summary>
        /// WM_LBUTTONDBLCLK消息是在用户双击鼠标左键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_LBUTTONDBLCLK = 0x0203,

        /// <summary>
        /// WM_RBUTTONDOWN消息是当用户在窗口的客户端区域按下鼠标右键时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_RBUTTONDOWN = 0x0204,

        /// <summary>
        /// WM_RBUTTONUP消息是在用户释放鼠标右键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_RBUTTONUP = 0x0205,

        /// <summary>
        /// 当用户在窗口的客户端区域内双击鼠标右键时，WM_RBUTTONDBLCLK消息将被发布。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_RBUTTONDBLCLK = 0x0206,

        /// <summary>
        /// WM_MBUTTONDOWN消息是当用户在窗口的客户端区域按下鼠标中键时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MBUTTONDOWN = 0x0207,

        /// <summary>
        /// WM_MBUTTONUP消息是在用户释放鼠标中键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MBUTTONUP = 0x0208,

        /// <summary>
        /// WM_MBUTTONDBLCLK消息是当用户双击鼠标中间按钮，而光标位于窗口的客户端区域时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MBUTTONDBLCLK = 0x0209,

        /// <summary>
        /// WM_DPICHANGED消息是当窗口的有效点数 (dpi) 更改时发送。 DPI 是窗口的规模因子。 有多个事件可能导致 DPI 更改。
        /// </summary>
        WM_DPICHANGED = 0x02E0,

        /// <summary>
        /// WM_USER用于定义专用窗口类使用的专用消息
        /// </summary>
        WM_USER = 0x0400,

        WM_TRAY_CALLBACK_MESSAGE = 0x0401
    }
}
