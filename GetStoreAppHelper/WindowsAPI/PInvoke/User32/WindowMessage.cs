namespace GetStoreAppHelper.WindowsAPI.PInvoke.User32
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
        /// 当应用程序请求通过调用 InitializeWindow 或 <see cref="User32Library.CreateWindowEx"> 函数创建窗口时发送。 (函数返回之前发送消息。) 新窗口的窗口过程在创建窗口后接收此消息，但在窗口变为可见之前。
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
        /// 将 <see cref="WM_SETREDRAW"> 消息发送到窗口，以允许重新绘制该窗口中的更改，或阻止重新绘制该窗口中的更改。
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
        /// <see cref="WM_ENDSESSION"> 消息在系统处理 <see cref="WM_QUERYENDSESSION"> 消息的结果后发送到应用程序。 <see cref="WM_ENDSESSION"> 消息通知应用程序会话是否结束。
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
        /// 应用程序更改字体资源池后，将 <see cref="WM_FONTCHANGE"> 消息发送到系统中的所有顶级窗口。
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
        /// 由具有 LBS_WANTKEYBOARDINPUT 样式的列表框发送到其所有者，以响应 <see cref="WM_CHAR"> 消息。
        /// </summary>
        WM_CHARTOITEM = 0x002F,

        /// <summary>
        /// 设置控件在绘制文本时要使用的字体。
        /// </summary>
        WM_SETFONT = 0x0030,

        /// <summary>
        /// 检索控件当前正在绘制其文本的字体。
        /// </summary>
        WM_GETFONT = 0x0031,

        /// <summary>
        /// 发送到窗口以将热键与窗口相关联。 当用户按下热键时，系统会激活窗口。
        /// </summary>
        WM_SETHOTKEY = 0x0032,

        /// <summary>
        /// 已发送以确定与窗口关联的热键。
        /// </summary>
        WM_GETHOTKEY = 0x0033,

        /// <summary>
        /// 发送到最小化的 (标志性的) 窗口。 窗口将由用户拖动，但没有为其类定义的图标。 应用程序可以将句柄返回到图标或游标。 当用户拖动图标时，系统会显示此光标或图标。
        /// </summary>
        WM_QUERYDRAGICON = 0x0037,

        /// <summary>
        /// 已发送到确定新项在所有者绘制组合框或列表框的排序列表中相对位置。 每当应用程序添加新项时，系统会将此消息发送到使用 CBS_SORT 或 LBS_SORT 样式创建的组合框或列表框的所有者。
        /// </summary>
        WM_COMPAREITEM = 0x0039,

        /// <summary>
        /// Microsoft Active Accessibility 和 Microsoft UI 自动化发送，以获取有关服务器应用程序中包含的可访问对象的信息。 应用程序永远不会直接发送此消息。
        /// </summary>
        WM_GETOBJECT = 0x003D,

        /// <summary>
        /// 当系统检测到超过 30 秒到 60 秒间隔的系统时间超过 12.5% 时，发送到所有顶级窗口。 这表示系统内存较低。
        /// </summary>
        WM_COMPACTING = 0x0041,

        /// <summary>
        /// 每当发生 COM 端口事件时，通信设备驱动程序都会发布 <see cref="WM_COMMNOTIFY"> 消息。该消息指示窗口的输入或输出队列的状态。
        /// </summary>
        WM_COMMNOTIFY = 0x0044,

        /// <summary>
        /// 发送到一个窗口，其大小、位置或位置在 Z 顺序中将随着 对 SetWindowPos 函数或其他窗口管理函数的调用而更改。
        /// </summary>
        WM_WINDOWPOSCHANGING = 0x0046,

        /// <summary>
        /// 由于对 SetWindowPos 函数或其他窗口管理函数的调用，其大小、位置或位置在 Z 顺序中的窗口已更改。
        /// </summary>
        WM_WINDOWPOSCHANGED = 0x0047,

        /// <summary>
        /// 通知应用程序，系统（通常是电池供电的个人电脑）即将进入暂停模式。
        /// </summary>
        WM_POWER = 0x0048,

        /// <summary>
        /// 应用程序将 <see cref="WM_COPYDATA"> 消息发送到另一个应用程序。
        /// </summary>
        WM_COPYDATA = 0x004A,

        /// <summary>
        /// 当用户取消应用程序的日记活动时发布到应用程序。 消息使用 NULL 窗口句柄发布。
        /// </summary>
        WM_CANCELJOURNAL = 0x004B,

        /// <summary>
        /// 当发生事件或控件需要一些信息时，由公共控件发送到其父窗口。
        /// </summary>
        WM_NOTIFY = 0x004E,

        /// <summary>
        /// 当用户选择新的输入语言（在键盘控制面板应用程序) 或系统任务栏上的指示器中指定的热键时，发布到具有焦点的窗口。 应用程序可以通过将消息传递给 <see cref="User32Library.DefWindowProc"> 函数来接受更改，或者拒绝更改，并阻止它立即返回) 。
        /// </summary>
        WM_INPUTLANGCHANGEREQUEST = 0x0050,

        /// <summary>
        /// 更改应用程序输入语言后，发送到最受影响的窗口。 应进行任何特定于应用程序的设置并将消息传递给 <see cref="User32Library.DefWindowProc"> 函数，该函数会将消息传递给所有第一级子窗口。 这些子窗口可以将消息传递给 <see cref="User32Library.DefWindowProc"> ，使其将消息传递给其子窗口等。
        /// </summary>
        WM_INPUTLANGCHANGE = 0x0051,

        /// <summary>
        /// 发送到已启动具有Windows帮助的训练卡的应用程序。 当用户单击可创作按钮时，该消息会通知应用程序。 应用程序通过在对 WinHelp 函数的调用中指定HELP_TCARD命令来启动训练卡。
        /// </summary>
        WM_TCARD = 0x0052,

        /// <summary>
        /// 指示用户按下了 F1 键。 如果按下 F1 时菜单处于活动状态， <see cref="WM_HELP"> 发送到与菜单关联的窗口;否则， <see cref="WM_HELP"> 将发送到具有键盘焦点的窗口。 如果没有窗口具有键盘焦点， <see cref="WM_HELP"> 将发送到当前活动窗口。
        /// </summary>
        WM_HELP = 0x0053,

        /// <summary>
        /// 用户登录或关闭后发送到所有窗口。 当用户登录或关闭时，系统会更新用户特定的设置。 系统更新设置后立即发送此消息。
        /// </summary>
        WM_USERCHANGED = 0x0054,

        /// <summary>
        /// 确定窗口是否接受 <see cref="WM_NOTIFY"> 通知消息中的 ANSI 或 Unicode 结构。 <see cref="WM_NOTIFYFORMAT"> 消息从公共控件发送到其父窗口，从父窗口发送到公共控件。
        /// </summary>
        WM_NOTIFYFORMAT = 0x0055,

        /// <summary>
        /// 通知用户希望显示上下文菜单的窗口。 用户可能已在窗口中单击鼠标右键 (右键单击) 、按 Shift+F10 或按应用程序键 (上下文菜单键) 某些键盘上可用。
        /// </summary>
        WM_CONTEXTMENU = 0x007B,

        /// <summary>
        /// 当 <see cref="User32Library.SetWindowLong"> 函数即将更改窗口的一个或多个样式时，发送到窗口。
        /// </summary>
        WM_STYLECHANGING = 0x007C,

        /// <summary>
        /// <see cref="User32Library.SetWindowLong"> 函数更改窗口的一个或多个样式后发送到窗口。
        /// </summary>
        WM_STYLECHANGED = 0x007D,

        /// <summary>
        /// 显示分辨率发生更改时 ，会将 <see cref="WM_DISPLAYCHANGE"> 消息发送到所有窗口。
        /// </summary>
        WM_DISPLAYCHANGE = 0x007E,

        /// <summary>
        /// 发送到窗口以检索与窗口关联的大图标或小图标的句柄。 系统在 Alt+TAB 对话框中显示大图标，并在窗口标题中显示小图标。
        /// </summary>
        WM_GETICON = 0x007F,

        /// <summary>
        /// 将新的大或小图标与窗口相关联。 系统在 Alt+TAB 对话框中显示大图标，以及窗口标题中的小图标。
        /// </summary>
        WM_SETICON = 0x0080,

        /// <summary>
        /// 首次创建窗口时 ，在 <see cref="WM_CREATE"> 消息之前发送。
        /// </summary>
        WM_NCCREATE = 0x0081,

        /// <summary>
        /// 通知窗口其非客户区域正在被销毁。 <see cref="User32Library.DestroyWindow"> 函数将 <see cref="WM_NCDESTROY"> 消息发送到 <see cref="WM_DESTROY"> 消息后面的窗口。<see cref="WM_DESTROY"> 用于释放与窗口关联的已分配内存对象。
        /// 子窗口被销毁后，将发送 <see cref="WM_NCDESTROY"> 消息。 相比之下， <see cref="WM_DESTROY"> 在销毁子窗口之前发送。
        /// </summary>
        WM_NCDESTROY = 0x0082,

        /// <summary>
        /// 在必须计算窗口工作区的大小和位置时发送。 通过处理此消息，当窗口的大小或位置发生更改时，应用程序可以控制窗口工作区的内容。
        /// </summary>
        WM_NCCALCSIZE = 0x0083,

        /// <summary>
        /// 发送到窗口以确定窗口的哪个部分对应于特定的屏幕坐标。 例如，当光标移动、按下或释放鼠标按钮或响应对 WindowFromPoint 等函数的调用时，可能会发生这种情况。 如果未捕获鼠标，则会将消息发送到光标下方的窗口。 否则，消息将发送到已捕获鼠标的窗口。
        /// </summary>
        WM_NCHITTEST = 0x0084,

        /// <summary>
        /// The WM_NCPAINT message is sent to a window when its frame must be painted.
        /// </summary>
        WM_NCPAINT = 0x0085,

        /// <summary>
        /// The WM_NCACTIVATE message is sent to a window when its nonclient area needs to be changed to indicate an active or inactive state.
        /// </summary>
        WM_NCACTIVATE = 0x0086,

        /// <summary>
        /// The WM_GETDLGCODE message is sent to the window procedure associated with a control. By default, the system handles all keyboard input to the control; the system interprets certain types of keyboard input as FolderDialog box navigation keys. To override this default behavior, the control can respond to the WM_GETDLGCODE message to indicate the types of input it wants to process itself.
        /// </summary>
        WM_GETDLGCODE = 0x0087,

        /// <summary>
        /// The WM_SYNCPAINT message is used to synchronize painting while avoiding linking independent GUI threads.
        /// </summary>
        WM_SYNCPAINT = 0x0088,

        /// <summary>
        /// The WM_NCMOUSEMOVE message is posted to a window when the cursor is moved within the nonclient area of the window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCMOUSEMOVE = 0x00A0,

        /// <summary>
        /// The WM_NCLBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCLBUTTONDOWN = 0x00A1,

        /// <summary>
        /// The WM_NCLBUTTONUP message is posted when the user releases the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCLBUTTONUP = 0x00A2,

        /// <summary>
        /// The WM_NCLBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCLBUTTONDBLCLK = 0x00A3,

        /// <summary>
        /// The WM_NCRBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCRBUTTONDOWN = 0x00A4,

        /// <summary>
        /// The WM_NCRBUTTONUP message is posted when the user releases the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCRBUTTONUP = 0x00A5,

        /// <summary>
        /// The WM_NCRBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCRBUTTONDBLCLK = 0x00A6,

        /// <summary>
        /// The WM_NCMBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCMBUTTONDOWN = 0x00A7,

        /// <summary>
        /// The WM_NCMBUTTONUP message is posted when the user releases the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCMBUTTONUP = 0x00A8,

        /// <summary>
        /// The WM_NCMBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCMBUTTONDBLCLK = 0x00A9,

        /// <summary>
        /// The WM_NCXBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCXBUTTONDOWN = 0x00AB,

        /// <summary>
        /// The WM_NCXBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCXBUTTONUP = 0x00AC,

        /// <summary>
        /// The WM_NCXBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCXBUTTONDBLCLK = 0x00AD,

        /// <summary>
        /// The WM_INPUT_DEVICE_CHANGE message is sent to the window that registered to receive raw input. A window receives this message through its WindowProc function.
        /// </summary>
        WM_BM_CLICK = 0x00F5,

        /// <summary>
        /// The WM_INPUT_DEVICE_CHANGE message is sent to the window that registered to receive raw input. A window receives this message through its WindowProc function.
        /// </summary>
        WM_INPUT_DEVICE_CHANGE = 0x00FE,

        /// <summary>
        /// The WM_INPUT message is sent to the window that is getting raw input.
        /// </summary>
        WM_INPUT = 0x00FF,

        /// <summary>
        /// This message filters for keyboard messages.
        /// </summary>
        WM_KEYFIRST = 0x0100,

        /// <summary>
        /// 按下非系统键时，使用键盘焦点发布到窗口。 非系统键是未按下 ALT 键时按下的键。
        /// </summary>
        WM_KEYDOWN = 0x0100,

        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        WM_KEYUP = 0x0101,

        /// <summary>
        /// 当 TranslateMessage 函数翻译 <see cref="WM_KEYDOWN"> 消息时，发布到具有键盘焦点的窗口。 <see cref="WM_CHAR"> 消息包含按下的键的字符代码。
        /// </summary>
        WM_CHAR = 0x0102,

        /// <summary>
        /// The WM_DEADCHAR message is posted to the window with the keyboard focus when a WM_KEYUP message is translated by the TranslateMessage function. WM_DEADCHAR specifies a character code generated by a dead key. A dead key is a key that generates a character, such as the umlaut (double-dot), that is combined with another character to form a composite character. For example, the umlaut-O character (Ö) is generated by typing the dead key for the umlaut character, and then typing the O key.
        /// </summary>
        WM_DEADCHAR = 0x0103,

        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user presses the F10 key (which activates the menu bar) or holds down the ALT key and then presses another key. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
        /// </summary>
        WM_SYSKEYDOWN = 0x0104,

        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user releases a key that was pressed while the ALT key was held down. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
        /// </summary>
        WM_SYSKEYUP = 0x0105,

        /// <summary>
        /// The WM_SYSCHAR message is posted to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. It specifies the character code of a system character key — that is, a character key that is pressed while the ALT key is down.
        /// </summary>
        WM_SYSCHAR = 0x0106,

        /// <summary>
        /// The WM_SYSDEADCHAR message is sent to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. WM_SYSDEADCHAR specifies the character code of a system dead key — that is, a dead key that is pressed while holding down the ALT key.
        /// </summary>
        WM_SYSDEADCHAR = 0x0107,

        /// <summary>
        /// The WM_UNICHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_UNICHAR message contains the character code of the key that was pressed.
        /// The WM_UNICHAR message is equivalent to WM_CHAR, but it uses Unicode Transformation Format (UTF)-32, whereas WM_CHAR uses UTF-16. It is designed to send or post Unicode characters to ANSI windows and it can can handle Unicode Supplementary Plane characters.
        /// </summary>
        WM_UNICHAR = 0x0109,

        /// <summary>
        /// This message filters for keyboard messages.
        /// </summary>
        WM_KEYLAST = 0x0109,

        /// <summary>
        /// Sent immediately before the IME generates the composition string as a result of a keystroke. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_STARTCOMPOSITION = 0x010D,

        /// <summary>
        /// Sent to an application when the IME ends composition. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_ENDCOMPOSITION = 0x010E,

        /// <summary>
        /// Sent to an application when the IME changes composition status as a result of a keystroke. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_COMPOSITION = 0x010F,

        WM_IME_KEYLAST = 0x010F,

        /// <summary>
        /// The WM_INITDIALOG message is sent to the FolderDialog box procedure immediately before a FolderDialog box is displayed. Dialog box procedures typically use this message to initialize controls and carry out any other initialization tasks that affect the appearance of the FolderDialog box.
        /// </summary>
        WM_INITDIALOG = 0x0110,

        /// <summary>
        /// 当用户从菜单中选择命令项、控件将通知消息发送到其父窗口或翻译快捷键击时发送。
        /// </summary>
        WM_COMMAND = 0x0111,

        /// <summary>
        /// 当用户从 “窗口 ”菜单中选择命令时，窗口会收到此消息， (以前称为系统或控件菜单) ，或者当用户选择最大化按钮、最小化按钮、还原按钮或关闭按钮时。
        /// </summary>
        WM_SYSCOMMAND = 0x0112,

        /// <summary>
        /// The WM_TIMER message is posted to the installing thread's message queue when a timer expires. The message is posted by the GetMessage or PeekMessage function.
        /// </summary>
        WM_TIMER = 0x0113,

        /// <summary>
        /// The WM_HSCROLL message is sent to a window when a scroll event occurs in the window's standard horizontal scroll bar. This message is also sent to the owner of a horizontal scroll bar control when a scroll event occurs in the control.
        /// </summary>
        WM_HSCROLL = 0x0114,

        /// <summary>
        /// The WM_VSCROLL message is sent to a window when a scroll event occurs in the window's standard vertical scroll bar. This message is also sent to the owner of a vertical scroll bar control when a scroll event occurs in the control.
        /// </summary>
        WM_VSCROLL = 0x0115,

        /// <summary>
        /// The WM_INITMENU message is sent when a menu is about to become active. It occurs when the user clicks an item on the menu bar or presses a menu key. This allows the application to modify the menu before it is displayed.
        /// </summary>
        WM_INITMENU = 0x0116,

        /// <summary>
        /// The WM_INITMENUPOPUP message is sent when a drop-down menu or submenu is about to become active. This allows an application to modify the menu before it is displayed, without changing the entire menu.
        /// </summary>
        WM_INITMENUPOPUP = 0x0117,

        /// <summary>
        /// The WM_MENUSELECT message is sent to a menu's owner window when the user selects a menu item.
        /// </summary>
        WM_MENUSELECT = 0x011F,

        /// <summary>
        /// The WM_MENUCHAR message is sent when a menu is active and the user presses a key that does not correspond to any mnemonic or accelerator key. This message is sent to the window that owns the menu.
        /// </summary>
        WM_MENUCHAR = 0x0120,

        /// <summary>
        /// The WM_ENTERIDLE message is sent to the owner window of a modal FolderDialog box or menu that is entering an idle state. A modal FolderDialog box or menu enters an idle state when no messages are waiting in its queue after it has processed one or more previous messages.
        /// </summary>
        WM_ENTERIDLE = 0x0121,

        /// <summary>
        /// The WM_MENURBUTTONUP message is sent when the user releases the right mouse button while the cursor is on a menu item.
        /// </summary>
        WM_MENURBUTTONUP = 0x0122,

        /// <summary>
        /// The WM_MENUDRAG message is sent to the owner of a drag-and-drop menu when the user drags a menu item.
        /// </summary>
        WM_MENUDRAG = 0x0123,

        /// <summary>
        /// The WM_MENUGETOBJECT message is sent to the owner of a drag-and-drop menu when the mouse cursor enters a menu item or moves from the center of the item to the top or bottom of the item.
        /// </summary>
        WM_MENUGETOBJECT = 0x0124,

        /// <summary>
        /// The WM_UNINITMENUPOPUP message is sent when a drop-down menu or submenu has been destroyed.
        /// </summary>
        WM_UNINITMENUPOPUP = 0x0125,

        /// <summary>
        /// The WM_MENUCOMMAND message is sent when the user makes a selection from a menu.
        /// </summary>
        WM_MENUCOMMAND = 0x0126,

        /// <summary>
        /// An application sends the WM_CHANGEUISTATE message to indicate that the user interface (UI) state should be changed.
        /// </summary>
        WM_CHANGEUISTATE = 0x0127,

        /// <summary>
        /// An application sends the WM_UPDATEUISTATE message to change the user interface (UI) state for the specified window and all its child windows.
        /// </summary>
        WM_UPDATEUISTATE = 0x0128,

        /// <summary>
        /// An application sends the WM_QUERYUISTATE message to retrieve the user interface (UI) state for a window.
        /// </summary>
        WM_QUERYUISTATE = 0x0129,

        /// <summary>
        /// The WM_CTLCOLORMSGBOX message is sent to the owner window of a message box before Windows draws the message box. By responding to this message, the owner window can set the text and background colors of the message box by using the given display device context handle.
        /// </summary>
        WM_CTLCOLORMSGBOX = 0x0132,

        /// <summary>
        /// An edit control that is not read-only or disabled sends the WM_CTLCOLOREDIT message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the edit control.
        /// </summary>
        WM_CTLCOLOREDIT = 0x0133,

        /// <summary>
        /// Sent to the parent window of a list box before the system draws the list box. By responding to this message, the parent window can set the text and background colors of the list box by using the specified display device context handle.
        /// </summary>
        WM_CTLCOLORLISTBOX = 0x0134,

        /// <summary>
        /// The WM_CTLCOLORBTN message is sent to the parent window of a button before drawing the button. The parent window can change the button's text and background colors. However, only owner-drawn buttons respond to the parent window processing this message.
        /// </summary>
        WM_CTLCOLORBTN = 0x0135,

        /// <summary>
        /// The WM_CTLCOLORDLG message is sent to a FolderDialog box before the system draws the FolderDialog box. By responding to this message, the FolderDialog box can set its text and background colors using the specified display device context handle.
        /// </summary>
        WM_CTLCOLORDLG = 0x0136,

        /// <summary>
        /// The WM_CTLCOLORSCROLLBAR message is sent to the parent window of a scroll bar control when the control is about to be drawn. By responding to this message, the parent window can use the display context handle to set the background color of the scroll bar control.
        /// </summary>
        WM_CTLCOLORSCROLLBAR = 0x0137,

        /// <summary>
        /// A static control, or an edit control that is read-only or disabled, sends the WM_CTLCOLORSTATIC message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the static control.
        /// </summary>
        WM_CTLCOLORSTATIC = 0x0138,

        /// <summary>
        /// Use WM_MOUSEFIRST to specify the first mouse message. Use the PeekMessage() Function.
        /// </summary>
        WM_MOUSEFIRST = 0x0200,

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
        /// The WM_MOUSEWHEEL message is sent to the focus window when the mouse wheel is rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
        /// </summary>
        WM_MOUSEWHEEL = 0x020A,

        /// <summary>
        /// The WM_XBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_XBUTTONDOWN = 0x020B,

        /// <summary>
        /// The WM_XBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_XBUTTONUP = 0x020C,

        /// <summary>
        /// The WM_XBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_XBUTTONDBLCLK = 0x020D,

        /// <summary>
        /// The WM_MOUSEHWHEEL message is sent to the focus window when the mouse's horizontal scroll wheel is tilted or rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
        /// </summary>
        WM_MOUSEHWHEEL = 0x020E,

        /// <summary>
        /// Use WM_MOUSELAST to specify the last mouse message. Used with PeekMessage() Function.
        /// </summary>
        WM_MOUSELAST = 0x020E,

        /// <summary>
        /// The WM_PARENTNOTIFY message is sent to the parent of a child window when the child window is created or destroyed, or when the user clicks a mouse button while the cursor is over the child window. When the child window is being created, the system sends WM_PARENTNOTIFY just before the InitializeWindow or CreateWindowEx function that creates the window returns. When the child window is being destroyed, the system sends the message before any processing to destroy the window takes place.
        /// </summary>
        WM_PARENTNOTIFY = 0x0210,

        /// <summary>
        /// The WM_ENTERMENULOOP message informs an application's main window procedure that a menu modal loop has been entered.
        /// </summary>
        WM_ENTERMENULOOP = 0x0211,

        /// <summary>
        /// The WM_EXITMENULOOP message informs an application's main window procedure that a menu modal loop has been exited.
        /// </summary>
        WM_EXITMENULOOP = 0x0212,

        /// <summary>
        /// The WM_NEXTMENU message is sent to an application when the right or left arrow key is used to switch between the menu bar and the system menu.
        /// </summary>
        WM_NEXTMENU = 0x0213,

        /// <summary>
        /// The WM_SIZING message is sent to a window that the user is resizing. By processing this message, an application can monitor the size and position of the drag rectangle and, if needed, change its size or position.
        /// </summary>
        WM_SIZING = 0x0214,

        /// <summary>
        /// The WM_CAPTURECHANGED message is sent to the window that is losing the mouse capture.
        /// </summary>
        WM_CAPTURECHANGED = 0x0215,

        /// <summary>
        /// The WM_MOVING message is sent to a window that the user is moving. By processing this message, an application can monitor the position of the drag rectangle and, if needed, change its position.
        /// </summary>
        WM_MOVING = 0x0216,

        /// <summary>
        /// Notifies applications that a power-management event has occurred.
        /// </summary>
        WM_POWERBROADCAST = 0x0218,

        /// <summary>
        /// Notifies an application of a change to the hardware configuration of a device or the computer.
        /// </summary>
        WM_DEVICECHANGE = 0x0219,

        /// <summary>
        /// An application sends the WM_MDICREATE message to a multiple-document interface (MDI) client window to create an MDI child window.
        /// </summary>
        WM_MDICREATE = 0x0220,

        /// <summary>
        /// An application sends the WM_MDIDESTROY message to a multiple-document interface (MDI) client window to close an MDI child window.
        /// </summary>
        WM_MDIDESTROY = 0x0221,

        /// <summary>
        /// An application sends the WM_MDIACTIVATE message to a multiple-document interface (MDI) client window to instruct the client window to activate a different MDI child window.
        /// </summary>
        WM_MDIACTIVATE = 0x0222,

        /// <summary>
        /// An application sends the WM_MDIRESTORE message to a multiple-document interface (MDI) client window to restore an MDI child window from maximized or minimized size.
        /// </summary>
        WM_MDIRESTORE = 0x0223,

        /// <summary>
        /// An application sends the WM_MDINEXT message to a multiple-document interface (MDI) client window to activate the next or previous child window.
        /// </summary>
        WM_MDINEXT = 0x0224,

        /// <summary>
        /// An application sends the WM_MDIMAXIMIZE message to a multiple-document interface (MDI) client window to maximize an MDI child window. The system resizes the child window to make its client area fill the client window. The system places the child window's window menu icon in the rightmost position of the frame window's menu bar, and places the child window's restore icon in the leftmost position. The system also appends the title bar text of the child window to that of the frame window.
        /// </summary>
        WM_MDIMAXIMIZE = 0x0225,

        /// <summary>
        /// An application sends the WM_MDITILE message to a multiple-document interface (MDI) client window to arrange all of its MDI child windows in a tile format.
        /// </summary>
        WM_MDITILE = 0x0226,

        /// <summary>
        /// An application sends the WM_MDICASCADE message to a multiple-document interface (MDI) client window to arrange all its child windows in a cascade format.
        /// </summary>
        WM_MDICASCADE = 0x0227,

        /// <summary>
        /// An application sends the WM_MDIICONARRANGE message to a multiple-document interface (MDI) client window to arrange all minimized MDI child windows. It does not affect child windows that are not minimized.
        /// </summary>
        WM_MDIICONARRANGE = 0x0228,

        /// <summary>
        /// An application sends the WM_MDIGETACTIVE message to a multiple-document interface (MDI) client window to retrieve the handle to the active MDI child window.
        /// </summary>
        WM_MDIGETACTIVE = 0x0229,

        /// <summary>
        /// An application sends the WM_MDISETMENU message to a multiple-document interface (MDI) client window to replace the entire menu of an MDI frame window, to replace the window menu of the frame window, or both.
        /// </summary>
        WM_MDISETMENU = 0x0230,

        /// <summary>
        /// The WM_ENTERSIZEMOVE message is sent one time to a window after it enters the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns.
        /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows is enabled.
        /// </summary>
        WM_ENTERSIZEMOVE = 0x0231,

        /// <summary>
        /// The WM_EXITSIZEMOVE message is sent one time to a window, after it has exited the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns.
        /// </summary>
        WM_EXITSIZEMOVE = 0x0232,

        /// <summary>
        /// Sent when the user drops a file on the window of an application that has registered itself as a recipient of dropped files.
        /// </summary>
        WM_DROPFILES = 0x0233,

        /// <summary>
        /// An application sends the WM_MDIREFRESHMENU message to a multiple-document interface (MDI) client window to refresh the window menu of the MDI frame window.
        /// </summary>
        WM_MDIREFRESHMENU = 0x0234,

        /// <summary>
        /// Sent to an application when a window is activated. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_SETCONTEXT = 0x0281,

        /// <summary>
        /// Sent to an application to notify it of changes to the IME window. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_NOTIFY = 0x0282,

        /// <summary>
        /// Sent by an application to direct the IME window to carry out the requested command. The application uses this message to control the IME window that it has created. To send this message, the application calls the SendMessage function with the following parameters.
        /// </summary>
        WM_IME_CONTROL = 0x0283,

        /// <summary>
        /// Sent to an application when the IME window finds no space to extend the area for the composition window. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_COMPOSITIONFULL = 0x0284,

        /// <summary>
        /// Sent to an application when the operating system is about to change the current IME. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_SELECT = 0x0285,

        /// <summary>
        /// Sent to an application when the IME gets a character of the conversion result. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_CHAR = 0x0286,

        /// <summary>
        /// Sent to an application to provide commands and request information. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_REQUEST = 0x0288,

        /// <summary>
        /// Sent to an application by the IME to notify the application of a key press and to keep message order. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_KEYDOWN = 0x0290,

        /// <summary>
        /// Sent to an application by the IME to notify the application of a key release and to keep message order. A window receives this message through its WindowProc function.
        /// </summary>
        WM_IME_KEYUP = 0x0291,

        /// <summary>
        /// The WM_MOUSEHOVER message is posted to a window when the cursor hovers over the client area of the window for the period of time specified in a prior call to TrackMouseEvent.
        /// </summary>
        WM_MOUSEHOVER = 0x02A1,

        /// <summary>
        /// The WM_MOUSELEAVE message is posted to a window when the cursor leaves the client area of the window specified in a prior call to TrackMouseEvent.
        /// </summary>
        WM_MOUSELEAVE = 0x02A3,

        /// <summary>
        /// The WM_NCMOUSEHOVER message is posted to a window when the cursor hovers over the nonclient area of the window for the period of time specified in a prior call to TrackMouseEvent.
        /// </summary>
        WM_NCMOUSEHOVER = 0x02A0,

        /// <summary>
        /// The WM_NCMOUSELEAVE message is posted to a window when the cursor leaves the nonclient area of the window specified in a prior call to TrackMouseEvent.
        /// </summary>
        WM_NCMOUSELEAVE = 0x02A2,

        /// <summary>
        /// The WM_WTSSESSION_CHANGE message notifies applications of changes in session state.
        /// </summary>
        WM_WTSSESSION_CHANGE = 0x02B1,

        WM_TABLET_FIRST = 0x02c0,
        WM_TABLET_LAST = 0x02df,

        /// <summary>
        /// <see cref="WM_DPICHANGED"> 消息是当窗口的有效点数 (dpi) 更改时发送。 DPI 是窗口的规模因子。 有多个事件可能导致 DPI 更改。
        /// </summary>
        WM_DPICHANGED = 0x02E0,

        /// <summary>
        /// For Per Monitor v2 top-level windows, this message is sent to all HWNDs in the child HWDN tree of the window that is undergoing a DPI change. This message occurs before the top-level window receives <see cref="WM_DPICHANGED"/>, and traverses the child tree from the bottom up.
        /// </summary>
        WM_DPICHANGED_BEFOREPARENT = 0x02E2,

        /// <summary>
        /// For Per Monitor v2 top-level windows, this message is sent to all HWNDs in the child HWDN tree of the window that is undergoing a DPI change. This message occurs after the top-level window receives <see cref="WM_DPICHANGED"/>, and traverses the child tree from the top down.
        /// </summary>
        WM_DPICHANGED_AFTERPARENT = 0x02E3,

        /// <summary>
        /// The WM_GETDPISCALEDSIZE message tells the operating system that the window will be sized to dimensions other than the default.
        /// </summary>
        WM_GETDPISCALEDSIZE = 0x02E4,

        /// <summary>
        /// An application sends a WM_CUT message to an edit control or combo box to delete (cut) the current selection, if any, in the edit control and copy the deleted text to the clipboard in CF_TEXT format.
        /// </summary>
        WM_CUT = 0x0300,

        /// <summary>
        /// An application sends the WM_COPY message to an edit control or combo box to copy the current selection to the clipboard in CF_TEXT format.
        /// </summary>
        WM_COPY = 0x0301,

        /// <summary>
        /// An application sends a WM_PASTE message to an edit control or combo box to copy the current content of the clipboard to the edit control at the current caret position. Data is inserted only if the clipboard contains data in CF_TEXT format.
        /// </summary>
        WM_PASTE = 0x0302,

        /// <summary>
        /// An application sends a WM_CLEAR message to an edit control or combo box to delete (clear) the current selection, if any, from the edit control.
        /// </summary>
        WM_CLEAR = 0x0303,

        /// <summary>
        /// An application sends a WM_UNDO message to an edit control to undo the last operation. When this message is sent to an edit control, the previously deleted text is restored or the previously added text is deleted.
        /// </summary>
        WM_UNDO = 0x0304,

        /// <summary>
        /// The WM_RENDERFORMAT message is sent to the clipboard owner if it has delayed rendering a specific clipboard format and if an application has requested data in that format. The clipboard owner must render data in the specified format and place it on the clipboard by calling the SetClipboardData function.
        /// </summary>
        WM_RENDERFORMAT = 0x0305,

        /// <summary>
        /// The WM_RENDERALLFORMATS message is sent to the clipboard owner before it is destroyed, if the clipboard owner has delayed rendering one or more clipboard formats. For the content of the clipboard to remain available to other applications, the clipboard owner must render data in all the formats it is capable of generating, and place the data on the clipboard by calling the SetClipboardData function.
        /// </summary>
        WM_RENDERALLFORMATS = 0x0306,

        /// <summary>
        /// The WM_DESTROYCLIPBOARD message is sent to the clipboard owner when a call to the EmptyClipboard function empties the clipboard.
        /// </summary>
        WM_DESTROYCLIPBOARD = 0x0307,

        /// <summary>
        /// The WM_DRAWCLIPBOARD message is sent to the first window in the clipboard viewer chain when the content of the clipboard changes. This enables a clipboard viewer window to display the new content of the clipboard.
        /// </summary>
        WM_DRAWCLIPBOARD = 0x0308,

        /// <summary>
        /// The WM_PAINTCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area needs repainting.
        /// </summary>
        WM_PAINTCLIPBOARD = 0x0309,

        /// <summary>
        /// The WM_VSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's vertical scroll bar. The owner should scroll the clipboard image and update the scroll bar values.
        /// </summary>
        WM_VSCROLLCLIPBOARD = 0x030A,

        /// <summary>
        /// The WM_SIZECLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area has changed size.
        /// </summary>
        WM_SIZECLIPBOARD = 0x030B,

        /// <summary>
        /// The WM_ASKCBFORMATNAME message is sent to the clipboard owner by a clipboard viewer window to request the name of a CF_OWNERDISPLAY clipboard format.
        /// </summary>
        WM_ASKCBFORMATNAME = 0x030C,

        /// <summary>
        /// The WM_CHANGECBCHAIN message is sent to the first window in the clipboard viewer chain when a window is being removed from the chain.
        /// </summary>
        WM_CHANGECBCHAIN = 0x030D,

        /// <summary>
        /// The WM_HSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window. This occurs when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's horizontal scroll bar. The owner should scroll the clipboard image and update the scroll bar values.
        /// </summary>
        WM_HSCROLLCLIPBOARD = 0x030E,

        /// <summary>
        /// This message informs a window that it is about to receive the keyboard focus, giving the window the opportunity to realize its logical palette when it receives the focus.
        /// </summary>
        WM_QUERYNEWPALETTE = 0x030F,

        /// <summary>
        /// The WM_PALETTEISCHANGING message informs applications that an application is going to realize its logical palette.
        /// </summary>
        WM_PALETTEISCHANGING = 0x0310,

        /// <summary>
        /// This message is sent by the OS to all top-level and overlapped windows after the window with the keyboard focus realizes its logical palette.
        /// This message enables windows that do not have the keyboard focus to realize their logical palettes and update their client areas.
        /// </summary>
        WM_PALETTECHANGED = 0x0311,

        /// <summary>
        /// The WM_HOTKEY message is posted when the user presses a hot key registered by the RegisterHotKey function. The message is placed at the top of the message queue associated with the thread that registered the hot key.
        /// </summary>
        WM_HOTKEY = 0x0312,

        /// <summary>
        /// 任务栏窗口右键单击时弹出菜单的消息
        /// </summary>
        WM_TASKBARRCLICK = 0x0313,

        /// <summary>
        /// The WM_PRINT message is sent to a window to request that it draw itself in the specified device context, most commonly in a printer device context.
        /// </summary>
        WM_PRINT = 0x0317,

        /// <summary>
        /// The WM_PRINTCLIENT message is sent to a window to request that it draw its client area in the specified device context, most commonly in a printer device context.
        /// </summary>
        WM_PRINTCLIENT = 0x0318,

        /// <summary>
        /// The WM_APPCOMMAND message notifies a window that the user generated an application command event, for example, by clicking an application command button using the mouse or typing an application command key on the keyboard.
        /// </summary>
        WM_APPCOMMAND = 0x0319,

        /// <summary>
        /// The WM_THEMECHANGED message is broadcast to every window following a theme change event. Examples of theme change events are the activation of a theme, the deactivation of a theme, or a transition from one theme to another.
        /// </summary>
        WM_THEMECHANGED = 0x031A,

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        WM_CLIPBOARDUPDATE = 0x031D,

        /// <summary>
        /// The system will send a window the WM_DWMCOMPOSITIONCHANGED message to indicate that the availability of desktop composition has changed.
        /// </summary>
        WM_DWMCOMPOSITIONCHANGED = 0x031E,

        /// <summary>
        /// WM_DWMNCRENDERINGCHANGED is called when the non-client area rendering status of a window has changed. Only windows that have set the flag DWM_BLURBEHIND.fTransitionOnMaximized to true will get this message.
        /// </summary>
        WM_DWMNCRENDERINGCHANGED = 0x031F,

        /// <summary>
        /// Sent to all top-level windows when the colorization color has changed.
        /// </summary>
        WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,

        /// <summary>
        /// WM_DWMWINDOWMAXIMIZEDCHANGE will let you know when a DWM composed window is maximized. You also have to register for this message as well. You'd have other window go opaque when this message is sent.
        /// </summary>
        WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,

        /// <summary>
        /// Sent to request extended title bar information. A window receives this message through its WindowProc function.
        /// </summary>
        WM_GETTITLEBARINFOEX = 0x033F,

        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,
        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,

        /// <summary>
        /// The WM_APP constant is used by applications to help define private messages, usually of the form WM_APP+X, where X is an integer value.
        /// </summary>
        WM_APP = 0x8000,

        /// <summary>
        /// <see cref="WM_USER"> 用于定义专用窗口类使用的专用消息
        /// </summary>
        WM_USER = 0x0400,

        /// <summary>
        /// An application sends the WM_CPL_LAUNCH message to Windows Control Panel to request that a Control Panel application be started.
        /// </summary>
        WM_CPL_LAUNCH = WM_USER + 0x1000,

        /// <summary>
        /// The WM_CPL_LAUNCHED message is sent when a Control Panel application, started by the WM_CPL_LAUNCH message, has closed. The WM_CPL_LAUNCHED message is sent to the window identified by the wParam parameter of the WM_CPL_LAUNCH message that started the application.
        /// </summary>
        WM_CPL_LAUNCHED = WM_USER + 0x1001,

        /// <summary>
        /// WM_SYSTIMER is a well-known yet still undocumented message. Windows uses WM_SYSTIMER for internal actions like scrolling.
        /// </summary>
        WM_SYSTIMER = 0x118,

        /// <summary>
        /// The accessibility state has changed.
        /// </summary>
        WM_HSHELL_ACCESSIBILITYSTATE = 11,

        /// <summary>
        /// The shell should activate its main window.
        /// </summary>
        WM_HSHELL_ACTIVATESHELLWINDOW = 3,

        /// <summary>
        /// The user completed an input event (for example, pressed an application command button on the mouse or an application command key on the keyboard), and the application did not handle the WM_APPCOMMAND message generated by that input.
        /// If the Shell procedure handles the WM_COMMAND message, it should not call CallNextHookEx. See the Return Value section for more information.
        /// </summary>
        WM_HSHELL_APPCOMMAND = 12,

        /// <summary>
        /// A window is being minimized or maximized. The system needs the coordinates of the minimized rectangle for the window.
        /// </summary>
        WM_HSHELL_GETMINRECT = 5,

        /// <summary>
        /// Keyboard language was changed or a new keyboard layout was loaded.
        /// </summary>
        WM_HSHELL_LANGUAGE = 8,

        /// <summary>
        /// The title of a window in the task bar has been redrawn.
        /// </summary>
        WM_HSHELL_REDRAW = 6,

        /// <summary>
        /// The user has selected the task list. A shell application that provides a task list should return TRUE to prevent Windows from starting its task list.
        /// </summary>
        WM_HSHELL_TASKMAN = 7,

        /// <summary>
        /// A top-level, unowned window has been created. The window exists when the system calls this hook.
        /// </summary>
        WM_HSHELL_WINDOWCREATED = 1,

        /// <summary>
        /// A top-level, unowned window is about to be destroyed. The window still exists when the system calls this hook.
        /// </summary>
        WM_HSHELL_WINDOWDESTROYED = 2,

        /// <summary>
        /// The activation has changed to a different top-level, unowned window.
        /// </summary>
        WM_HSHELL_WINDOWACTIVATED = 4,

        /// <summary>
        /// A top-level window is being replaced. The window exists when the system calls this hook.
        /// </summary>
        WM_HSHELL_WINDOWREPLACED = 13,
    }
}
