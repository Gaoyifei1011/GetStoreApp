namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 窗口消息
    /// 系统预留使用的消息 0x0000 - 0x03FF
    /// 被私有窗口类使用的消息 0x0400 - 0x07FF
    /// 被应用程序使用的消息 0x0800 - 0xBFFF
    /// 被应用程序使用的字符串消息 0xC000 - 0xFFFF
    /// 系统预留 大于0xFFFF
    /// </summary>
    public enum WindowMessage : int
    {
        /// <summary>
        /// 不执行任何操作。 如果应用程序想要帖子收件人窗口将忽略的邮件，则应用程序会发送WM_NULL邮件。
        /// </summary>
        WM_NULL = 0x0000,

        /// <summary>
        /// 当应用程序请求通过调用 CreateWindow 或 CreateWindowEx 函数创建窗口时发送。 (函数返回之前发送消息。) 新窗口的窗口过程在创建窗口后接收此消息，但在窗口变为可见之前。
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

        WM_SETVISIBLE = 0x0009,

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
        /// 当系统或其他应用程序发出绘制应用程序窗口部分的请求时，将发送 WM_PAINT 消息。 当调用 UpdateWindow 或 RedrawWindow 函数或
        /// DispatchMessage 函数时，应用程序通过使用 GetMessage 或 PeekMessage 函数获取 WM_PAINT 消息时发送该消息。
        /// </summary>
        WM_PAINT = 0x000F,

        /// <summary>
        /// 发送为窗口或应用程序应终止的信号。
        /// </summary>
        WM_CLOSE = 0x0010,

        /// <summary>
        /// WM_ENDSESSION 消息在系统处理 WM_QUERYENDSESSION 消息的结果后发送到应用程序。 WM_ENDSESSION 消息通知应用程序会话是否结束。
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
        /// WM_ENDSESSION 消息在系统处理 WM_QUERYENDSESSION 消息的结果后发送到应用程序。 WM_ENDSESSION 消息通知应用程序会话是否结束。
        /// </summary>
        WM_ENDSESSION = 0x0016,

        WM_SYSTEMERROR = 0x0017,

        /// <summary>
        /// 当窗口即将隐藏或显示时发送到窗口。
        /// </summary>
        WM_SHOWWINDOW = 0x0018,

        /// <summary>
        /// 在每个控件开始绘制之前，都会向其父窗口发送 WM_CTLCOLOR 通告消息，在该消息的处理函数中，可以设置控件显示文本的前景色、背景色以及字体。该消息处理函数还要求返回一个画刷的句柄，用于在控件具体的绘制之前擦除其客户区。
        /// </summary>
        WM_CTLCOLOR = 0x0019,

        /// <summary>
        /// 应用程序在更改WIN.INI文件后，将 WM_WININICHANGE 消息发送到所有顶级窗口。 SystemParametersInfo 函数在应用程序使用该函数更改WIN.INI中的设置后发送此消息。
        /// </summary>
        WM_WININICHANGE = 0x001A,

        /// <summary>
        /// 当 SystemParametersInfo 函数更改系统范围设置或策略设置发生更改时，将发送到所有顶级窗口的消息。
        /// </summary>
        WM_SETTINGCHANGE = WM_WININICHANGE,

        /// <summary>
        /// 每当用户更改设备模式设置时， WM_DEVMODECHANGE 消息都会发送到所有顶级窗口。
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
        /// 当光标处于非活动窗口中且用户按下鼠标按钮时发送。 仅当子窗口将其传递给 DefWindowProc 函数时，父窗口才会接收此消息。
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

        WM_LOGOFF = 0x0025,

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

        WM_ALTTABACTIVE = 0x0029,

        /// <summary>
        /// 每当向打印管理器队列中添加或删除作业时，都会从打印管理器发送 WM_SPOOLERSTATUS 消息。
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
        /// 当列表框或组合框被销毁或项目被 LB_DELETESTRING、 LB_RESETCONTENT、 CB_DELETESTRING或 CB_RESETCONTENT 消息删除时，发送到列表框或组合框的所有者。 系统为每个已删除的项发送 一条 WM_DELETEITEM 消息。 系统为包含非零项数据的任何已删除列表框或组合框项发送 WM_DELETEITEM 消息。
        /// </summary>
        WM_DELETEITEM = 0x002D,

        /// <summary>
        /// 由具有 LBS_WANTKEYBOARDINPUT 样式的列表框发送到其所有者以响应 WM_KEYDOWN 消息。
        /// </summary>
        WM_VKEYTOITEM = 0x002E,

        /// <summary>
        /// 由具有 LBS_WANTKEYBOARDINPUT 样式的列表框发送到其所有者，以响应 WM_CHAR 消息。
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

        WM_SHELLNOTIFY = 0x0034,

        WM_ISACTIVEICON = 0x0035,

        WM_QUERYPARKICON = 0x0036,

        /// <summary>
        /// 发送到最小化的 (标志性的) 窗口。 窗口将由用户拖动，但没有为其类定义的图标。 应用程序可以将句柄返回到图标或游标。 当用户拖动图标时，系统会显示此光标或图标。
        /// </summary>
        WM_QUERYDRAGICON = 0x0037,

        WM_WINHELP = 0x0038,

        /// <summary>
        /// 已发送到确定新项在所有者绘制组合框或列表框的排序列表中相对位置。 每当应用程序添加新项时，系统会将此消息发送到使用 CBS_SORT 或 LBS_SORT 样式创建的组合框或列表框的所有者。
        /// </summary>
        WM_COMPAREITEM = 0x0039,

        WM_FULLSCREEN = 0x003A,

        WM_CLIENTSHUTDOWN = 0x003B,

        WM_DDEMLEVENT = 0x003C,

        /// <summary>
        /// Microsoft Active Accessibility 和 Microsoft UI 自动化发送，以获取有关服务器应用程序中包含的可访问对象的信息。 应用程序永远不会直接发送此消息。
        /// </summary>
        WM_GETOBJECT = 0x003D,

        // 0x003E - 0x003F 消息未定义

        WM_TESTING = 0x0040,

        /// <summary>
        /// 当系统检测到超过 30 秒到 60 秒间隔的系统时间超过 12.5% 时，发送到所有顶级窗口。 这表示系统内存较低。
        /// </summary>
        WM_COMPACTING = 0x0041,

        WM_OTHERWINDOWCREATED = 0x0042,

        WM_OTHERWINDOWDESTROYED = 0x0043,

        /// <summary>
        /// 每当发生 COM 端口事件时，通信设备驱动程序都会发布 WM_COMMNOTIFY 消息。该消息指示窗口的输入或输出队列的状态。
        /// </summary>
        WM_COMMNOTIFY = 0x0044,

        // 0x0045 消息未定义

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
        /// 从 Win3.1 开始可能与 WM_COPYDATA 有关，现在很可能从 MSDN 中删除。每个于此相关的功能还是带着这个消息。
        /// </summary>
        WM_COPYGLOBALDATA = 0x0049,

        /// <summary>
        /// 应用程序将 WM_COPYDATA 消息发送到另一个应用程序。
        /// </summary>
        WM_COPYDATA = 0x004A,

        /// <summary>
        /// 当用户取消应用程序的日记活动时发布到应用程序。 消息使用 NULL 窗口句柄发布。
        /// </summary>
        WM_CANCELJOURNAL = 0x004B,

        // 0x004C 消息未定义

        WM_KEYF1 = 0x004D,

        /// <summary>
        /// 当发生事件或控件需要一些信息时，由公共控件发送到其父窗口。
        /// </summary>
        WM_NOTIFY = 0x004E,

        WM_ACCESS_WINDOW = 0x004F,

        /// <summary>
        /// 当用户选择新的输入语言（在键盘控制面板应用程序) 或系统任务栏上的指示器中指定的热键时，发布到具有焦点的窗口。 应用程序可以通过将消息传递给 DefWindowProc 函数来接受更改，或者拒绝更改，并阻止它立即返回) 。
        /// </summary>
        WM_INPUTLANGCHANGEREQUEST = 0x0050,

        /// <summary>
        /// 更改应用程序输入语言后，发送到最受影响的窗口。 应进行任何特定于应用程序的设置并将消息传递给 DefWindowProc 函数，该函数会将消息传递给所有第一级子窗口。 这些子窗口可以将消息传递给 DefWindowProc ，使其将消息传递给其子窗口等。
        /// </summary>
        WM_INPUTLANGCHANGE = 0x0051,

        /// <summary>
        /// 发送到已启动具有Windows帮助的训练卡的应用程序。 当用户单击可创作按钮时，该消息会通知应用程序。 应用程序通过在对 WinHelp 函数的调用中指定HELP_TCARD命令来启动训练卡。
        /// </summary>
        WM_TCARD = 0x0052,

        /// <summary>
        /// 指示用户按下了 F1 键。 如果按下 F1 时菜单处于活动状态， WM_HELP 发送到与菜单关联的窗口;否则， WM_HELP 将发送到具有键盘焦点的窗口。 如果没有窗口具有键盘焦点， WM_HELP 将发送到当前活动窗口。
        /// </summary>
        WM_HELP = 0x0053,

        /// <summary>
        /// 用户登录或关闭后发送到所有窗口。 当用户登录或关闭时，系统会更新用户特定的设置。 系统更新设置后立即发送此消息。
        /// </summary>
        WM_USERCHANGED = 0x0054,

        /// <summary>
        /// 确定窗口是否接受 WM_NOTIFY 通知消息中的 ANSI 或 Unicode 结构。 WM_NOTIFYFORMAT 消息从公共控件发送到其父窗口，从父窗口发送到公共控件。
        /// </summary>
        WM_NOTIFYFORMAT = 0x0055,

        // 0x0056 - 0x006F 消息未定义

        WM_FINALDESTROY = 0x0070,

        WM_MEASUREITEM_CLIENTDATA = 0x0071,

        // 0x0072 - 0x007A 消息未定义

        /// <summary>
        /// 通知用户希望显示上下文菜单的窗口。 用户可能已在窗口中单击鼠标右键 (右键单击) 、按 Shift+F10 或按应用程序键 (上下文菜单键) 某些键盘上可用。
        /// </summary>
        WM_CONTEXTMENU = 0x007B,

        /// <summary>
        /// 当 SetWindowLong 函数即将更改窗口的一个或多个样式时，发送到窗口。
        /// </summary>
        WM_STYLECHANGING = 0x007C,

        /// <summary>
        /// SetWindowLong 函数更改窗口的一个或多个样式后发送到窗口。
        /// </summary>
        WM_STYLECHANGED = 0x007D,

        /// <summary>
        /// 显示分辨率发生更改时 ，会将 WM_DISPLAYCHANGE 消息发送到所有窗口。
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
        /// 首次创建窗口时 ，在 WM_CREATE 消息之前发送。
        /// </summary>
        WM_NCCREATE = 0x0081,

        /// <summary>
        /// 通知窗口其非客户区域正在被销毁。 DestroyWindow 函数将 WM_NCDESTROY 消息发送到 WM_DESTROY 消息后面的窗口。WM_DESTROY 用于释放与窗口关联的已分配内存对象。
        /// 子窗口被销毁后，将发送 WM_NCDESTROY 消息。 相比之下， WM_DESTROY 在销毁子窗口之前发送。
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
        /// WM_NCPAINT 消息在必须绘制其框架时发送到窗口。
        /// </summary>
        WM_NCPAINT = 0x0085,

        /// <summary>
        /// 当需要更改其非client 区域以指示活动或非活动状态时，发送到窗口。
        /// </summary>
        WM_NCACTIVATE = 0x0086,

        /// <summary>
        /// 发送到与控件关联的窗口过程。 默认情况下，系统将处理控件的所有键盘输入;系统将某些类型的键盘输入解释为对话框导航键。 若要替代此默认行为，控件可以响应 WM_GETDLGCODE 消息，以指示要处理其本身的输入类型。
        /// </summary>
        WM_GETDLGCODE = 0x0087,

        /// <summary>
        /// WM_SYNCPAINT 消息用于同步绘制，同时避免链接独立的 GUI 线程。
        /// </summary>
        WM_SYNCPAINT = 0x0088,

        WM_SYNCTASK = 0x0089,

        // 0x008A 消息未定义

        WM_KLUDGEMINRECT = 0x008B,

        WM_LPKDRAWSWITCHWND = 0x008C,

        // 0x008D - 0x008F 消息未定义

        WM_UAHDESTROYWINDOW = 0x0090,

        WM_UAHDRAWMENU = 0x0091,

        WM_UAHDRAWMENUITEM = 0x0092,

        WM_UAHINITMENU = 0x0093,

        WM_UAHMEASUREMENUITEM = 0x0094,

        WM_UAHNCPAINTMENUPOPUP = 0x0095,

        WM_UAHUPDATE = 0x0096,

        // 0x0097 - 0x009F 消息未定义

        /// <summary>
        /// 当光标在窗口的非工作区内移动时发布到窗口。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCMOUSEMOVE = 0x00A0,

        /// <summary>
        /// 当用户在光标位于窗口的非工作区内时按下鼠标左键时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCLBUTTONDOWN = 0x00A1,

        /// <summary>
        /// 当用户释放鼠标左键时，光标位于窗口的非工作区内时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCLBUTTONUP = 0x00A2,

        /// <summary>
        /// 当用户在光标位于窗口的非工作区内时双击鼠标左键时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCLBUTTONDBLCLK = 0x00A3,

        /// <summary>
        /// 当用户按下鼠标右键时，光标位于窗口的非工作区内时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCRBUTTONDOWN = 0x00A4,

        /// <summary>
        /// 当用户松开鼠标右键时，光标位于窗口的非工作区内时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCRBUTTONUP = 0x00A5,

        /// <summary>
        /// 当用户在光标位于窗口的非工作区内时双击鼠标右键时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCRBUTTONDBLCLK = 0x00A6,

        /// <summary>
        /// 当用户按下鼠标中键时光标位于窗口的非工作区内时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCMBUTTONDOWN = 0x00A7,

        /// <summary>
        /// 当用户松开鼠标中键时，光标位于窗口的非工作区内时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCMBUTTONUP = 0x00A8,

        /// <summary>
        /// 当用户在光标位于窗口的非工作区内时双击鼠标中键时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCMBUTTONDBLCLK = 0x00A9,

        // 0x00AA 消息未定义

        /// <summary>
        /// 当用户按下第一个或第二个 X 按钮时，光标位于窗口的非工作区时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则 不会 发布此消息。
        /// </summary>
        WM_NCXBUTTONDOWN = 0x00AB,

        /// <summary>
        /// 当用户释放第一个或第二个 X 按钮时，光标位于窗口的非工作区时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则 不会 发布此消息。
        /// </summary>
        WM_NCXBUTTONUP = 0x00AC,

        /// <summary>
        /// 当用户在光标位于窗口的非工作区时双击第一个或第二个 X 按钮时发布。 此消息将发布到包含光标的窗口。 如果窗口捕获了鼠标，则不会发布此消息。
        /// </summary>
        WM_NCXBUTTONDBLCLK = 0x00AD,

        WM_NCUAHDRAWCAPTION = 0x00AE,

        WM_NCUAHDRAWFRAME = 0x00AF,

        /// <summary>
        /// 获取编辑控件中当前所选内容) TCHARs 中 (开始和结束字符位置。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_GETSEL = 0x00B0,

        /// <summary>
        /// 在编辑控件中选择一系列字符。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_SETSEL = 0x00B1,

        /// <summary>
        /// 获取编辑控件 的格式设置矩形 。 格式设置矩形是控件在其中绘制文本的限制矩形。 限制矩形与编辑控件窗口的大小无关。 可以将此消息发送到编辑控件或富编辑控件。
        /// </summary>
        EM_GETRECT = 0x00B2,

        /// <summary>
        /// 设置多行编辑控件 的格式矩形 。 格式设置矩形是控件在其中绘制文本的限制矩形。 限制矩形与编辑控件窗口的大小无关。
        /// 此消息仅由多行编辑控件处理。 可以将此消息发送到编辑控件或富编辑控件。
        /// </summary>
        EM_SETRECT = 0x00B3,

        /// <summary>
        /// 设置多行编辑控件 的格式矩形 。 EM_SETRECTNP消息与EM_SETRECT消息相同，只是EM_SETRECTNP不会重绘编辑控件窗口。
        /// 格式设置矩形是控件在其中绘制文本的限制矩形。 限制矩形与编辑控件窗口的大小无关。
        /// 此消息仅由多行编辑控件处理。 可以将此消息发送到编辑控件或富编辑控件。
        /// </summary>
        EM_SETRECTNP = 0x00B4,

        /// <summary>
        /// 在多行编辑控件中垂直滚动文本。 此消息等效于将 WM_VSCROLL 消息发送到编辑控件。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_SCROLL = 0x00B5,

        /// <summary>
        /// 滚动多行编辑控件中的文本。
        /// </summary>
        EM_LINESCROLL = 0x00B6,

        /// <summary>
        /// 将插入点滚动到编辑控件的视图中。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_SCROLLCARET = 0x00B7,

        /// <summary>
        /// 获取编辑控件的修改标志的状态。 标志指示是否已修改编辑控件的内容。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_GETMODIFY = 0x00B8,

        /// <summary>
        /// 设置或清除编辑控件的修改标志。 修改标志指示编辑控件中的文本是否已修改。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_SETMODIFY = 0x00B9,

        /// <summary>
        /// 获取多行编辑控件中的行数。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_GETLINECOUNT = 0x00BA,

        /// <summary>
        /// 获取多行编辑控件中指定行的第一个字符的字符索引。 字符索引是从编辑控件的开头开始的字符的从零开始的索引。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_LINEINDEX = 0x00BB,

        /// <summary>
        /// 设置多行编辑控件将使用的内存的句柄。
        /// </summary>
        EM_SETHANDLE = 0x00BC,

        /// <summary>
        /// 获取当前为多行编辑控件的文本分配的内存的句柄。
        /// </summary>
        EM_GETHANDLE = 0x00BD,

        /// <summary>
        /// 获取多行编辑控件垂直滚动条中滚动框 (拇指) 的位置。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_GETTHUMB = 0x00BE,

        // 0x00BF - 0x00C0 消息未定义

        /// <summary>
        /// 检索编辑控件中行的长度（以字符为单位）。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_LINELENGTH = 0x00C1,

        /// <summary>
        /// 将编辑控件中的选定文本或丰富的编辑控件替换为指定的文本。
        /// </summary>
        EM_REPLACESEL = 0x00C2,

        /// <summary>
        /// 设置富编辑控件中所选文本的字体。
        /// </summary>
        EM_SETFONT = 0x00C3,

        /// <summary>
        /// 从编辑控件复制文本行，并将其置于指定的缓冲区中。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_GETLINE = 0x00C4,

        /// <summary>
        /// 设置编辑控件的文本限制。 文本限制是用户可以在编辑控件中键入的最大文本量（以 TCHAR 为单位）。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// 对于编辑控件和 Microsoft Rich Edit 1.0，将使用字节。 对于 Microsoft Rich Edit 2.0 及更高版本，将使用字符。
        /// </summary>
        EM_LIMITTEXT = 0x00C5,

        /// <summary>
        /// 设置编辑控件的文本限制。 文本限制是用户可在编辑控件中键入的最大文本量（ 在 TCHARs 中）。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// 对于编辑控件和 Microsoft Rich Edit 1.0，将使用字节。 对于 Microsoft Rich Edit 2.0 及更高版本，将使用字符。
        /// EM_SETLIMITTEXT 消息与 EM_LIMITTEXT 消息相同。
        /// </summary>
        EM_SETLIMITTEXT = EM_LIMITTEXT,

        /// <summary>
        /// 确定编辑控件的撤消队列中是否有任何操作。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_CANUNDO = 0x00C6,

        /// <summary>
        /// 此消息撤消控件队列中最后一次编辑控件操作。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_UNDO = 0x00C7,

        /// <summary>
        /// 设置一个标志，用于确定多行编辑控件是否包含软换行符。 软换行符由两个回车符和一个换行符组成，插入到由于单词包装而断开的行的末尾。
        /// </summary>
        EM_FMTLINES = 0x00C8,

        /// <summary>
        /// 获取包含多行编辑控件中指定字符索引的行的索引。 字符索引是从编辑控件的开头开始的字符的从零开始的索引。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_LINEFROMCHAR = 0x00C9,

        /// <summary>
        /// 本操作现已不支持，请使用 EM_SETWORDBREAKPROC
        /// </summary>
        EM_SETWORDBREAK = 0x00CA,

        /// <summary>
        /// EM_SETTABSTOPS消息设置多行编辑控件中的制表位。 当文本复制到控件时，文本中的任何制表符都会导致空间生成到下一个制表位。
        /// 此消息仅由多行编辑控件处理。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_SETTABSTOPS = 0x00CB,

        /// <summary>
        /// 设置或删除编辑控件的密码字符。 设置密码字符时，将显示该字符代替用户键入的字符。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_SETPASSWORDCHAR = 0x00CC,

        /// <summary>
        /// 重置编辑控件的撤消标志。 每当可以撤消编辑控件中的操作时，都会设置撤消标志。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_EMPTYUNDOBUFFER = 0x00CD,

        /// <summary>
        /// 获取多行编辑控件中最上端可见行的从零开始的索引。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_GETFIRSTVISIBLELINE = 0x00CE,

        /// <summary>
        /// 设置或删除编辑控件的只读样式 (ES_READONLY) 。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_SETREADONLY = 0x00CF,

        /// <summary>
        /// 将编辑控件的默认 Wordwrap 函数替换为应用程序定义的 Wordwrap 函数。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_SETWORDBREAKPROC = 0x00D0,

        /// <summary>
        /// 获取当前 Wordwrap 函数的地址。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_GETWORDBREAKPROC = 0x00D1,

        /// <summary>
        /// 获取编辑控件在用户输入文本时显示的密码字符。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_GETPASSWORDCHAR = 0x00D2,

        /// <summary>
        /// 设置编辑控件的左右边距的宽度。 消息重新绘制控件以反映新边距。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_SETMARGINS = 0x00D3,

        /// <summary>
        /// 获取编辑控件的左右边距的宽度。
        /// </summary>
        EM_GETMARGINS = 0x00D4,

        /// <summary>
        /// 获取编辑控件的当前文本限制。 可以将此消息发送到编辑控件或丰富的编辑控件。
        /// </summary>
        EM_GETLIMITTEXT = 0x00D5,

        /// <summary>
        /// 检索编辑控件中指定字符的工作区坐标。 可以将此消息发送到编辑控件或富编辑控件。
        /// </summary>
        EM_POSFROMCHAR = 0x00D6,

        /// <summary>
        /// 获取有关编辑控件工作区中最靠近指定点的字符的信息。 可以将此消息发送到编辑控件或富编辑控件。
        /// </summary>
        EM_CHARFROMPOS = 0x00D7,

        /// <summary>
        /// 设置状态标志，确定编辑控件如何与输入法编辑器交互 (IME) 。
        /// </summary>
        EM_SETIMESTATUS = 0x00D8,

        /// <summary>
        /// 获取一组状态标志，指示编辑控件如何与输入法编辑器交互 (IME) 。
        /// </summary>
        EM_GETIMESTATUS = 0x00D9,

        EM_MSGMAX = 0x00DA,

        // 0x00DB - 0x00DF 消息未定义

        /// <summary>
        /// 发送 SBM_SETPOS 消息以设置滚动框的位置 (拇指) ，如果请求，请重新绘制滚动条以反映滚动框的新位置。
        /// 应用程序不应直接发送此消息。 相反，它们应使用 SetScrollPos 函数。 窗口通过其 WndProc 函数接收此消息。 实现自定义滚动条控件的应用程序必须响应这些消息， 以便 SetScrollPos 函数正常工作。
        /// </summary>
        SBM_SETPOS = 0x00E0,

        /// <summary>
        /// 发送 SBM_GETPOS 消息以检索滚动条控件滚动框的当前位置。 当前位置是一个相对值，该值取决于当前滚动范围。 例如，如果滚动范围为 0 到 100，并且滚动框位于条形图中间，则当前位置为 50。
        /// 应用程序不应直接发送此消息。 相反，它们应使用 GetScrollPos 函数。 窗口通过其 WndProc 函数接收此消息。 实现自定义滚动条控件的应用程序必须响应这些消息， 以便 GetScrollPos 函数正常运行。
        /// </summary>
        SBM_GETPOS = 0x00E1,

        /// <summary>
        /// 发送 SBM_SETRANGE 消息以设置滚动条控件的最小和最大位置值。
        /// 应用程序不应直接发送此消息。 相反，它们应使用 SetScrollRange 函数。 窗口通过其 WndProc 函数接收此消息。 实现自定义滚动条控件的应用程序必须响应这些消息， 才能使 SetScrollRange 函数正常工作。
        /// </summary>
        SBM_SETRANGE = 0x00E2,

        /// <summary>
        /// 发送 SBM_GETRANGE 消息以检索滚动条控件的最小和最大位置值。
        /// 应用程序不应直接发送此消息。 相反，它们应使用 GetScrollRange 函数。 窗口通过其 WndProc 函数接收此消息。 实现自定义滚动条控件的应用程序必须响应这些消息， 以便 GetScrollRange 函数正常工作。
        /// </summary>
        SBM_GETRANGE = 0x00E3,

        /// <summary>
        /// 应用程序发送 SBM_ENABLE_ARROWS 消息，以启用或禁用滚动条控件的一个或多个箭头。
        /// </summary>
        SBM_ENABLE_ARROWS = 0x00E4,

        // 0x00E5 消息未定义

        /// <summary>
        /// 应用程序将 SBM_SETRANGEREDRAW 消息发送到滚动条控件，以设置最小和最大位置值并重新绘制控件。
        /// </summary>
        SBM_SETRANGEREDRAW = 0x00E6,

        // 0x00E7 - 0x00E8 消息未定义

        /// <summary>
        /// 发送 SBM_SETSCROLLINFO 消息以设置滚动条的参数。
        /// 应用程序不应直接发送此消息。 相反，它们应使用 SetScrollInfo 函数。 窗口通过其 WndProc 函数接收此消息。 实现自定义滚动条控件的应用程序必须响应这些消息， SetScrollInfo 函数才能正常运行。
        /// </summary>
        SBM_SETSCROLLINFO = 0x00E9,

        /// <summary>
        /// 发送 SBM_GETSCROLLINFO 消息以检索滚动条的参数。
        /// 应用程序不应直接发送此消息。 相反，它们应使用 GetScrollInfo 函数。 窗口通过其 WndProc 函数接收此消息。 实现自定义滚动条控件的应用程序必须响应这些消息，以便 GetScrollInfo 函数正常工作。
        /// </summary>
        SBM_GETSCROLLINFO = 0x00EA,

        /// <summary>
        /// 应用程序发送以检索有关指定滚动条的信息。
        /// </summary>
        SBM_GETSCROLLBARINFO = 0x00EB,

        // 0x00EC - 0x00EF 消息未定义

        /// <summary>
        /// 获取单选按钮或复选框的选中状态。 可以显式发送此消息或使用 Button_GetCheck 宏。
        /// </summary>
        BM_GETCHECK = 0x00F0,

        /// <summary>
        /// 设置单选按钮或复选框的选中状态。 可以显式发送此消息，也可以使用 Button_SetCheck 宏。
        /// </summary>
        BM_SETCHECK = 0x00F1,

        /// <summary>
        /// 检索按钮或复选框的状态。 可以显式发送此消息或使用 Button_GetState 宏。
        /// </summary>
        BM_GETSTATE = 0x00F2,

        /// <summary>
        /// 设置按钮的突出显示状态。 突出显示状态指示按钮是否突出显示，就像用户已推送按钮一样。 可以显式发送此消息或使用 Button_SetState 宏。
        /// </summary>
        BM_SETSTATE = 0x00F3,

        /// <summary>
        /// 设置按钮的样式。 可以显式发送此消息或使用 Button_SetStyle 宏。
        /// </summary>
        BM_SETSTYLE = 0x00F4,

        /// <summary>
        /// 模拟用户单击按钮。 此消息会导致该按钮接收 WM_LBUTTONDOWN 和 WM_LBUTTONUP 消息，以及按钮的父窗口接收 BN_CLICKED 通知代码。
        /// </summary>
        BM_CLICK = 0x00F5,

        /// <summary>
        /// 检索与按钮关联的图像 (图标或位图) 的句柄。
        /// </summary>
        BM_GETIMAGE = 0x00F6,

        /// <summary>
        /// 将新图像 (图标或位图) 与按钮相关联。
        /// </summary>
        BM_SETIMAGE = 0x00F7,

        /// <summary>
        /// 在单选按钮上设置一个标志，用于控制按钮接收焦点时生成 BN_CLICKED 消息。
        /// </summary>
        BM_SETDONTCLICK = 0x00F8,

        // 0x00F9 - 0x00FD 消息未定义

        /// <summary>
        /// 发送到注册以接收原始输入的窗口。
        /// 仅当应用程序调用 RegisterRawInputDevices 和 RIDEV_DEVNOTIFY 标志后，原始输入通知才可用。
        /// 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_INPUT_DEVICE_CHANGE = 0x00FE,

        /// <summary>
        /// 发送到正在获取原始输入的窗口。
        /// </summary>
        WM_INPUT = 0x00FF,

        /// <summary>
        /// This message filters for keyboard messages.
        /// </summary>
        WM_KEYFIRST = 0x0100,

        /// <summary>
        /// 按下非系统键时，使用键盘焦点发布到窗口。 非系统键是未按下 ALT 键时按下的键。
        /// </summary>
        WM_KEYDOWN = WM_KEYFIRST,

        /// <summary>
        /// 当释放非系统键时，发布到具有键盘焦点的窗口。 非系统键是当 Alt 键未按下时按下的键，或者当窗口具有键盘焦点时按下的键盘键。
        /// </summary>
        WM_KEYUP = 0x0101,

        /// <summary>
        /// 当 TranslateMessage 函数翻译 WM_KEYDOWN 消息时，发布到具有键盘焦点的窗口。 WM_CHAR 消息包含按下的键的字符代码。
        /// </summary>
        WM_CHAR = 0x0102,

        /// <summary>
        /// 当 TranslateMessage 函数翻译 WM_KEYUP 消息时，发布到具有键盘焦点的窗口。 WM_DEADCHAR 指定死键生成的字符代码。 死键是生成字符（如 umlaut (双点) ）的键，该键与另一个字符结合使用以形成复合字符。 例如，通过键入 umlaut 字符的死键，然后键入 O 键，生成 umlaut-O 字符 ( ) 。
        /// </summary>
        WM_DEADCHAR = 0x0103,

        /// <summary>
        /// 当用户按下 F10 键 (激活菜单栏) 或按住 Alt 键，然后按另一个键时，发布到具有键盘焦点的窗口。 当当前没有窗口具有键盘焦点时，也会发生这种情况;在这种情况下， WM_SYSKEYDOWN 消息发送到活动窗口。 接收消息的窗口可以通过检查 lParam 参数中的上下文代码来区分这两个上下文。
        /// </summary>
        WM_SYSKEYDOWN = 0x0104,

        /// <summary>
        /// 当用户释放按下 Alt 键时按下的键时，发布到具有键盘焦点的窗口。 当没有窗口当前具有键盘焦点时，也会发生这种情况;在这种情况下， WM_SYSKEYUP 消息将发送到活动窗口。 接收消息的窗口可以通过检查 lParam 参数中的上下文代码来区分这两个上下文。
        /// </summary>
        WM_SYSKEYUP = 0x0105,

        /// <summary>
        /// 当 TranslateMessage 函数翻译 WM_SYSKEYDOWN 消息时，使用键盘焦点发布到窗口。 它指定系统字符键的字符代码，即在 ALT 键关闭时按下的字符键。
        /// </summary>
        WM_SYSCHAR = 0x0106,

        /// <summary>
        /// 当 TranslateMessage 函数翻译 WM_SYSKEYDOWN 消息时，使用键盘焦点发送到窗口。 WM_SYSDEADCHAR 指定系统死键的字符代码，即按住 Alt 键时按下的死键。
        /// </summary>
        WM_SYSDEADCHAR = 0x0107,

        WM_YOMICHAR = 0x0108,

        /// <summary>
        /// 应用程序可以使用 WM_UNICHAR 消息将输入发布到其他窗口。 此消息包含按下的键的字符代码。 (测试目标应用是否可以通过将 wParam 设置为 UNICODE_NOCHAR.) 的消息来处理 WM_UNICHAR 消息
        /// </summary>
        WM_UNICHAR = 0x0109,

        /// <summary>
        /// 此消息筛选键盘消息。
        /// </summary>
        WM_KEYLAST = WM_UNICHAR,

        WM_CONVERTREQUEST = 0x010A,

        WM_CONVERTRESULT = 0x010B,

        WM_INTERIM = 0x010C,

        /// <summary>
        /// 在 IME 生成合成字符串之前立即发送，因为击键。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_STARTCOMPOSITION = 0x010D,

        /// <summary>
        /// 当 IME 结束组合时发送到应用程序。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_ENDCOMPOSITION = 0x010E,

        /// <summary>
        /// 当 IME 因击键而更改组合状态时发送到应用程序。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_COMPOSITION = 0x010F,

        WM_IME_KEYLAST = WM_IME_COMPOSITION,

        /// <summary>
        /// 在显示对话框之前立即发送到对话框过程。 对话框过程通常使用此消息来初始化控件并执行影响对话框外观的任何其他初始化任务。
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
        /// 在计时器过期时发布到安装线程的消息队列。 消息由 GetMessage 或 PeekMessage 函数发布。
        /// </summary>
        WM_TIMER = 0x0113,

        /// <summary>
        /// 当窗口的标准水平滚动条中发生滚动事件时， 会将 WM_HSCROLL 消息发送到窗口。 当控件中发生滚动事件时，此消息也会发送到水平滚动条控件的所有者。
        /// </summary>
        WM_HSCROLL = 0x0114,

        /// <summary>
        /// 当窗口的标准垂直滚动条中发生滚动事件时， 会将 WM_VSCROLL 消息发送到窗口。 当控件中发生滚动事件时，此消息也会发送到垂直滚动条控件的所有者。
        /// </summary>
        WM_VSCROLL = 0x0115,

        /// <summary>
        /// 当菜单即将处于活动状态时发送。 当用户单击菜单栏上的项或按菜单键时发生。 这样，应用程序就可以在显示菜单之前对其进行修改。
        /// </summary>
        WM_INITMENU = 0x0116,

        /// <summary>
        /// 下拉菜单或子菜单即将处于活动状态时发送。 这样，应用程序就可以在显示菜单之前修改菜单，而无需更改整个菜单。
        /// </summary>
        WM_INITMENUPOPUP = 0x0117,

        /// <summary>
        /// WM_SYSTIMER 是一个众所周知但仍未记录的消息。Windows 使用 WM_SYSTIMER 进行滚动等内部操作。
        /// </summary>
        WM_SYSTIMER = 0x0118,

        /// <summary>
        /// 传递有关手势的信息。
        /// </summary>
        WM_GESTURE = 0x0119,

        /// <summary>
        /// 提供设置手势配置的机会。
        /// </summary>
        WM_GESTURENOTIFY = 0x011A,

        WM_GESTUREINPUT = 0x011B,

        WM_GESTURENOTIFIED = 0x011C,

        // 0x011D - 0x011E 消息未定义

        /// <summary>
        /// 当用户选择菜单项时发送到菜单的所有者窗口。
        /// </summary>
        WM_MENUSELECT = 0x011F,

        /// <summary>
        /// 当菜单处于活动状态并且用户按下与任何助记键或快捷键不对应的键时发送。 此消息将发送到拥有菜单的窗口。
        /// </summary>
        WM_MENUCHAR = 0x0120,

        /// <summary>
        /// 发送到进入空闲状态的模式对话框或菜单的所有者窗口。 模式对话框或菜单在处理一个或多个以前的消息后没有消息在队列中等待时进入空闲状态。
        /// </summary>
        WM_ENTERIDLE = 0x0121,

        /// <summary>
        /// 当用户在光标位于菜单项上时释放鼠标右键时发送。
        /// </summary>
        WM_MENURBUTTONUP = 0x0122,

        /// <summary>
        /// 当用户拖动菜单项时发送到拖放菜单的所有者。
        /// </summary>
        WM_MENUDRAG = 0x0123,

        /// <summary>
        /// 当鼠标光标进入菜单项或从项目中心移动到项的顶部或底部时，发送到拖放菜单的所有者。
        /// </summary>
        WM_MENUGETOBJECT = 0x0124,

        /// <summary>
        /// 下拉菜单或子菜单被销毁时发送。
        /// </summary>
        WM_UNINITMENUPOPUP = 0x0125,

        /// <summary>
        /// 当用户从菜单中做出选择时发送。
        /// </summary>
        WM_MENUCOMMAND = 0x0126,

        /// <summary>
        /// 应用程序发送 WM_CHANGEUISTATE 消息以指示应更改 UI 状态。
        /// </summary>
        WM_CHANGEUISTATE = 0x0127,

        /// <summary>
        /// 应用程序发送 WM_UPDATEUISTATE 消息以更改指定窗口及其所有子窗口的 UI 状态。
        /// </summary>
        WM_UPDATEUISTATE = 0x0128,

        /// <summary>
        /// 应用程序发送 WM_QUERYUISTATE 消息以检索窗口的 UI 状态。
        /// </summary>
        WM_QUERYUISTATE = 0x0129,

        // 0x012A - 0x0130 消息未定义

        WM_LBTRACKPOINT = 0x0131,

        /// <summary>
        /// 在系统绘制列表框之前发送到列表框的父窗口。 通过响应此消息，父窗口可以使用指定的显示设备上下文句柄设置列表框的文本和背景色。
        /// </summary>
        WM_CTLCOLORMSGBOX = 0x0132,

        /// <summary>
        /// 非只读或禁用的编辑控件将在控件即将绘制时将 WM_CTLCOLOREDIT 消息发送到其父窗口。 通过响应此消息，父窗口可以使用指定的设备上下文句柄来设置编辑控件的文本和背景色。
        /// </summary>
        WM_CTLCOLOREDIT = 0x0133,

        /// <summary>
        /// 在系统绘制列表框之前发送到列表框的父窗口。 通过响应此消息，父窗口可以使用指定的显示设备上下文句柄设置列表框的文本和背景色。
        /// </summary>
        WM_CTLCOLORLISTBOX = 0x0134,

        /// <summary>
        /// 在绘制按钮之前， WM_CTLCOLORBTN 消息发送到按钮的父窗口。 父窗口可以更改按钮的文本和背景色。 但是，只有所有者绘制的按钮响应处理此消息的父窗口。
        /// </summary>
        WM_CTLCOLORBTN = 0x0135,

        /// <summary>
        /// 在系统绘制对话框之前发送到对话框。 通过响应此消息，对话框可以使用指定的显示设备上下文句柄设置其文本和背景颜色。
        /// </summary>
        WM_CTLCOLORDLG = 0x0136,

        /// <summary>
        /// 当控件即将绘制时， WM_CTLCOLORSCROLLBAR 消息将发送到滚动条控件的父窗口。 通过响应此消息，父窗口可以使用显示上下文句柄设置滚动条控件的背景色。
        /// </summary>
        WM_CTLCOLORSCROLLBAR = 0x0137,

        /// <summary>
        /// 静态控件或只读或禁用的编辑控件在即将绘制控件时将 WM_CTLCOLORSTATIC 消息发送到其父窗口。 通过响应此消息，父窗口可以使用指定的设备上下文句柄来设置静态控件的文本前景和背景色。
        /// </summary>
        WM_CTLCOLORSTATIC = 0x0138,

        // 0x0139 - 0x013F 消息未定义

        /// <summary>
        /// 获取组合框的编辑控件中当前所选内容的起始和结束字符位置。
        /// </summary>
        CB_GETEDITSEL = 0x0140,

        /// <summary>
        /// 限制用户可在组合框的编辑控件中键入的文本长度。
        /// </summary>
        CB_LIMITTEXT = 0x0141,

        /// <summary>
        /// 应用程序发送 CB_SETEDITSEL 消息以在组合框的编辑控件中选择字符。
        /// </summary>
        CB_SETEDITSEL = 0x0142,

        /// <summary>
        /// 将字符串添加到组合框的列表框中。 如果组合框没有 CBS_SORT 样式，则字符串将添加到列表的末尾。 否则，字符串将插入到列表中，并对列表进行排序。
        /// </summary>
        CB_ADDSTRING = 0x0143,

        /// <summary>
        /// 删除组合框列表框中的字符串。
        /// </summary>
        CB_DELETESTRING = 0x0144,

        /// <summary>
        /// 将名称添加到组合框显示的列表。 该消息将添加与指定字符串和文件属性集匹配的目录和文件的名称。 CB_DIR 还可以将映射的驱动器号添加到列表中。
        /// </summary>
        CB_DIR = 0x0145,

        /// <summary>
        /// 获取组合框列表框中的项数。
        /// </summary>
        CB_GETCOUNT = 0x0146,

        /// <summary>
        /// 应用程序在组合框的列表框中发送 CB_GETCURSEL 消息以检索当前所选项（如果有）的索引。
        /// </summary>
        CB_GETCURSEL = 0x0147,

        /// <summary>
        /// 从组合框列表中获取字符串。
        /// </summary>
        CB_GETLBTEXT = 0x0148,

        /// <summary>
        /// 获取组合框中字符串的长度（以字符为单位）。
        /// </summary>
        CB_GETLBTEXTLEN = 0x0149,

        /// <summary>
        /// 将字符串或项数据插入组合框列表中。 与 CB_ADDSTRING 消息不同， CB_INSERTSTRING 消息不会导致对具有 CBS_SORT 样式的列表进行排序。
        /// </summary>
        CB_INSERTSTRING = 0x014A,

        /// <summary>
        /// 从列表框中删除所有项，并编辑组合框的控件。
        /// </summary>
        CB_RESETCONTENT = 0x014B,

        /// <summary>
        /// 在组合框的列表框中搜索以指定字符串中的字符开头的项目。
        /// </summary>
        CB_FINDSTRING = 0x014C,

        /// <summary>
        /// 在组合框列表中搜索以指定字符串中的字符开头的项。 如果找到匹配项，则会将其选中并复制到编辑控件。
        /// </summary>
        CB_SELECTSTRING = 0x014D,

        /// <summary>
        /// 应用程序发送 CB_SETCURSEL 消息，以在组合框中选择字符串。 如有必要，列表会将字符串滚动到视图中。 组合框的编辑控件中的文本将更改以反映新选定内容，并删除列表中任何以前的选定内容。
        /// </summary>
        CB_SETCURSEL = 0x014E,

        /// <summary>
        /// 应用程序发送 CB_SHOWDROPDOWN 消息以显示或隐藏具有 CBS_DROPDOWN 或 CBS_DROPDOWNLIST 样式的组合框的列表框。
        /// </summary>
        CB_SHOWDROPDOWN = 0x014F,

        /// <summary>
        /// 应用程序将 CB_GETITEMDATA 消息发送到组合框，以检索与组合框中指定项关联的应用程序提供的值。
        /// </summary>
        CB_GETITEMDATA = 0x0150,

        /// <summary>
        /// 应用程序发送 CB_SETITEMDATA 消息以设置与组合框中指定项关联的值。
        /// </summary>
        CB_SETITEMDATA = 0x0151,

        /// <summary>
        /// 应用程序发送 CB_GETDROPPEDCONTROLRECT 消息以检索处于下拉状态的组合框的屏幕坐标。
        /// </summary>
        CB_GETDROPPEDCONTROLRECT = 0x0152,

        /// <summary>
        /// 应用程序发送 CB_SETITEMHEIGHT 消息以设置组合框中的列表项或选择字段的高度。
        /// </summary>
        CB_SETITEMHEIGHT = 0x0153,

        /// <summary>
        /// 确定组合框中列表项或选择字段的高度。
        /// </summary>
        CB_GETITEMHEIGHT = 0x0154,

        /// <summary>
        /// 应用程序发送 CB_SETEXTENDEDUI 消息，以选择具有 CBS_DROPDOWN 或 CBS_DROPDOWNLIST 样式的组合框的默认 UI 或扩展 UI。
        /// </summary>
        CB_SETEXTENDEDUI = 0x0155,

        /// <summary>
        /// 确定组合框是具有默认用户界面还是扩展用户界面。
        /// </summary>
        CB_GETEXTENDEDUI = 0x0156,

        /// <summary>
        /// 确定组合框的列表框是否下拉。
        /// </summary>
        CB_GETDROPPEDSTATE = 0x0157,

        /// <summary>
        /// 查找与 lParam 参数中指定的字符串匹配的组合框中的第一个列表框字符串。
        /// </summary>
        CB_FINDSTRINGEXACT = 0x0158,

        /// <summary>
        /// 应用程序发送 CB_SETLOCALE 消息以设置组合框的当前区域设置。 如果组合框具有使用 CB_ADDSTRING 添加 CBS_SORT 样式和字符串，则组合框的区域设置会影响列表项的排序方式。
        /// </summary>
        CB_SETLOCALE = 0x0159,

        /// <summary>
        /// 获取组合框的当前区域设置。 区域设置用于确定使用 CB_ADDSTRING 消息添加 CBS_SORT 样式和文本的组合框显示文本的正确排序顺序。
        /// </summary>
        CB_GETLOCALE = 0x015A,

        /// <summary>
        /// 应用程序发送 CB_GETTOPINDEX 消息以检索组合框列表框部分第一个可见项的从零开始的索引。 最初，索引为 0 的项位于列表框顶部，但如果列表框内容已滚动，则其他项可能位于顶部。
        /// </summary>
        CB_GETTOPINDEX = 0x015B,

        /// <summary>
        /// 应用程序发送 CB_SETTOPINDEX 消息，以确保特定项在组合框的列表框中可见。 系统滚动列表框内容，以便指定的项显示在列表框顶部或已达到最大滚动范围。
        /// </summary>
        CB_SETTOPINDEX = 0x015C,

        /// <summary>
        /// 获取列表框可以水平滚动的宽度（以像素为单位）， (可滚动宽度) 。 仅当列表框具有水平滚动条时，此选项才适用。
        /// </summary>
        CB_GETHORIZONTALEXTENT = 0x015D,

        /// <summary>
        /// 应用程序发送 CB_SETHORIZONTALEXTENT 消息以设置宽度（以像素为单位），列表框可以水平滚动 (可滚动宽度) 。 如果列表框的宽度小于此值，则水平滚动条水平滚动列表框中的项目。 如果列表框的宽度等于或大于此值，则隐藏水平滚动条，或者，如果组合框具有 CBS_DISABLENOSCROLL 样式，则禁用。
        /// </summary>
        CB_SETHORIZONTALEXTENT = 0x015E,

        /// <summary>
        /// 获取具有 CBS_DROPDOWN 或 CBS_DROPDOWNLIST 样式的组合框列表框的最小允许宽度（以像素为单位）。
        /// </summary>
        CB_GETDROPPEDWIDTH = 0x015F,

        /// <summary>
        /// 应用程序发送 CB_SETDROPPEDWIDTH 消息，以设置具有 CBS_DROPDOWN 或 CBS_DROPDOWNLIST 样式的组合框列表框的最小允许宽度（以像素为单位）。
        /// </summary>
        CB_SETDROPPEDWIDTH = 0x0160,

        /// <summary>
        /// 应用程序在将大量项目添加到组合框的列表框部分之前发送 CB_INITSTORAGE 消息。 此消息分配用于存储列表框项的内存。
        /// </summary>
        CB_INITSTORAGE = 0x0161,

        CB_MSGMAX_OLD = 0x0162,

        CB_MULTIPLEADDSTRING = 0x0163,

        /// <summary>
        /// 获取有关指定组合框的信息。
        /// </summary>
        CB_GETCOMBOBOXINFO = 0x0164,

        CB_MSGMAX = 0x0165,

        // 0x0166 - 0x017F 消息未定义

        /// <summary>
        /// 将字符串添加到列表框。 如果列表框没有 LBS_SORT 样式，则字符串将添加到列表末尾。 否则，字符串将插入到列表中，并对列表进行排序。
        /// </summary>
        LB_ADDSTRING = 0x0180,

        /// <summary>
        /// 将字符串或项数据插入列表框中。 与 LB_ADDSTRING 消息不同， LB_INSERTSTRING 消息不会导致对具有 LBS_SORT 样式的列表进行排序。
        /// </summary>
        LB_INSERTSTRING = 0x0181,

        /// <summary>
        /// 删除列表框中的字符串。
        /// </summary>
        LB_DELETESTRING = 0x0182,

        /// <summary>
        /// 在多选列表框中选择一个或多个连续项。
        /// </summary>
        LB_SELITEMRANGEEX = 0x0183,

        /// <summary>
        /// 从列表框中删除所有项。
        /// </summary>
        LB_RESETCONTENT = 0x0184,

        /// <summary>
        /// 在多选列表框中选择一个项目，如有必要，将项目滚动到视图中。
        /// </summary>
        LB_SETSEL = 0x0185,

        /// <summary>
        /// 根据需要选择字符串并将其滚动到视图中。 选中新字符串后，列表框将从之前选择的字符串中删除突出显示。
        /// </summary>
        LB_SETCURSEL = 0x0186,

        /// <summary>
        /// 获取项的选择状态。
        /// </summary>
        LB_GETSEL = 0x0187,

        /// <summary>
        /// 获取单选列表框中当前选定项（如果有）的索引。
        /// </summary>
        LB_GETCURSEL = 0x0188,

        /// <summary>
        /// 从列表框中获取字符串。
        /// </summary>
        LB_GETTEXT = 0x0189,

        /// <summary>
        /// 获取列表框中字符串的长度。
        /// </summary>
        LB_GETTEXTLEN = 0x018A,

        /// <summary>
        /// 获取列表框中的项数。
        /// </summary>
        LB_GETCOUNT = 0x018B,

        /// <summary>
        /// 在列表框中搜索以指定字符串中的字符开头的项。 如果找到匹配项，则选择该项。
        /// </summary>
        LB_SELECTSTRING = 0x018C,

        /// <summary>
        /// 向列表框显示的列表添加名称。 该消息将添加与指定字符串和文件属性集匹配的目录和文件的名称。 LB_DIR 还可以将映射的驱动器号添加到列表框。
        /// </summary>
        LB_DIR = 0x018D,

        /// <summary>
        /// 获取列表框中第一个可见项的索引。 最初，索引为 0 的项位于列表框的顶部，但如果列表框内容已滚动，则另一个项目可能位于顶部。 多列列表框中的第一个可见项是左上角的项。
        /// </summary>
        LB_GETTOPINDEX = 0x018E,

        /// <summary>
        /// 查找以指定字符串开头的列表框中的第一个字符串。
        /// </summary>
        LB_FINDSTRING = 0x018F,

        /// <summary>
        /// 获取多选列表框中所选项的总数。
        /// </summary>
        LB_GETSELCOUNT = 0x0190,

        /// <summary>
        /// 用整数数组填充缓冲区，该数组指定多选列表框中选定项的项数。
        /// </summary>
        LB_GETSELITEMS = 0x0191,

        /// <summary>
        /// 设置列表框中的制表位位置。
        /// </summary>
        LB_SETTABSTOPS = 0x0192,

        /// <summary>
        /// 获取列表框的水平滚动宽度（以像素为单位），如果列表框具有水平滚动条，则列表框可以水平滚动 (可滚动宽度) 。
        /// </summary>
        LB_GETHORIZONTALEXTENT = 0x0193,

        /// <summary>
        /// 设置列表框可以水平滚动的宽度（以像素为单位）， (可滚动宽度) 。 如果列表框的宽度小于此值，水平滚动条水平滚动列表框中的项目。 如果列表框的宽度等于或大于此值，则水平滚动条处于隐藏状态。
        /// </summary>
        LB_SETHORIZONTALEXTENT = 0x0194,

        /// <summary>
        /// 设置多列列表框中所有列的宽度（以像素为单位）。
        /// </summary>
        LB_SETCOLUMNWIDTH = 0x0195,

        /// <summary>
        /// 将指定的文件名添加到包含目录列表的列表框中。
        /// </summary>
        LB_ADDFILE = 0x0196,

        /// <summary>
        /// 确保列表框中的指定项可见。
        /// </summary>
        LB_SETTOPINDEX = 0x0197,

        /// <summary>
        /// 获取当前显示在列表框中的列表框项的矩形的尺寸。
        /// </summary>
        LB_GETITEMRECT = 0x0198,

        /// <summary>
        /// 获取与指定列表框项关联的应用程序定义值。
        /// </summary>
        LB_GETITEMDATA = 0x0199,

        /// <summary>
        /// 设置与列表框中指定项关联的值。
        /// </summary>
        LB_SETITEMDATA = 0x019A,

        /// <summary>
        /// 在多选列表框中选择或取消选择一个或多个连续项。
        /// </summary>
        LB_SELITEMRANGE = 0x019B,

        /// <summary>
        /// 设置定位项，即从中启动多个选择的项。 多个选择跨越定位点项到插入点项的所有项。
        /// </summary>
        LB_SETANCHORINDEX = 0x019C,

        /// <summary>
        /// 获取定位点项的索引，即从中启动多个选择的项。 多个选择跨越定位点项到插入点项的所有项。
        /// </summary>
        LB_GETANCHORINDEX = 0x019D,

        /// <summary>
        /// 将焦点矩形设置为多选列表框中指定索引处的项。 如果该项不可见，则会滚动到视图中。
        /// </summary>
        LB_SETCARETINDEX = 0x019E,

        /// <summary>
        /// 检索具有多选列表框中焦点的项的索引。 该项目可能或可能未选中。
        /// </summary>
        LB_GETCARETINDEX = 0x019F,

        /// <summary>
        /// 设置列表框中项的高度（以像素为单位）。 如果列表框具有 LBS_OWNERDRAWVARIABLE 样式，此消息将设置 wParam 参数指定的项的高度。 否则，此消息将设置列表框中所有项的高度。
        /// </summary>
        LB_SETITEMHEIGHT = 0x01A0,

        /// <summary>
        /// 获取列表框中项的高度。
        /// </summary>
        LB_GETITEMHEIGHT = 0x01A1,

        /// <summary>
        /// 查找与指定字符串完全匹配的第一个列表框字符串，但搜索不区分大小写。
        /// </summary>
        LB_FINDSTRINGEXACT = 0x01A2,

        LBCB_CARETON = 0x01A3,

        LBCB_CARETOFF = 0x01A4,

        /// <summary>
        /// 设置列表框的当前区域设置。 可以使用区域设置来确定列表框显示的文本 (的正确排序顺序，其中包含 LBS_SORT 样式) 以及 由 LB_ADDSTRING 邮件添加的文本。
        /// </summary>
        LB_SETLOCALE = 0x01A5,

        /// <summary>
        /// 获取列表框的当前区域设置。 可以使用区域设置来确定列表框显示的文本 (的正确排序顺序，列表框具有 LBS_SORT 样式) 和 LB_ADDSTRING 邮件添加的文本。
        /// </summary>
        LB_GETLOCALE = 0x01A6,

        /// <summary>
        /// 设置使用 LBS_NODATA 样式创建的列表框中的项目计数，而不用 LBS_HASSTRINGS 样式创建。
        /// </summary>
        LB_SETCOUNT = 0x01A7,

        /// <summary>
        /// 分配用于存储列表框项的内存。 在应用程序向列表框添加大量项目之前，将使用此消息。
        /// </summary>
        LB_INITSTORAGE = 0x01A8,

        /// <summary>
        /// 获取列表框中最接近指定点的项的从零开始的索引。
        /// </summary>
        LB_ITEMFROMPOINT = 0x01A9,

        LB_INSERTSTRINGUPPER = 0x01AA,

        LB_INSERTSTRINGLOWER = 0x01AB,

        LB_ADDSTRINGUPPER = 0x01AC,

        LB_ADDSTRINGLOWER = 0x01AD,

        LBCB_STARTTRACK = 0x01AE,

        LBCB_ENDTRACK = 0x01AF,

        LB_MSGMAX_OLD = 0x01B0,

        LB_MULTIPLEADDSTRING = 0x01B1,

        /// <summary>
        /// 获取指定列表框中每列的项数。
        /// </summary>
        LB_GETLISTBOXINFO = 0x01B2,

        LB_MSGMAX = 0x01B3,

        // 0x01B4 - 0x01DF 消息未定义

        MN_FIRST = 0x01E0,

        /// <summary>
        /// 检索当前窗口的菜单句柄。
        /// </summary>
        MN_GETHMENU = 0x01E1,

        // 0x01E2 - 0x01FF 消息未定义

        /// <summary>
        /// 使用 WM_MOUSEFIRST 指定第一条鼠标消息。使用 PeekMessage() 函数。
        /// </summary>
        WM_MOUSEFIRST = 0x0200,

        /// <summary>
        /// 当光标移动时，WM_MOUSEMOVE 消息被发送到窗口。如果没有捕获鼠标，则将消息发送到包含光标的窗口。否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MOUSEMOVE = WM_MOUSEFIRST,

        /// <summary>
        /// WM_LBUTTONDOWN 消息是当用户按下鼠标左键，而光标位于窗口的客户端区域时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_LBUTTONDOWN = 0x0201,

        /// <summary>
        /// WM_LBUTTONUP 消息是在用户释放鼠标左键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_LBUTTONUP = 0x0202,

        /// <summary>
        /// WM_LBUTTONDBLCLK 消息是在用户双击鼠标左键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_LBUTTONDBLCLK = 0x0203,

        /// <summary>
        /// WM_RBUTTONDOWN 消息是当用户在窗口的客户端区域按下鼠标右键时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_RBUTTONDOWN = 0x0204,

        /// <summary>
        /// WM_RBUTTONUP 消息是在用户释放鼠标右键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_RBUTTONUP = 0x0205,

        /// <summary>
        /// 当用户在窗口的客户端区域内双击鼠标右键时，WM_RBUTTONDBLCLK 消息将被发布。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_RBUTTONDBLCLK = 0x0206,

        /// <summary>
        /// WM_MBUTTONDOWN消息是当用户在窗口的客户端区域按下鼠标中键时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MBUTTONDOWN = 0x0207,

        /// <summary>
        /// WM_MBUTTONUP 消息是在用户释放鼠标中键时发出的，而光标位于窗口的客户端区域。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MBUTTONUP = 0x0208,

        /// <summary>
        /// WM_MBUTTONDBLCLK 消息是当用户双击鼠标中间按钮，而光标位于窗口的客户端区域时发出的。如果没有捕获鼠标，则消息将被发送到光标下方的窗口。
        /// 否则，消息将被发送到捕获了鼠标的窗口。
        /// </summary>
        WM_MBUTTONDBLCLK = 0x0209,

        /// <summary>
        /// 当鼠标滚轮旋转时，发送到焦点窗口。 DefWindowProc 函数将消息传播到窗口的父级。 不应有消息的内部转发，因为 DefWindowProc 会将其传播到父链上，直到找到处理它的窗口。
        /// </summary>
        WM_MOUSEWHEEL = 0x020A,

        /// <summary>
        /// 当用户按下第一个或第二个 X 按钮时，光标位于窗口的工作区时发布。 如果未捕获鼠标，则会将消息发布到光标下方的窗口。 否则，消息将发布到捕获了鼠标的窗口。
        /// </summary>
        WM_XBUTTONDOWN = 0x020B,

        /// <summary>
        /// 当用户释放第一个或第二个 X 按钮时，光标位于窗口的工作区时发布。 如果未捕获鼠标，消息将发布到光标下方的窗口中。 否则，消息将发布到捕获了鼠标的窗口。
        /// </summary>
        WM_XBUTTONUP = 0x020C,

        /// <summary>
        /// 当用户在光标位于窗口的工作区时双击第一个或第二个 X 按钮时发布。 如果未捕获鼠标，消息将发布到光标下方的窗口中。 否则，消息将发布到捕获了鼠标的窗口。
        /// </summary>
        WM_XBUTTONDBLCLK = 0x020D,

        /// <summary>
        /// 当鼠标的水平滚轮倾斜或旋转时，发送到活动窗口。 DefWindowProc 函数将消息传播到窗口的父级。 不应有消息的内部转发，因为 DefWindowProc 会将其传播到父链上，直到找到处理它的窗口。
        /// </summary>
        WM_MOUSEHWHEEL = 0x020E,

        /// <summary>
        /// 使用 WM_MOUSELAST 指定最后一条鼠标消息。与 PeekMessage() 函数一起使用。
        /// </summary>
        WM_MOUSELAST = WM_MOUSEHWHEEL,

        // 0x020F 消息未定义

        /// <summary>
        /// 当子代窗口上发生重大操作时，发送到窗口。 此消息现已扩展为包含 WM_POINTERDOWN 事件。 创建子窗口时，系统会在创建该窗口的 CreateWindow 或 CreateWindowEx 函数返回之前发送 WM_PARENTNOTIFY。 当子窗口被销毁时，系统会在进行任何销毁窗口的处理之前发送消息。
        /// </summary>
        WM_PARENTNOTIFY = 0x0210,

        /// <summary>
        /// 通知应用程序的主窗口过程已输入菜单模式循环。
        /// </summary>
        WM_ENTERMENULOOP = 0x0211,

        /// <summary>
        /// 通知应用程序的主窗口过程已退出菜单模式循环。
        /// </summary>
        WM_EXITMENULOOP = 0x0212,

        /// <summary>
        /// 当使用向右键或向左键在菜单栏和系统菜单之间切换时发送到应用程序。
        /// </summary>
        WM_NEXTMENU = 0x0213,

        /// <summary>
        /// 发送到用户正在调整大小的窗口。 通过处理此消息，应用程序可以监视拖动矩形的大小和位置，并根据需要更改其大小或位置。
        /// </summary>
        WM_SIZING = 0x0214,

        /// <summary>
        /// 发送到丢失鼠标捕获的窗口。
        /// </summary>
        WM_CAPTURECHANGED = 0x0215,

        /// <summary>
        /// 发送到用户正在移动的窗口。 通过处理此消息，应用程序可以监视拖动矩形的位置，并根据需要更改其位置。
        /// </summary>
        WM_MOVING = 0x0216,

        // 0x0217 消息未定义

        /// <summary>
        /// 通知应用程序电源管理事件已发生。
        /// </summary>
        WM_POWERBROADCAST = 0x0218,

        /// <summary>
        /// 通知应用程序更改设备或计算机的硬件配置。
        /// </summary>
        WM_DEVICECHANGE = 0x0219,

        // 0x021A - 0x021F 消息未定义

        /// <summary>
        /// 应用程序将 WM_MDICREATE 消息发送到多文档接口， (MDI) 客户端窗口创建 MDI 子窗口。
        /// </summary>
        WM_MDICREATE = 0x0220,

        /// <summary>
        /// 应用程序将 WM_MDIDESTROY 消息发送到多文档接口， (MDI) 客户端窗口关闭 MDI 子窗口。
        /// </summary>
        WM_MDIDESTROY = 0x0221,

        /// <summary>
        /// 应用程序将 WM_MDIGETACTIVE 消息发送到多文档接口 (MDI) 客户端窗口，以检索活动 MDI 子窗口的句柄。
        /// </summary>
        WM_MDIACTIVATE = 0x0222,

        /// <summary>
        /// 应用程序将 WM_MDIRESTORE 消息发送到多文档接口， (MDI) 客户端窗口，以便从最大化或最小化的大小还原 MDI 子窗口。
        /// </summary>
        WM_MDIRESTORE = 0x0223,

        /// <summary>
        /// 应用程序将 WM_MDINEXT 消息发送到多文档接口 (MDI) 客户端窗口以激活下一个或上一个子窗口。
        /// </summary>
        WM_MDINEXT = 0x0224,

        /// <summary>
        /// 应用程序将 WM_MDIMAXIMIZE 消息发送到多文档接口， (MDI) 客户端窗口最大化 MDI 子窗口。 系统调整子窗口的大小，使其工作区填充客户端窗口。 系统将子窗口的窗口菜单图标置于框架窗口菜单栏的最右侧位置，并将子窗口的还原图标置于最左侧的位置。 系统还会将子窗口的标题栏文本追加到框架窗口的标题栏文本。
        /// </summary>
        WM_MDIMAXIMIZE = 0x0225,

        /// <summary>
        /// 应用程序将 WM_MDITILE 消息发送到多文档界面， (MDI) 客户端窗口，以磁贴格式排列其所有 MDI 子窗口。
        /// </summary>
        WM_MDITILE = 0x0226,

        /// <summary>
        /// 应用程序将 WM_MDICASCADE 消息发送到多文档接口 (MDI) 客户端窗口，以级联格式排列其所有子窗口。
        /// </summary>
        WM_MDICASCADE = 0x0227,

        /// <summary>
        /// 应用程序将 WM_MDIICONARRANGE 消息发送到多文档接口， (MDI) 客户端窗口排列所有最小化的 MDI 子窗口。 它不会影响未最小化的子窗口。
        /// </summary>
        WM_MDIICONARRANGE = 0x0228,

        /// <summary>
        /// 应用程序将 WM_MDIGETACTIVE 消息发送到多文档接口 (MDI) 客户端窗口，以检索活动 MDI 子窗口的句柄。
        /// </summary>
        WM_MDIGETACTIVE = 0x0229,

        WM_DROPOBJECT = 0x022A,

        WM_QUERYDROPOBJECT = 0x022B,

        WM_BEGINDRAG = 0x022C,

        WM_DRAGLOOP = 0x022D,

        WM_DRAGSELECT = 0x022E,

        WM_DRAGMOVE = 0x022F,

        /// <summary>
        /// 应用程序将 WM_MDISETMENU 消息发送到多文档界面， (MDI) 客户端窗口替换 MDI 框架窗口的整个菜单，替换框架窗口的窗口菜单，或两者。
        /// </summary>
        WM_MDISETMENU = 0x0230,

        /// <summary>
        /// 在窗口进入移动模式循环或调整大小循环后，将一次发送到窗口。 当用户单击窗口的标题栏或大小调整边框或窗口将 WM_SYSCOMMAND 消息传递到 DefWindowProc 函数和消息 的 wParam 参数指定 SC_MOVE 或 SC_SIZE 值时，窗口将输入移动或调整模式循环。
        /// 当 DefWindowProc 返回时，该操作已完成。
        /// </summary>
        WM_ENTERSIZEMOVE = 0x0231,

        /// <summary>
        /// 在窗口退出移动或调整模式循环后，将一次发送到窗口。 当用户单击窗口的标题栏或大小调整边框或窗口将 WM_SYSCOMMAND 消息传递到 DefWindowProc 函数时，窗口将输入移动或调整模式循环，而消息的 wParam 参数指定 SC_MOV E 或 SC_SIZE 值。 当 DefWindowProc 返回时，此操作将完成。
        /// </summary>
        WM_EXITSIZEMOVE = 0x0232,

        /// <summary>
        /// 当用户在已将自己注册为已删除文件的收件人的应用程序窗口上删除文件时发送。
        /// </summary>
        WM_DROPFILES = 0x0233,

        /// <summary>
        /// 应用程序将 WM_MDIREFRESHMENU 消息发送到多文档接口， (MDI) 客户端窗口刷新 MDI 框架窗口的窗口菜单。
        /// </summary>
        WM_MDIREFRESHMENU = 0x0234,

        // 0x0235 - 0x0237 消息未定义

        /// <summary>
        /// 当附加了数字化仪的监视器的设置发生更改时，发送到窗口。 此消息包含有关显示模式缩放的信息。
        /// </summary>
        WM_POINTERDEVICECHANGE = 0x0238,

        /// <summary>
        /// 在输入数字化器范围内检测到指针设备时发送到窗口。 此消息包含有关设备及其邻近度的信息。
        /// </summary>
        WM_POINTERDEVICEINRANGE = 0x0239,

        /// <summary>
        /// 当指针设备离开输入数字化器的范围时，发送到窗口。 此消息包含有关设备及其邻近度的信息。
        /// </summary>
        WM_POINTERDEVICEOUTOFRANGE = 0x023A,

        WM_STOPINERTIA = 0x023B,

        WM_ENDINERTIA = 0x023C,

        WM_EDGYINERTIA = 0x023D,

        // 0x023E - 0x023F 消息未定义

        /// <summary>
        /// 当一个或多个触摸点（如手指或笔）触摸触摸敏感数字化器表面时，通知窗口。
        /// </summary>
        WM_TOUCH = 0x0240,

        /// <summary>
        /// 发布以在指针上提供一个更新，该指针在窗口的非工作区上或在悬停未捕获的联系人移动到窗口的非工作区时进行接触。 指针悬停时，消息以指针恰好悬停在哪个窗口为目标。 当指针与表面接触时，指针会隐式捕获到指针接触的窗口，并且该窗口继续接收指针的输入，直到它断开接触。
        /// 如果窗口捕获了此指针，则不会发布此消息。 相反， WM_POINTERUPDATE 将发布到捕获此指针的窗口。
        /// </summary>
        WM_NCPOINTERUPDATE = 0x0241,

        /// <summary>
        /// 当指针在窗口的非工作区上进行接触时发布。 消息以指针联系的窗口为目标。 指针隐式捕获到窗口，以便窗口继续接收指针的输入，直到它中断接触。
        /// 如果窗口捕获了此指针，则不会发布此消息。 相反， WM_POINTERDOWN 会发布到捕获此指针的窗口。
        /// </summary>
        WM_NCPOINTERDOWN = 0x0242,

        /// <summary>
        /// 在窗口的非工作区上建立接触的指针断开联系人时发布。 该消息以指针在其中进行接触的窗口为目标，此时指针被隐式捕获到窗口，以便窗口继续接收指针的输入，直到它中断接触，包括 WM_NCPOINTERUP 通知。
        /// 如果窗口捕获了此指针，则不会发布此消息。 相反， WM_NCPOINTERUP 将发布到已捕获此指针的窗口。
        /// </summary>
        WM_NCPOINTERUP = 0x0243,

        WM_NCPOINTERLAST = 0x0244,

        /// <summary>
        /// 发布以提供在窗口工作区上方的指针或悬停在窗口工作区上的未捕获指针上的接触的更新。 指针悬停时，消息以指针恰好悬停在哪个窗口为目标。 当指针与表面接触时，指针会隐式捕获到指针接触的窗口，并且该窗口继续接收指针的输入，直到它断开接触。
        /// </summary>
        WM_POINTERUPDATE = 0x0245,

        /// <summary>
        /// 当指针在窗口的工作区上进行接触时发布。 此输入消息以指针接触的窗口为目标，指针隐式捕获到窗口，以便窗口继续接收指针的输入，直到它中断接触。
        /// </summary>
        WM_POINTERDOWN = 0x0246,

        /// <summary>
        /// 在窗口工作区上建立接触的指针中断联系人时发布。 此输入消息以指针进行接触的窗口为目标，指针在该时间点隐式捕获到窗口，以便窗口继续接收输入消息，包括指针的 WM_POINTERUP 通知，直到它中断联系人。
        /// </summary>
        WM_POINTERUP = 0x0247,

        WM_POINTER_reserved_248 = 0x0248,

        /// <summary>
        /// 当新指针进入窗口上方的检测范围 (悬停) 或现有指针在窗口边界内移动时，发送到窗口。
        /// </summary>
        WM_POINTERENTER = 0x0249,

        /// <summary>
        /// 当指针离开窗口上的检测范围时， (将鼠标悬停) 或指针移动到窗口边界之外时，发送到窗口。
        /// </summary>
        WM_POINTERLEAVE = 0x024A,

        /// <summary>
        /// 当主指针在窗口上生成 WM_POINTERDOWN 时发送到非活动窗口。 只要邮件保持未处理状态，它才会到达父窗口链，直到到达顶级窗口。 应用程序可以响应此消息，以指定是否要激活它们。
        /// </summary>
        WM_POINTERACTIVATE = 0x024B,

        /// <summary>
        /// 发送到丢失输入指针捕获的窗口。
        /// </summary>
        WM_POINTERCAPTURECHANGED = 0x024C,

        /// <summary>
        /// 发送到向下触摸的窗口，以确定最可能的触摸目标。
        /// </summary>
        WM_TOUCHHITTESTING = 0x024D,

        /// <summary>
        /// 旋转滚轮时，使用前台键盘焦点发布到窗口。
        /// </summary>
        WM_POINTERWHEEL = 0x024E,

        /// <summary>
        /// 在水平滚轮旋转时，使用前台键盘焦点发布到窗口。
        /// </summary>
        WM_POINTERHWHEEL = 0x024F,

        WM_POINTER_reserved_250 = 0x0250,

        WM_POINTER_reserved_251 = 0x0251,

        WM_POINTER_reserved_252 = 0x0252,

        WM_POINTER_reserved_253 = 0x0253,

        WM_POINTER_reserved_254 = 0x0254,

        WM_POINTER_reserved_255 = 0x0255,

        WM_POINTER_reserved_256 = 0x0256,

        WM_POINTERLAST = 0x0257,

        // 0x0258 - 0x026F 消息未定义

        WM_VISIBILITYCHANGED = 0x0270,

        WM_VIEWSTATECHANGED = 0x0271,

        WM_UNREGISTER_WINDOW_SERVICES = 0x0272,

        WM_CONSOLIDATED = 0x0273,

        // 0x0274 - 0x027F 消息未定义

        WM_IME_REPORT = 0x0280,

        /// <summary>
        /// 激活窗口时发送到应用程序。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_SETCONTEXT = 0x0281,

        /// <summary>
        /// 发送到应用程序以通知应用程序对 IME 窗口所做的更改。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_NOTIFY = 0x0282,

        /// <summary>
        /// 应用程序发送以指示 IME 窗口执行请求的命令。 应用程序使用此消息来控制已创建的 IME 窗口。 若要发送此消息，应用程序使用以下参数调用 PostMessage 函数。
        /// </summary>
        WM_IME_CONTROL = 0x0283,

        /// <summary>
        /// 当 IME 窗口没有空间扩展合成窗口的区域时发送到应用程序。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_COMPOSITIONFULL = 0x0284,

        /// <summary>
        /// 当操作系统即将更改当前 IME 时发送到应用程序。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_SELECT = 0x0285,

        /// <summary>
        /// 当 IME 获取转换结果的字符时发送到应用程序。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_CHAR = 0x0286,

        WM_IME_SYSTEM = 0x0287,

        /// <summary>
        /// 发送到应用程序以提供命令和请求信息。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_REQUEST = 0x0288,

        WM_KANJI_reserved_289 = 0x0289,

        WM_KANJI_reserved_28a = 0x028A,

        WM_KANJI_reserved_28b = 0x028B,

        WM_KANJI_reserved_28c = 0x028C,

        WM_KANJI_reserved_28d = 0x028D,

        WM_KANJI_reserved_28e = 0x028E,

        WM_KANJI_reserved_28f = 0x028F,

        /// <summary>
        /// IME 发送到应用程序以通知应用程序按下键并保留消息顺序。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_KEYDOWN = 0x0290,

        /// <summary>
        /// 通过 IME 发送到应用程序以通知应用程序密钥发布并保留消息顺序。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_IME_KEYUP = 0x0291,

        WM_KANJI_reserved_292 = 0x0292,

        WM_KANJI_reserved_293 = 0x0293,

        WM_KANJI_reserved_294 = 0x0294,

        WM_KANJI_reserved_295 = 0x0295,

        WM_KANJI_reserved_296 = 0x0296,

        WM_KANJI_reserved_297 = 0x0297,

        WM_KANJI_reserved_298 = 0x0298,

        WM_KANJI_reserved_299 = 0x0299,

        WM_KANJI_reserved_29a = 0x029A,

        WM_KANJI_reserved_29b = 0x029B,

        WM_KANJI_reserved_29c = 0x029C,

        WM_KANJI_reserved_29d = 0x029D,

        WM_KANJI_reserved_29e = 0x029E,

        WM_KANJILAST = 0x029F,

        /// <summary>
        /// 在之前调用 TrackMouseEvent 中指定的时间段内，光标悬停在窗口的非工作区上时，发布到窗口。
        /// </summary>
        WM_NCMOUSEHOVER = 0x02A0,

        /// <summary>
        /// 当光标悬停在窗口的工作区上之前调用 TrackMouseEvent 中指定的时间段时，将发布到窗口。
        /// </summary>
        WM_MOUSEHOVER = 0x02A1,

        /// <summary>
        /// 当光标离开之前对 TrackMouseEvent 的调用中指定的窗口的非client 区域时，发布到窗口。
        /// </summary>
        WM_NCMOUSELEAVE = 0x02A2,

        /// <summary>
        /// 当光标离开之前对 TrackMouseEvent 调用中指定的窗口的工作区时，发布到窗口。
        /// </summary>
        WM_MOUSELEAVE = 0x02A3,

        WM_TRACKMOUSEEVENT__reserved_2a4 = 0x02A4,

        WM_TRACKMOUSEEVENT__reserved_2a5 = 0x02A5,

        WM_TRACKMOUSEEVENT__reserved_2a6 = 0x02A6,

        WM_TRACKMOUSEEVENT__reserved_2a7 = 0x02A7,

        WM_TRACKMOUSEEVENT__reserved_2a8 = 0x02A8,

        WM_TRACKMOUSEEVENT__reserved_2a9 = 0x02A9,

        WM_TRACKMOUSEEVENT__reserved_2aa = 0x02AA,

        WM_TRACKMOUSEEVENT__reserved_2ab = 0x02AB,

        WM_TRACKMOUSEEVENT__reserved_2ac = 0x02AC,

        WM_TRACKMOUSEEVENT__reserved_2ad = 0x02AD,

        WM_TRACKMOUSEEVENT__reserved_2ae = 0x02AE,

        WM_TRACKMOUSEEVENT_LAST = 0x02AF,

        // 0x02B0 消息未定义

        /// <summary>
        /// 通知应用程序会话状态的更改。
        /// </summary>
        WM_WTSSESSION_CHANGE = 0x02B1,

        // 0x02B2 - 0x02BF 消息未定义

        WM_TABLET_FIRST = 0x02C0,

        WM_TABLET__reserved_2c1 = 0x02C1,

        WM_TABLET__reserved_2c2 = 0x02C2,

        WM_TABLET__reserved_2c3 = 0x02C3,

        WM_TABLET__reserved_2c4 = 0x02C4,

        WM_TABLET__reserved_2c5 = 0x02C5,

        WM_TABLET__reserved_2c6 = 0x02C6,

        WM_TABLET__reserved_2c7 = 0x02C7,

        WM_POINTERDEVICEADDED = 0x02C8,

        WM_POINTERDEVICEDELETED = 0x02C9,

        WM_TABLET__reserved_2ca = 0x02CA,

        WM_FLICK = 0x02CB,

        WM_TABLET__reserved_2cc = 0x02CC,

        WM_FLICKINTERNAL = 0x02CD,

        WM_BRIGHTNESSCHANGED = 0x02CE,

        WM_TABLET__reserved_2cf = 0x02CF,

        WM_TABLET__reserved_2d0 = 0x02D0,

        WM_TABLET__reserved_2d1 = 0x02D1,

        WM_TABLET__reserved_2d2 = 0x02D2,

        WM_TABLET__reserved_2d3 = 0x02D3,

        WM_TABLET__reserved_2d4 = 0x02D4,

        WM_TABLET__reserved_2d5 = 0x02D5,

        WM_TABLET__reserved_2d6 = 0x02D6,

        WM_TABLET__reserved_2d7 = 0x02D7,

        WM_TABLET__reserved_2d8 = 0x02D8,

        WM_TABLET__reserved_2d9 = 0x02D9,

        WM_TABLET__reserved_2da = 0x02DA,

        WM_TABLET__reserved_2db = 0x02DB,

        WM_TABLET__reserved_2dc = 0x02DC,

        WM_TABLET__reserved_2dd = 0x02DD,

        WM_TABLET__reserved_2de = 0x02DE,

        WM_TABLET_LAST = 0x02DF,

        /// <summary>
        /// 当窗口的有效点数 (dpi) 更改时发送。 DPI 是窗口的规模因子。 有多个事件可能导致 DPI 更改。 以下列表指示 DPI 更改的可能原因。
        /// 窗口将移动到具有不同 DPI 的新监视器。
        /// 承载窗口的监视器的 DPI 会更改。
        /// 窗口的当前 DPI 始终等于 WM_DPICHANGED 发送的最后 一个 DPI。 对于知道 DPI 更改的线程，窗口应缩放到的缩放因子。
        /// </summary>
        WM_DPICHANGED = 0x02E0,

        // 0x02E1 消息未定义

        /// <summary>
        /// 对于 Per Monitor v2 顶级窗口，此消息将发送到正在发生 DPI 更改的窗口的子 HWDN 树中的所有 HWND。 此消息发生在顶层窗口接收 WM_DPICHANGED 之前，并从上到下遍历子树。
        /// </summary>
        WM_DPICHANGED_BEFOREPARENT = 0x02E2,

        /// <summary>
        /// 对于 Per Monitor v2 顶级窗口，此消息将发送到正在更改 DPI 的窗口的子 HWDN 树中的所有 HWND。 在顶层窗口收到 WM_DPICHANGED 并遍历从上到下子树之后，会出现此消息。
        /// </summary>
        WM_DPICHANGED_AFTERPARENT = 0x02E3,

        /// <summary>
        /// 此消息告知操作系统，窗口的大小将调整为默认值以外的维度。
        /// 在发送 WM_DPICHANGED 消息之前，此消息将发送到具有每个监视器 v2 DPI_AWARENESS_CONTEXT的顶级窗口，并允许窗口计算其挂起 DPI 更改所需的大小。 由于线性 DPI 缩放是默认行为，因此仅在窗口想要以非线性方式缩放的情况下才有用。 如果应用程序响应此消息，生成的大小将是发送到 WM_DPICHANGED 的候选矩形。
        /// 使用此消息可更改 随 WM_DPICHANGED 一起提供的 rect 大小。
        /// </summary>
        WM_GETDPISCALEDSIZE = 0x02E4,

        // 0x02E5 - 0x02FF 消息未定义

        /// <summary>
        /// 应用程序将 WM_CUT 消息发送到编辑控件或组合框，以删除 (剪切) 当前所选内容（如果有），并在编辑控件中以 CF_TEXT 格式将已删除的文本复制到剪贴板。
        /// </summary>
        WM_CUT = 0x0300,

        /// <summary>
        /// 应用程序将 WM_COPY 消息发送到编辑控件或组合框，以 CF_TEXT 格式将当前选定内容复制到剪贴板。
        /// </summary>
        WM_COPY = 0x0301,

        /// <summary>
        /// 应用程序将 WM_PASTE 消息发送到编辑控件或组合框，以将剪贴板的当前内容复制到位于当前插入点位置的编辑控件。 仅当剪贴板包含 CF_TEXT 格式的数据时，才会插入数据。
        /// </summary>
        WM_PASTE = 0x0302,

        /// <summary>
        /// 应用程序将 WM_CLEAR 消息发送到编辑控件或组合框，以从编辑控件中删除 (清除) 当前所选内容（如果有）。
        /// </summary>
        WM_CLEAR = 0x0303,

        /// <summary>
        /// 应用程序将 WM_UNDO 消息发送到编辑控件以撤消最后一个操作。 当此消息发送到编辑控件时，将还原以前删除的文本或删除以前添加的文本。
        /// </summary>
        WM_UNDO = 0x0304,

        /// <summary>
        /// 如果剪贴板所有者延迟呈现特定剪贴板格式，并且应用程序已请求采用该格式的数据，则将其发送到剪贴板所有者。 剪贴板所有者必须以指定的格式呈现数据，并通过调用 SetClipboardData 函数将其置于剪贴板上。
        /// </summary>
        WM_RENDERFORMAT = 0x0305,

        /// <summary>
        /// 如果剪贴板所有者延迟呈现一个或多个剪贴板格式，则将其发送到剪贴板所有者，然后再将其销毁。 若要使剪贴板的内容可供其他应用程序使用，剪贴板所有者必须以能够生成的所有格式呈现数据，并通过调用 SetClipboardData 函数将数据放置在剪贴板上。
        /// </summary>
        WM_RENDERALLFORMATS = 0x0306,

        /// <summary>
        /// 当对 EmptyClipboard 函数的调用耗尽剪贴板时，发送到剪贴板所有者。
        /// </summary>
        WM_DESTROYCLIPBOARD = 0x0307,

        /// <summary>
        /// 当剪贴板的内容发生更改时，发送到剪贴板查看器链中的第一个窗口。 这使剪贴板查看器窗口能够显示剪贴板的新内容。
        /// </summary>
        WM_DRAWCLIPBOARD = 0x0308,

        /// <summary>
        /// 当剪贴板包含 CF_OWNERDISPLAY 格式的数据并且剪贴板查看器的工作区需要重新绘制时，由剪贴板查看器的剪贴板所有者窗口发送到剪贴板所有者。
        /// </summary>
        WM_PAINTCLIPBOARD = 0x0309,

        /// <summary>
        /// 当剪贴板包含 CF_OWNERDISPLAY 格式的数据并且剪贴板查看器的垂直滚动条中发生事件时，由剪贴板查看器的剪贴板查看器窗口发送到剪贴板所有者。 所有者应滚动剪贴板图像并更新滚动条值。
        /// </summary>
        WM_VSCROLLCLIPBOARD = 0x030A,

        /// <summary>
        /// 当剪贴板包含 采用CF_OWNERDISPLAY 格式的数据并且剪贴板查看器的工作区已更改大小时，剪贴板查看器窗口将发送给剪贴板所有者。
        /// </summary>
        WM_SIZECLIPBOARD = 0x030B,

        /// <summary>
        /// 通过剪贴板查看器窗口发送到剪贴板所有者，以请求 CF_OWNERDISPLAY 剪贴板格式的名称。
        /// </summary>
        WM_ASKCBFORMATNAME = 0x030C,

        /// <summary>
        /// 从链中删除窗口时，发送到剪贴板查看器链中的第一个窗口。
        /// </summary>
        WM_CHANGECBCHAIN = 0x030D,

        /// <summary>
        /// 通过剪贴板查看器窗口发送到剪贴板所有者。 当剪贴板包含 CF_OWNERDISPLAY 格式的数据，并且剪贴板查看器的水平滚动条中发生事件时，会发生此情况。 所有者应滚动剪贴板图像并更新滚动条值。
        /// </summary>
        WM_HSCROLLCLIPBOARD = 0x030E,

        /// <summary>
        /// WM_QUERYNEWPALETTE 消息通知一个窗口，它即将接收键盘焦点，使窗口有机会在收到焦点时实现其逻辑调色板。
        /// </summary>
        WM_QUERYNEWPALETTE = 0x030F,

        /// <summary>
        /// WM_PALETTEISCHANGING 消息通知应用程序应用程序要实现其逻辑调色板。
        /// </summary>
        WM_PALETTEISCHANGING = 0x0310,

        /// <summary>
        /// 当窗口具有键盘焦点的窗口已实现其逻辑调色板后， WM_PALETTECHANGED 消息将发送到所有顶级和重叠窗口，从而更改系统调色板。 此消息启用使用调色板但没有键盘焦点的窗口来实现其逻辑调色板并更新其工作区。
        /// </summary>
        WM_PALETTECHANGED = 0x0311,

        /// <summary>
        /// 当用户按下 RegisterHotKey 函数注册的热键时发布。 消息放置在与注册热键的线程关联的消息队列的顶部。
        /// </summary>
        WM_HOTKEY = 0x0312,

        /// <summary>
        /// 任务栏窗口右键单击时弹出菜单的消息
        /// </summary>
        WM_SYSMENU = 0x0313,

        WM_HOOKMSG = 0x0314,

        WM_EXITPROCESS = 0x0315,

        WM_WAKETHREAD = 0x0316,

        /// <summary>
        /// WM_PRINT 消息将发送到窗口，以请求它在指定的设备上下文中绘制自身，这通常是打印机设备上下文中的。
        /// </summary>
        WM_PRINT = 0x0317,

        /// <summary>
        /// 将 WM_PRINTCLIENT 消息发送到窗口，以请求它在指定的设备上下文中绘制其工作区，通常位于打印机设备上下文中。
        /// 与 WM_PRINT 不同，DefWindowProc 不会处理 WM_PRINTCLIENT。 窗口应通过应用程序定义的 WndProc 函数处理 WM_PRINTCLIENT 消息，以便其正确使用。
        /// </summary>
        WM_PRINTCLIENT = 0x0318,

        /// <summary>
        /// 通知窗口用户生成应用程序命令事件，例如，使用鼠标单击应用程序命令按钮或在键盘上键入应用程序命令键。
        /// </summary>
        WM_APPCOMMAND = 0x0319,

        /// <summary>
        /// 在主题更改事件之后广播到每个窗口。 主题更改事件的示例包括主题的激活、主题停用或从一个主题过渡到另一个主题。
        /// </summary>
        WM_THEMECHANGED = 0x031A,

        WM_UAHINIT = 0x031B,

        WM_DESKTOPNOTIFY = 0x031C,

        /// <summary>
        /// 剪贴板的内容发生更改时发送。
        /// </summary>
        WM_CLIPBOARDUPDATE = 0x031D,

        /// <summary>
        /// 通知所有顶级窗口，桌面窗口管理器 (DWM) 组合已启用或禁用。
        /// </summary>
        WM_DWMCOMPOSITIONCHANGED = 0x031E,

        /// <summary>
        /// 当非工作区呈现策略发生更改时发送。
        /// </summary>
        WM_DWMNCRENDERINGCHANGED = 0x031F,

        /// <summary>
        /// 通知所有顶级窗口着色颜色已更改。
        /// </summary>
        WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,

        /// <summary>
        /// 当桌面窗口管理器 (DWM) 组合窗口最大化时发送。
        /// </summary>
        WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,

        WM_DWMEXILEFRAME = 0x0322,

        /// <summary>
        /// 指示窗口提供静态位图以用作该窗口的缩略图表示形式。
        /// </summary>
        WM_DWMSENDICONICTHUMBNAIL = 0x0323,

        WM_MAGNIFICATION_STARTED = 0x0324,

        WM_MAGNIFICATION_ENDED = 0x0325,

        /// <summary>
        /// 指示窗口提供静态位图以用作实时预览 (也称为该窗口的 速览预览) 。
        /// </summary>
        WM_DWMSENDICONICLIVEPREVIEWBITMAP = 0x0326,

        WM_DWMTHUMBNAILSIZECHANGED = 0x0327,

        WM_MAGNIFICATION_OUTPUT = 0x0328,

        WM_BSDRDATA = 0x0329,

        WM_DWMTRANSITIONSTATECHANGED = 0x032A,

        // 0x032B 消息未定义

        WM_KEYBOARDCORRECTIONCALLOUT = 0x032C,

        WM_KEYBOARDCORRECTIONACTION = 0x032D,

        WM_UIACTION = 0x032E,

        WM_ROUTED_UI_EVENT = 0x032F,

        WM_MEASURECONTROL = 0x0330,

        WM_GETACTIONTEXT = 0x0331,

        WM_CE_ONLY__reserved_332 = 0x0332,

        WM_FORWARDKEYDOWN = 0x0333,

        WM_FORWARDKEYUP = 0x0334,

        WM_CE_ONLY__reserved_335 = 0x0335,

        WM_CE_ONLY__reserved_336 = 0x0336,

        WM_CE_ONLY__reserved_337 = 0x0337,

        WM_CE_ONLY__reserved_338 = 0x0338,

        WM_CE_ONLY__reserved_339 = 0x0339,

        WM_CE_ONLY__reserved_33a = 0x033A,

        WM_CE_ONLY__reserved_33b = 0x033B,

        WM_CE_ONLY__reserved_33c = 0x033C,

        WM_CE_ONLY__reserved_33d = 0x033D,

        WM_CE_ONLY_LAST = 0x033E,

        /// <summary>
        /// 发送到请求扩展标题栏信息。 窗口通过其 WndProc 函数接收此消息。
        /// </summary>
        WM_GETTITLEBARINFOEX = 0x033F,

        WM_NOTIFYWOW = 0x0340,

        // 0x0341 - 0x0357 消息未定义

        WM_HANDHELDFIRST = 0x0358,

        WM_HANDHELD_reserved_359 = 0x0359,

        WM_HANDHELD_reserved_35a = 0x035A,

        WM_HANDHELD_reserved_35b = 0x035B,

        WM_HANDHELD_reserved_35c = 0x035C,

        WM_HANDHELD_reserved_35d = 0x035D,

        WM_HANDHELD_reserved_35e = 0x035E,

        WM_HANDHELDLAST = 0x035F,

        WM_AFXFIRST = 0x0360,

        WM_AFX_reserved_361 = 0x0361,

        WM_AFX_reserved_362 = 0x0362,

        WM_AFX_reserved_363 = 0x0363,

        WM_AFX_reserved_364 = 0x0364,

        WM_AFX_reserved_365 = 0x0365,

        WM_AFX_reserved_366 = 0x0366,

        WM_AFX_reserved_367 = 0x0367,

        WM_AFX_reserved_368 = 0x0368,

        WM_AFX_reserved_369 = 0x0369,

        WM_AFX_reserved_36a = 0x036A,

        WM_AFX_reserved_36b = 0x036B,

        WM_AFX_reserved_36c = 0x036C,

        WM_AFX_reserved_36d = 0x036D,

        WM_AFX_reserved_36e = 0x036E,

        WM_AFX_reserved_36f = 0x036F,

        WM_AFX_reserved_370 = 0x0370,

        WM_AFX_reserved_371 = 0x0371,

        WM_AFX_reserved_372 = 0x0372,

        WM_AFX_reserved_373 = 0x0373,

        WM_AFX_reserved_374 = 0x0374,

        WM_AFX_reserved_375 = 0x0375,

        WM_AFX_reserved_376 = 0x0376,

        WM_AFX_reserved_377 = 0x0377,

        WM_AFX_reserved_378 = 0x0378,

        WM_AFX_reserved_379 = 0x0379,

        WM_AFX_reserved_37a = 0x037A,

        WM_AFX_reserved_37b = 0x037B,

        WM_AFX_reserved_37c = 0x037C,

        WM_AFX_reserved_37d = 0x037D,

        WM_AFX_reserved_37e = 0x037E,

        WM_AFXLAST = 0x037F,

        WM_PENWINFIRST = 0x0380,

        WM_PENWIN_reserved_381 = 0x0381,

        WM_PENWIN_reserved_382 = 0x0382,

        WM_PENWIN_reserved_383 = 0x0383,

        WM_PENWIN_reserved_384 = 0x0384,

        WM_PENWIN_reserved_385 = 0x0385,

        WM_PENWIN_reserved_386 = 0x0386,

        WM_PENWIN_reserved_387 = 0x0387,

        WM_PENWIN_reserved_388 = 0x0388,

        WM_PENWIN_reserved_389 = 0x0389,

        WM_PENWIN_reserved_38a = 0x038A,

        WM_PENWIN_reserved_38b = 0x038B,

        WM_PENWIN_reserved_38c = 0x038C,

        WM_PENWIN_reserved_38d = 0x038D,

        WM_PENWIN_reserved_38e = 0x038E,

        WM_PENWINLAST = 0x038F,

        WM_COALESCE_FIRST = 0x0390,

        WM_COALESCE__reserved_391 = 0x0391,

        WM_COALESCE__reserved_392 = 0x0392,

        WM_COALESCE__reserved_393 = 0x0393,

        WM_COALESCE__reserved_394 = 0x0394,

        WM_COALESCE__reserved_395 = 0x0395,

        WM_COALESCE__reserved_396 = 0x0396,

        WM_COALESCE__reserved_397 = 0x0397,

        WM_COALESCE__reserved_398 = 0x0398,

        WM_COALESCE__reserved_399 = 0x0399,

        WM_COALESCE__reserved_39a = 0x039A,

        WM_COALESCE__reserved_39b = 0x039B,

        WM_COALESCE__reserved_39c = 0x039C,

        WM_COALESCE__reserved_39d = 0x039D,

        WM_COALESCE__reserved_39e = 0x039E,

        WM_COALESCE_LAST = 0x039F,

        WM_MM_RESERVED_FIRST = 0x03A0,

        WM_MM_RESERVED__reserved_3a1 = 0x03A1,

        WM_MM_RESERVED__reserved_3a2 = 0x03A2,

        WM_MM_RESERVED__reserved_3a3 = 0x03A3,

        WM_MM_RESERVED__reserved_3a4 = 0x03A4,

        WM_MM_RESERVED__reserved_3a5 = 0x03A5,

        WM_MM_RESERVED__reserved_3a6 = 0x03A6,

        WM_MM_RESERVED__reserved_3a7 = 0x03A7,

        WM_MM_RESERVED__reserved_3a8 = 0x03A8,

        WM_MM_RESERVED__reserved_3a9 = 0x03A9,

        WM_MM_RESERVED__reserved_3aA = 0x03AA,

        WM_MM_RESERVED__reserved_3ab = 0x03AB,

        WM_MM_RESERVED__reserved_3ac = 0x03AC,

        WM_MM_RESERVED__reserved_3ad = 0x03AD,

        WM_MM_RESERVED__reserved_3ae = 0x03AE,

        WM_MM_RESERVED__reserved_3af = 0x03AF,

        WM_MM_RESERVED__reserved_3b0 = 0x03B0,

        WM_MM_RESERVED__reserved_3b1 = 0x03B1,

        WM_MM_RESERVED__reserved_3b2 = 0x03B2,

        WM_MM_RESERVED__reserved_3b3 = 0x03B3,

        WM_MM_RESERVED__reserved_3b4 = 0x03B4,

        WM_MM_RESERVED__reserved_3b5 = 0x03B5,

        WM_MM_RESERVED__reserved_3b6 = 0x03B6,

        WM_MM_RESERVED__reserved_3b7 = 0x03B7,

        WM_MM_RESERVED__reserved_3b8 = 0x03B8,

        WM_MM_RESERVED__reserved_3b9 = 0x03B9,

        WM_MM_RESERVED__reserved_3ba = 0x03BA,

        WM_MM_RESERVED__reserved_3bb = 0x03BB,

        WM_MM_RESERVED__reserved_3bc = 0x03BC,

        WM_MM_RESERVED__reserved_3bd = 0x03BD,

        WM_MM_RESERVED__reserved_3be = 0x03BE,

        WM_MM_RESERVED__reserved_3bf = 0x03BF,

        WM_MM_RESERVED__reserved_3c0 = 0x03C0,

        WM_MM_RESERVED__reserved_3c1 = 0x03C1,

        WM_MM_RESERVED__reserved_3c2 = 0x03C2,

        WM_MM_RESERVED__reserved_3c3 = 0x03C3,

        WM_MM_RESERVED__reserved_3c4 = 0x03C4,

        WM_MM_RESERVED__reserved_3c5 = 0x03C5,

        WM_MM_RESERVED__reserved_3c6 = 0x03C6,

        WM_MM_RESERVED__reserved_3c7 = 0x03C7,

        WM_MM_RESERVED__reserved_3c8 = 0x03C8,

        WM_MM_RESERVED__reserved_3c9 = 0x03C9,

        WM_MM_RESERVED__reserved_3ca = 0x03CA,

        WM_MM_RESERVED__reserved_3cb = 0x03CB,

        WM_MM_RESERVED__reserved_3cc = 0x03CC,

        WM_MM_RESERVED__reserved_3cd = 0x03CD,

        WM_MM_RESERVED__reserved_3ce = 0x03CE,

        WM_MM_RESERVED__reserved_3cf = 0x03CF,

        WM_MM_RESERVED__reserved_3d0 = 0x03D0,

        WM_MM_RESERVED__reserved_3d1 = 0x03D1,

        WM_MM_RESERVED__reserved_3d2 = 0x03D2,

        WM_MM_RESERVED__reserved_3d3 = 0x03D3,

        WM_MM_RESERVED__reserved_3d4 = 0x03D4,

        WM_MM_RESERVED__reserved_3d5 = 0x03D5,

        WM_MM_RESERVED__reserved_3d6 = 0x03D6,

        WM_MM_RESERVED__reserved_3d7 = 0x03D7,

        WM_MM_RESERVED__reserved_3d8 = 0x03D8,

        WM_MM_RESERVED__reserved_3d9 = 0x03D9,

        WM_MM_RESERVED__reserved_3da = 0x03DA,

        WM_MM_RESERVED__reserved_3db = 0x03DB,

        WM_MM_RESERVED__reserved_3dc = 0x03DC,

        WM_MM_RESERVED__reserved_3dd = 0x03DD,

        WM_MM_RESERVED__reserved_3de = 0x03DE,

        WM_MM_RESERVED_LAST = 0x03DF,

        WM_INTERNAL_DDE_FIRST = 0x03E0,

        WM_INTERNAL_DDE__reserved_3e1 = 0x03E1,

        WM_INTERNAL_DDE__reserved_3e2 = 0x03E2,

        WM_INTERNAL_DDE__reserved_3e3 = 0x03E3,

        WM_INTERNAL_DDE__reserved_3e4 = 0x03E4,

        WM_INTERNAL_DDE__reserved_3e5 = 0x03E5,

        WM_INTERNAL_DDE__reserved_3e6 = 0x03E6,

        WM_INTERNAL_DDE__reserved_3e7 = 0x03E7,

        WM_INTERNAL_DDE__reserved_3e8 = 0x03E8,

        WM_INTERNAL_DDE__reserved_3e9 = 0x03E9,

        WM_INTERNAL_DDE__reserved_3ea = 0x03EA,

        WM_INTERNAL_DDE__reserved_3eb = 0x03EB,

        WM_INTERNAL_DDE__reserved_3ec = 0x03EC,

        WM_INTERNAL_DDE__reserved_3ed = 0x03ED,

        WM_INTERNAL_DDE__reserved_3ee = 0x03EE,

        WM_INTERNAL_DDE_LAST = 0x03EF,

        WM_CBT_RESERVED_FIRST = 0x03F0,

        WM_CBT_RESERVED__reserved_3f1 = 0x03F1,

        WM_CBT_RESERVED__reserved_3f2 = 0x03F2,

        WM_CBT_RESERVED__reserved_3f3 = 0x03F3,

        WM_CBT_RESERVED__reserved_3f4 = 0x03F4,

        WM_CBT_RESERVED__reserved_3f5 = 0x03F5,

        WM_CBT_RESERVED__reserved_3f6 = 0x03F6,

        WM_CBT_RESERVED__reserved_3f7 = 0x03F7,

        WM_CBT_RESERVED__reserved_3f8 = 0x03F8,

        WM_CBT_RESERVED__reserved_3f9 = 0x03F9,

        WM_CBT_RESERVED__reserved_3fa = 0x03FA,

        WM_CBT_RESERVED__reserved_3fb = 0x03FB,

        WM_CBT_RESERVED__reserved_3fc = 0x03FC,

        WM_CBT_RESERVED__reserved_3fd = 0x03FD,

        WM_CBT_RESERVED__reserved_3fe = 0x03FE,

        WM_CBT_RESERVED_LAST = 0x03FF,

        /// <summary>
        /// 用于定义专用窗口类使用的专用消息，通常为 WM_USER+x 格式，其中 x 是整数值。
        /// </summary>
        WM_USER = 0x0400,

        /// <summary>
        /// 用于定义专用消息，通常为 WM_APP+x 形式的专用消息，其中 x 是整数值。
        /// </summary>
        WM_APP = 0x8000,
    }
}
