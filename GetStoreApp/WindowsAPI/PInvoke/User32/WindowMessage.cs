namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// Windows 消息
    /// </summary>
    public enum WindowMessage : int
    {
        /// <summary>
        /// 不执行任何操作。 如果应用程序想要帖子收件人窗口将忽略的邮件，则应用程序会发送WM_NULL邮件。
        /// </summary>
        WM_NULL = 0x0000,

        /// <summary>
        /// 当应用程序请求通过调用 <see cref="User32Library.CreateWindowEx"> 或 CreateWindowEx 函数创建窗口时发送。 (函数返回之前发送消息。) 新窗口的窗口过程在创建窗口后接收此消息，但在窗口变为可见之前。
        /// </summary>
        WM_CREATE = 0x0001,

        /// <summary>
        /// 当窗口被销毁时发送。 它将发送到从屏幕中删除窗口后正在销毁的窗口过程。
        /// </summary>
        WM_DESTROY = 0x0002,

        /// <summary>
        /// 在移动窗口后发送。
        /// </summary>
        WM_MOVE = 0x0003,

        /// <summary>
        /// 更改窗口大小后发送到窗口。
        /// </summary>
        WM_SIZE = 0x0005,

        /// <summary>
        /// 发送到正在激活的窗口和正在停用的窗口。 如果窗口使用相同的输入队列，则会同步发送消息，首先发送到正在停用的顶级窗口的窗口过程，
        /// 然后发送到正在激活的顶级窗口的窗口过程。 如果窗口使用不同的输入队列，则会异步发送消息，以便立即激活该窗口。
        /// </summary>
        WM_ACTIVATE = 0x0006,

        /// <summary>
        /// 在窗口获得键盘焦点后发送到窗口。
        /// </summary>
        WM_SETFOCUS = 0x0007,

        /// <summary>
        /// 在失去键盘焦点之前立即发送到窗口。
        /// </summary>
        WM_KILLFOCUS = 0x0008,

        /// <summary>
        /// 当应用程序更改窗口的启用状态时发送。 它会发送到启用状态正在更改的窗口。 此消息在 EnableWindow 函数返回之前发送，
        /// 但在启用状态 (WS_DISABLED 样式位) 窗口已更改之后。
        /// </summary>
        WM_ENABLE = 0x000A,

        /// <summary>
        /// 将 WM_SETREDRAW 消息发送到窗口，以允许重新绘制该窗口中的更改，或阻止重新绘制该窗口中的更改。
        /// </summary>
        WM_SETREDRAW = 0x000B,

        /// <summary>
        /// 设置窗口的文本。
        /// </summary>
        WM_SETTEXT = 0x000C,

        /// <summary>
        /// 将对应于窗口的文本复制到调用方提供的缓冲区中。
        /// </summary>
        WM_GETTEXT = 0x000D,

        /// <summary>
        /// 确定与窗口关联的文本的长度（以字符为单位）。
        /// </summary>
        WM_GETTEXTLENGTH = 0x000E,

        /// <summary>
        /// 当系统或其他应用程序发出绘制应用程序窗口部分的请求时，将发送 <see cref="WM_PAINT"> 消息。 当调用 UpdateWindow 或 RedrawWindow 函数或
        /// DispatchMessage 函数时，应用程序通过使用 GetMessage 或 PeekMessage 函数获取 <see cref="WM_PAINT"> 消息时发送该消息。
        /// </summary>
        WM_PAINT = 0x000F,

        /// <summary>
        /// 发送为窗口或应用程序应终止的信号。
        /// </summary>
        WM_CLOSE = 0x0010,

        /// <summary>
        /// WM_ENDSESSION消息在系统处理WM_QUERYENDSESSION消息的结果后发送到应用程序。 WM_ENDSESSION消息通知应用程序会话是否结束。
        /// </summary>
        WM_QUERYENDSESSION = 0x0011,

        /// <summary>
        /// 指示终止应用程序的请求，并在应用程序调用 PostQuitMessage 函数时生成。 此消息导致 GetMessage 函数返回零。
        /// </summary>
        WM_QUIT = 0x0012,

        /// <summary>
        /// 当用户请求将窗口还原到其以前的大小和位置时，发送到图标。
        /// </summary>
        WM_QUERYOPEN = 0x0013,

        /// <summary>
        /// 当窗口背景必须在 (擦除时发送，例如，当窗口大小调整为) 时。 将发送消息以准备窗口的无效部分进行绘制。
        /// </summary>
        WM_ERASEBKGND = 0x0014,

        /// <summary>
        /// 对系统颜色设置进行更改时 ，会将WM_SYSCOLORCHANGE 消息发送到所有顶级窗口。
        /// </summary>
        WM_SYSCOLORCHANGE = 0x0015,

        /// <summary>
        /// <see cref="WM_ENDSESSION"> 消息在系统处理 <see cref="WM_QUERYENDSESSION"> 消息的结果后发送到应用程序。 <see cref="WM_ENDSESSION"> 消息通知应用程序会话是否结束。
        /// </summary>
        WM_ENDSESSION = 0x0016,

        /// <summary>
        /// 应用程序在更改WIN.INI文件后，将 <see cref="WM_WININICHANGE"> 消息发送到所有顶级窗口。 SystemParametersInfo 函数在应用程序使用该函数更改WIN.INI中的设置后发送此消息。
        /// </summary>
        WM_WININICHANGE = 0x001A,

        /// <summary>
        /// 当 SystemParametersInfo 函数更改系统范围设置或策略设置发生更改时，将发送到所有顶级窗口的消息。
        /// </summary>
        WM_SETTINGCHANGE = WM_WININICHANGE,

        /// <summary>
        /// 每当用户更改设备模式设置时， <see cref="WM_DEVMODECHANGE"> 消息都会发送到所有顶级窗口。
        /// </summary>
        WM_DEVMODECHANGE = 0x001B,

        /// <summary>
        /// 当属于与活动窗口不同的应用程序的窗口即将激活时发送。 该消息将发送到正在激活其窗口的应用程序以及正在停用其窗口的应用程序。
        /// </summary>
        WM_ACTIVATEAPP = 0x001C,

        /// <summary>
        /// 应用程序更改字体资源池后，将 WM_FONTCHANGE 消息发送到系统中的所有顶级窗口。
        /// </summary>
        WM_FONTCHANGE = 0x001D,

        /// <summary>
        /// 每当系统时间发生更改时发送的消息。
        /// </summary>
        WM_TIMECHANGE = 0x001E,

        /// <summary>
        /// 发送到取消某些模式，例如鼠标捕获。 例如，当显示对话框或消息框时，系统会将此消息发送到活动窗口。
        /// 某些函数还会显式将此消息发送到指定窗口，而不考虑它是活动窗口。
        /// </summary>
        WM_CANCELMODE = 0x001F,

        /// <summary>
        /// 如果鼠标导致光标在窗口中移动，并且未捕获鼠标输入，则发送到窗口。
        /// </summary>
        WM_SETCURSOR = 0x0020,

        /// <summary>
        /// 当光标处于非活动窗口中且用户按下鼠标按钮时发送。 仅当子窗口将其传递给 <see cref="User32Library.DefWindowProc"> 函数时，父窗口才会接收此消息。
        /// </summary>
        WM_MOUSEACTIVATE = 0x0021,

        /// <summary>
        /// 当用户单击窗口的标题栏或窗口被激活、移动或调整大小时，发送到子窗口。
        /// </summary>
        WM_CHILDACTIVATE = 0x0022,

        /// <summary>
        /// 由基于计算机的训练 (CBT) 应用程序发送，以将用户输入消息与通过 WH_JOURNALPLAYBACK 过程发送的其他消息分开。
        /// </summary>
        WM_QUEUESYNC = 0x0023,

        /// <summary>
        /// 当窗口的大小或位置即将更改时，发送到窗口。 应用程序可以使用此消息替代窗口的默认最大化大小和位置，或者默认的最小或最大跟踪大小。
        /// </summary>
        WM_GETMINMAXINFO = 0x0024,

        /// <summary>
        /// 发送给最小化窗口，当它图标将要被重画时
        /// </summary>
        WM_PAINTICON = 0x0026,

        /// <summary>
        /// 发送给某个最小化窗口，仅当它在画图标前它的背景必须被重画
        /// </summary>
        WM_ICONERASEBKGND = 0x0027,

        /// <summary>
        /// 发送到对话框过程，将键盘焦点设置为对话框中的其他控件。
        /// </summary>
        WM_NEXTDLGCTL = 0x0028,

        /// <summary>
        /// 每当向打印管理器队列中添加或删除作业时，都会从打印管理器发送 <see cref="WM_SPOOLERSTATUS"> 消息。
        /// </summary>
        WM_SPOOLERSTATUS = 0x002A,

        /// <summary>
        /// 当按钮、组合框、列表框或菜单的可视方面发生更改时，发送到所有者绘制按钮、组合框、列表框或菜单的父窗口。
        /// </summary>
        WM_DRAWITEM = 0x002B,

        /// <summary>
        /// 创建控件或菜单时，发送到组合框、列表框、列表视图控件或菜单项的所有者窗口。
        /// </summary>
        WM_MEASUREITEM = 0x002C,

        /// <summary>
        /// 当列表框或组合框被销毁或项目被 LB_DELETESTRING、 LB_RESETCONTENT、 CB_DELETESTRING或 CB_RESETCONTENT 消息删除时，发送到列表框或组合框的所有者。 系统为每个已删除的项发送 一条 <see cref="WM_DELETEITEM"> 消息。 系统为包含非零项数据的任何已删除列表框或组合框项发送 <see cref="WM_DELETEITEM"> 消息。
        /// </summary>
        WM_DELETEITEM = 0x002D,

        /// <summary>
        /// 由具有 LBS_WANTKEYBOARDINPUT 样式的列表框发送到其所有者以响应 <see cref="WM_KEYDOWN"> 消息。
        /// </summary>
        WM_VKEYTOITEM = 0x002E,

        /// <summary>
        /// 由具有 LBS_WANTKEYBOARDINPUT 样式的列表框发送到其所有者，以响应 WM_CHAR 消息。
        /// </summary>
        WM_CHARTOITEM = 0x002F,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_SETHOTKEY = 0x0032,
        WM_GETHOTKEY = 0x0033,
        WM_QUERYDRAGICON = 0x0037,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003D,
        WM_COMPACTING = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_POWER = 0x0048,

        /// <summary>
        /// 应用程序将 <see cref="WM_COPYDATA"> 消息发送到另一个应用程序。
        /// </summary>
        WM_COPYDATA = 0x004A,

        /// <summary>
        /// 按下非系统键时，使用键盘焦点发布到窗口。 非系统键是未按下 ALT 键时按下的键。
        /// </summary>
        WM_KEYDOWN = 0x0100,

        /// <summary>
        /// 当 TranslateMessage 函数翻译 <see cref="WM_KEYDOWN"> 消息时，发布到具有键盘焦点的窗口。 <see cref="WM_CHAR"> 消息包含按下的键的字符代码。
        /// </summary>
        WM_CHAR = 0x0102,

        /// <summary>
        /// 当用户从菜单中选择命令项、控件将通知消息发送到其父窗口或翻译快捷键击时发送。
        /// </summary>
        WM_COMMAND = 0x0111,

        /// <summary>
        /// 当用户从 “窗口 ”菜单中选择命令时，窗口会收到此消息， (以前称为系统或控件菜单) ，或者当用户选择最大化按钮、最小化按钮、还原按钮或关闭按钮时。
        /// </summary>
        WM_SYSCOMMAND = 0x0112,

        /// <summary>
        /// 当光标移动时，<see cref="WM_MOUSEMOVE"> 消息被发送到窗口。如果没有捕获鼠标，则将消息发送到包含光标的窗口。否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MOUSEMOVE = 0x0200,

        /// <summary>
        /// <see cref="WM_LBUTTONDOWN"> 消息是当用户按下鼠标左键，而光标位于窗口的客户端区域时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_LBUTTONDOWN = 0x0201,

        /// <summary>
        /// <see cref="WM_LBUTTONUP"> 消息是在用户释放鼠标左键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_LBUTTONUP = 0x0202,

        /// <summary>
        /// <see cref="WM_LBUTTONDBLCLK"> 消息是在用户双击鼠标左键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_LBUTTONDBLCLK = 0x0203,

        /// <summary>
        /// <see cref="WM_RBUTTONDOWN"> 消息是当用户在窗口的客户端区域按下鼠标右键时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_RBUTTONDOWN = 0x0204,

        /// <summary>
        /// <see cref="WM_RBUTTONUP"> 消息是在用户释放鼠标右键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_RBUTTONUP = 0x0205,

        /// <summary>
        /// 当用户在窗口的客户端区域内双击鼠标右键时，<see cref="WM_RBUTTONDBLCLK"> 消息将被发布。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_RBUTTONDBLCLK = 0x0206,

        /// <summary>
        /// WM_MBUTTONDOWN消息是当用户在窗口的客户端区域按下鼠标中键时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MBUTTONDOWN = 0x0207,

        /// <summary>
        /// <see cref="WM_MBUTTONUP"> 消息是在用户释放鼠标中键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MBUTTONUP = 0x0208,

        /// <summary>
        /// <see cref="WM_MBUTTONDBLCLK"> 消息是当用户双击鼠标中间按钮，而光标位于窗口的客户端区域时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MBUTTONDBLCLK = 0x0209,

        /// <summary>
        /// <see cref="WM_DPICHANGED"> 消息是当窗口的有效点数 (dpi) 更改时发送。 DPI 是窗口的规模因子。 有多个事件可能导致 DPI 更改。
        /// </summary>
        WM_DPICHANGED = 0x02E0,

        /// <summary>
        /// <see cref="WM_USER"> 用于定义专用窗口类使用的专用消息
        /// </summary>
        WM_USER = 0x0400,

        WM_TRAY_CALLBACK_MESSAGE = 0x0401
    }
}
