using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 从 User32.dll Windows库中导出的函数。
    /// </summary>
    public static class User32Library
    {
        private const string User32 = "User32.dll";

        /// <summary>
        /// 将消息信息传递给指定的窗口过程。
        /// </summary>
        /// <param name="lpPrevWndFunc">
        /// 上一个窗口过程。 如果通过调用设置为GWL_WNDPROC或DWL_DLGPROC的 nIndex 参数的 GetWindowLong 函数来获取此值，
        /// 则它实际上是窗口或对话框过程的地址，或者仅对 CallWindowProc 有意义的特殊内部值。</param>
        /// <param name="hWnd">用于接收消息的窗口过程的句柄。</param>
        /// <param name="Msg">消息。</param>
        /// <param name="wParam">其他的消息特定信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <param name="lParam">其他的消息特定信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <returns>返回值指定消息处理的结果，具体取决于发送的消息。</returns>
        [DllImport(User32)]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 创建下拉菜单、子菜单或快捷菜单。 菜单最初为空。 可以使用 InsertMenuItem 函数插入或追加菜单项。
        /// 还可以使用 InsertMenu 函数插入菜单项和 AppendMenu 函数来追加菜单项。
        /// </summary>
        /// <returns>如果函数成功，则返回值是新创建的菜单的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, EntryPoint = "CreatePopupMenu", CharSet = CharSet.Auto)]
        public static extern IntPtr CreatePopupMenu();

        /// <summary>
        /// 创建具有扩展窗口样式的重叠、弹出窗口或子窗口;否则，此函数与 CreateWindow 函数相同。
        /// 有关创建窗口以及 CreateWindowEx 的其他参数的完整说明的详细信息，请参阅 CreateWindow。
        /// </summary>
        /// <param name="dwExStyle">正在创建的窗口的扩展窗口样式。 </param>
        /// <param name="lpClassName">
        /// 由上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的空终止字符串或类原子。 原子必须位于 lpClassName 的低序单词中;高序单词必须为零。
        /// 如果 lpClassName 是字符串，则指定窗口类名称。 类名可以是注册到 RegisterClass 或 RegisterClassEx 的任何名称，前提是注册该类的模块也是创建窗口的模块。
        /// 类名也可以是任何预定义的系统类名称。
        /// </param>
        /// <param name="lpWindowName">
        /// 窗口名称。 如果窗口样式指定标题栏，则 lpWindowName 指向的窗口标题将显示在标题栏中。 使用 CreateWindow 创建控件（如按钮、复选框和静态控件）时，
        /// 请使用 lpWindowName 指定控件的文本。 使用 SS_ICON 样式创建静态控件时，请使用 lpWindowName 指定图标名称或标识符。
        /// 若要指定标识符，请使用语法“#num”。
        /// </param>
        /// <param name="dwStyle">正在创建的窗口的样式。</param>
        /// <param name="x">
        /// 窗口的初始水平位置。 对于重叠或弹出窗口， x 参数是窗口左上角的初始 x 坐标，以屏幕坐标表示。
        /// 对于子窗口， x 是窗口左上角相对于父窗口工作区左上角的 x 坐标。 如果 x 设置为 CW_USEDEFAULT，系统将选择窗口左上角的默认位置，并忽略 y 参数。
        /// CW_USEDEFAULT 仅适用于重叠窗口;如果为弹出窗口或子窗口指定， 则 x 和 y 参数设置为零。
        /// </param>
        /// <param name="y">
        /// 窗口的初始垂直位置。 对于重叠或弹出窗口， y 参数是窗口左上角的初始 y 坐标，以屏幕坐标表示。
        /// 对于子窗口， y 是子窗口左上角相对于父窗口工作区左上角的初始 y 坐标。 对于列表框 y ，是列表框工作区左上角相对于父窗口工作区左上角的初始 y 坐标。
        /// </param>
        /// <param name="nWidth">
        /// 窗口的宽度（以设备单位为单位）。 对于重叠的窗口， nWidth 是窗口的宽度、屏幕坐标或 CW_USEDEFAULT。
        /// 如果 nWidth是CW_USEDEFAULT，则系统会为窗口选择默认宽度和高度;默认宽度从初始 x 坐标扩展到屏幕的右边缘;默认高度从初始 y 坐标扩展到图标区域的顶部。
        /// CW_USEDEFAULT 仅适用于重叠窗口;如果为弹出窗口或子窗口指定 了CW_USEDEFAULT ， 则 nWidth 和 nHeight 参数设置为零。
        /// </param>
        /// <param name="nHeight">
        /// 窗口的高度（以设备单位为单位）。 对于重叠窗口， nHeight 是窗口的高度（以屏幕坐标为单位）。
        /// 如果 nWidth 参数设置为 CW_USEDEFAULT，则系统将忽略 nHeight。
        /// </param>
        /// <param name="hWndParent">
        /// 正在创建的窗口的父窗口或所有者窗口的句柄。 若要创建子窗口或拥有的窗口，请提供有效的窗口句柄。 对于弹出窗口，此参数是可选的。
        /// </param>
        /// <param name="hMenu">
        /// 菜单的句柄，或指定子窗口标识符，具体取决于窗口样式。 对于重叠或弹出窗口， hMenu 标识要与窗口一起使用的菜单；如果使用类菜单，则为 NULL 。
        /// 对于子窗口， hMenu 指定子窗口标识符，即对话框控件用来通知其父级事件的整数值。
        /// 应用程序确定子窗口标识符;对于具有相同父窗口的所有子窗口，它必须是唯一的。
        /// </param>
        /// <param name="hInstance">要与窗口关联的模块实例的句柄。</param>
        /// <param name="lpParam">
        /// 指向通过 CREATESTRUCT 结构传递给窗口的值的指针， (lpCreateParams 成员) WM_CREATE消息的lParam 参数所指向的值。
        /// 此消息在返回之前由此函数发送到创建的窗口。
        /// </param>
        /// <returns>如果函数成功，则返回值是新窗口的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, SetLastError = true, EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx(
           int dwExStyle,
           string lpClassName,
           string lpWindowName,
           int dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        /// <summary>
        /// 调用默认窗口过程，为应用程序未处理的任何窗口消息提供默认处理。 此函数可确保处理每个消息。 使用窗口过程收到的相同参数调用 DefWindowProc。
        /// </summary>
        /// <param name="hWnd">接收消息的窗口过程的句柄。</param>
        /// <param name="Msg">其他消息信息。</param>
        /// <param name="wParam">其他消息信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <param name="lParam">其他消息信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <returns>返回值是消息处理的结果，取决于消息。</returns>
        [DllImport(User32)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 销毁指定的菜单并释放菜单占用的任何内存。
        /// </summary>
        /// <param name="hMenu">要销毁的菜单的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, SetLastError = true)]
        public static extern bool DestroyMenu(IntPtr hMenu);

        /// <summary>
        /// 销毁指定的窗口。 该函数将 WM_DESTROY 和 WM_NCDESTROY 消息发送到窗口以停用它，并从中删除键盘焦点。 该函数还会销毁窗口的菜单、刷新线程消息队列、
        /// 销毁计时器、删除剪贴板所有权，如果窗口位于查看器链顶部) ，则中断剪贴板查看器链。
        /// 如果指定的窗口是父窗口或所有者窗口， 则 DestroyWindow 会在销毁父窗口或所有者窗口时自动销毁关联的子窗口或拥有窗口。
        /// 该函数首先销毁子窗口或拥有的窗口，然后销毁父窗口或所有者窗口。
        /// DestroyWindow 还会销毁 CreateDialog 函数创建的无模式对话框。
        /// </summary>
        /// <param name="hwnd">要销毁的窗口的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        /// <summary>
        /// 返回指定窗口的每英寸点 (dpi) 值。
        /// </summary>
        /// <param name="hwnd">要获取相关信息的窗口。</param>
        /// <returns>窗口的 DPI，无效 的 hwnd 值将导致返回值 0。</returns>
        [DllImport(User32)]
        public static extern int GetDpiForWindow(IntPtr hwnd);

        /// <summary>
        /// 检索鼠标光标的位置（以屏幕坐标为单位）。
        /// </summary>
        /// <param name="lpPoint">指向接收光标屏幕坐标的 POINT 结构的指针。</param>
        /// <returns>如果成功，则返回非零值，否则返回零。 </returns>
        [DllImport(User32)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        /// <summary>
        /// 检索顶级窗口的句柄，该窗口的类名称和窗口名称与指定的字符串匹配。 此函数不搜索子窗口。 此函数不执行区分大小写的搜索。
        /// </summary>
        /// <param name="lpClassName">
        /// 类名或上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的类名或类原子。原子必须位于 lpClassName 的低序单词中;高阶单词必须为零。
        /// 如果 lpClassName 指向字符串，则指定窗口类名。 类名可以是向 RegisterClass 或 RegisterClassEx 注册的任何名称，也可以是预定义控件类名称中的任何名称。
        /// 如果 lpClassName 为 NULL，它将查找其标题与 lpWindowName 参数匹配的任何窗口。
        /// </param>
        /// <param name="lpWindowName">窗口名称 (窗口的标题) 。 如果此参数为 NULL，则所有窗口名称都匹配。</param>
        /// <returns>如果函数成功，则返回值是具有指定类名和窗口名称的窗口的句柄。 如果函数失败，则返回值为 NULL。 </returns>
        [DllImport(User32, EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        /// <summary>检索前台窗口的句柄， (用户当前正在使用的窗口) 。 系统向创建前台窗口的线程分配略高于其他线程的优先级。</summary>
        /// <returns>返回值是前台窗口的句柄。 在某些情况下，前台窗口可以为 NULL ，例如窗口丢失激活时。</returns>
        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 检索指定窗口的边界矩形的尺寸。 尺寸以相对于屏幕左上角的屏幕坐标提供。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="lpRect">指向 RECT 结构的指针，用于接收窗口左上角和右下角的屏幕坐标。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(nameof(User32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// 在菜单中的指定位置插入新菜单项。
        /// </summary>
        /// <param name="hMenu">插入新菜单项的菜单的句柄。</param>
        /// <param name="uItem">要在其中插入新项的菜单项的标识符或位置。 此参数的含义取决于 fByPosition 的值。</param>
        /// <param name="fByPosition">控制项的含义。 如果此参数为 FALSE， 则项 为菜单项标识符。 否则，它是菜单项位置。 </param>
        /// <param name="lpmii">指向 MENUITEMINFO 结构的指针，其中包含有关新菜单项的信息。</param>
        /// <returns>如果该函数成功，则返回值为非零值。 如果函数失败，则返回值为零。</returns>
        [DllImport("User32.dll")]
        public static extern bool InsertMenuItem(IntPtr hMenu, int uItem, bool fByPosition, [In] ref MENUITEMINFO lpmii);

        /// <summary>
        /// 确定窗口是否最大化
        /// </summary>
        /// <param name="hWnd">要测试的窗口的句柄</param>
        /// <returns>如果缩放窗口，则返回值为非零。如果未缩放窗口，则返回值为零。</returns>
        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsZoomed(IntPtr hWnd);

        /// <summary>
        /// 加载图标、游标、动画游标或位图。
        /// </summary>
        /// <param name="hinst">DLL 或可执行文件 (.exe 模块的句柄) ，其中包含要加载的图像。</param>
        /// <param name="lpszName">要加载的图像。</param>
        /// <param name="type">要加载的图像的类型。</param>
        /// <param name="cx">图标或光标的宽度（以像素为单位）。</param>
        /// <param name="cy">图标或光标的高度（以像素为单位）。</param>
        /// <param name="fuLoad">此参数可使用以下一个或多个值。</param>
        /// <returns>如果函数成功，则返回值是新加载的图像的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, ImageType type, int cx, int cy, LoadImageFlags fuLoad);

        /// <summary>
        /// 显示一个模式对话框，其中包含系统图标、一组按钮和一条简短的应用程序特定消息，例如状态或错误信息。 消息框返回一个整数值，该值指示用户单击的按钮。
        /// </summary>
        /// <param name="hWnd">要创建的消息框的所有者窗口的句柄。 如果此参数为 NULL，则消息框没有所有者窗口。</param>
        /// <param name="lptext">要显示的消息。 如果字符串包含多个行，则可以使用回车符和/或换行符分隔每行之间的行。</param>
        /// <param name="lpcaption">对话框标题。 如果此参数为 NULL，则默认标题为 Error。</param>
        /// <param name="options">对话框的内容和行为。</param>
        /// <returns>
        /// 如果消息框有 “取消 ”按钮，则函数返回 IDCANCEL 值（如果按下 ESC 键或选中 “取消 ”按钮）。
        /// 如果消息框没有 “取消 ”按钮，则按 ESC 将不起作用 -除非存在MB_OK按钮。 如果显示MB_OK按钮，并且用户按 ESC，则返回值为 IDOK。
        /// 如果函数失败，则返回值为零。
        /// 如果函数成功，则返回值为 MessageBoxResult 的枚举值之一。
        /// </returns>
        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern MessageBoxResult MessageBox(IntPtr hWnd, string lptext, string lpcaption, MessageBoxOptions options);

        /// <summary>
        /// 注册一个窗口类，以便在对 CreateWindow 或 CreateWindowEx 函数的调用中随后使用。
        /// </summary>
        /// <param name="lpWndClass">指向 WNDCLASS 结构的指针。 在将结构传递给函数之前，必须用相应的类属性填充结构。</param>
        /// <return>如果函数成功，则返回值是唯一标识所注册类的类原子。 如果函数失败，则返回值为零。</return>
        [DllImport(User32, EntryPoint = "RegisterClassW", SetLastError = true)]
        public static extern short RegisterClass(ref WindowClass lpWndClass);

        /// <summary>
        /// 定义保证在整个系统中唯一的新窗口消息。 发送或发布消息时可以使用消息值。
        /// </summary>
        /// <param name="lpString">要注册的消息。</param>
        /// <returns>如果成功注册消息，则返回值是范围0xC000到0xFFFF的消息标识符。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, EntryPoint = "RegisterWindowMessageW")]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        /// <summary>
        /// 将指定的消息发送到窗口或窗口。 SendMessage 函数调用指定窗口的窗口过程，在窗口过程处理消息之前不会返回。
        /// </summary>
        /// <param name="hWnd">
        /// 窗口过程的句柄将接收消息。 如果此参数 HWND_BROADCAST ( (HWND) 0xffff) ，则会将消息发送到系统中的所有顶级窗口，
        /// 包括已禁用或不可见的未所有者窗口、重叠窗口和弹出窗口;但消息不会发送到子窗口。消息发送受 UIPI 的约束。
        /// 进程的线程只能将消息发送到较低或等于完整性级别的线程的消息队列。
        /// </param>
        /// <param name="wMsg">要发送的消息。</param>
        /// <param name="wParam">其他的消息特定信息。</param>
        /// <param name="lParam">其他的消息特定信息。</param>
        /// <returns>返回值指定消息处理的结果;这取决于发送的消息。</returns>
        [DllImport(User32, EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, int wParam, ref CopyDataStruct lParam);

        /// <summary>
        /// 将创建指定窗口的线程引入前台并激活窗口。 键盘输入将定向到窗口，并为用户更改各种视觉提示。 系统向创建前台窗口的线程分配略高于其他线程的优先级。
        /// </summary>
        /// <param name="hWnd">应激活并带到前台的窗口的句柄。</param>
        /// <returns>如果窗口被带到前台，则返回值为非零。如果未将窗口带到前台，则返回值为零。</returns>
        [DllImport(User32)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的32位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定 32 位整数的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的64位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定偏移量的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        /// <summary>
        /// 更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。
        /// </summary>
        /// <param name="hWnd">更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。</param>
        /// <param name="hWndInsertAfter">在 Z 顺序中定位窗口之前窗口的句柄。 </param>
        /// <param name="X">在 Z 顺序中定位窗口之前窗口的句柄。 </param>
        /// <param name="Y">窗口顶部的新位置，以客户端坐标表示。</param>
        /// <param name="cx">窗口的新宽度（以像素为单位）。</param>
        /// <param name="cy">窗口的新高度（以像素为单位）。</param>
        /// <param name="uFlags">窗口大小调整和定位标志。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            SetWindowPosFlags uFlags);

        /// <summary>
        /// 设置指定窗口的显示状态。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="nCmdShow">
        /// 控制窗口的显示方式。 如果启动应用程序的程序提供 STARTUPINFO 结构，则首次调用 ShowWindow 时忽略此参数。
        /// 否则，首次调用 ShowWindow 时，该值应该是 WinMain 函数在其 nCmdShow 参数中获取的值。
        /// </param>
        /// <returns>如果窗口以前可见，则返回值为非零。如果窗口之前已隐藏，则返回值为零。</returns>
        [DllImport(User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        /// <summary>
        /// 将焦点切换到指定的窗口，并将其带到前台。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="fAltTab">此参数的 TRUE 指示窗口正切换到使用 Alt/Ctl+Tab 键序列。 否则此参数应为 FALSE 。</param>
        /// <returns>无</returns>
        [DllImport(User32, SetLastError = true)]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        /// <summary>
        /// 在指定位置显示快捷菜单，并跟踪快捷菜单上的项选择。 快捷菜单可在屏幕上的任意位置显示。
        /// </summary>
        /// <param name="hmenu">
        /// 要显示的快捷菜单的句柄。 可以通过调用 CreatePopupMenu 函数来创建新的快捷菜单或通过调用 GetSubMenu 函数来检索与现有菜单项关联的子菜单的句柄来获取此句柄。
        /// </param>
        /// <param name="flags">指定函数选项。</param>
        /// <param name="x">快捷菜单的水平位置，以屏幕坐标为单位。</param>
        /// <param name="y">快捷菜单的垂直位置，以屏幕坐标为单位。</param>
        /// <param name="reserved">保留;必须为零。</param>
        /// <param name="hwnd">
        /// 拥有快捷菜单的窗口的句柄。 此窗口从菜单中接收所有消息。 在函数返回之前，该窗口不会从菜单中收到 WM_COMMAND 消息。
        /// 如果在 uFlags 参数中指定TPM_NONOTIFY，函数不会将消息发送到 hWnd 标识的窗口。 但是，仍必须在 hWnd 中传递窗口句柄。
        /// 它可以是应用程序的任何窗口句柄。
        /// </param>
        /// <param name="prcRect"></param>
        /// <returns>
        /// 如果在 uFlags 参数中指定TPM_RETURNCMD，则返回值是用户选择的项的菜单项标识符。 如果用户取消菜单而不进行选择，或者发生错误，则返回值为零。
        /// 如果未在 uFlags 参数中指定TPM_RETURNCMD，则如果函数成功且失败，则返回值为零。
        /// </returns>
        [DllImport(User32)]
        public static extern int TrackPopupMenu(IntPtr hMenu, TrackPopupMenuFlags uFlags, int x, int y, int reserved, IntPtr hwnd, IntPtr prcRect);
    }
}
