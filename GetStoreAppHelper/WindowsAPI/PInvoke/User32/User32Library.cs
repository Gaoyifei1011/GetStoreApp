using GetStoreAppHelper.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.User32
{
    public class User32Library
    {
        private const string User32 = "User32.dll";

        /// <summary>
        /// 创建具有扩展窗口样式的重叠、弹出窗口或子窗口;否则，此函数与 InitializeWindow 函数相同。
        /// 有关创建窗口以及 <see cref="CreateWindowEx"> 的其他参数的完整说明的详细信息，请参阅 InitializeWindow。
        /// </summary>
        /// <param name="dwExStyle">正在创建的窗口的扩展窗口样式。 </param>
        /// <param name="lpClassName">
        /// 由上一次对 <see cref="RegisterClass"> 或 RegisterClassEx 函数的调用创建的空终止字符串或类原子。 原子必须位于 <param  name="lpClassName"> 的低序单词中;高序单词必须为零。 如果 <param  name="lpClassName"> 是字符串，则指定窗口类名称。 类名可以是注册到 <see cref="RegisterClass"> 或 RegisterClassEx 的任何名称，前提是注册该类的模块也是创建窗口的模块。 类名也可以是任何预定义的系统类名称。
        /// </param>
        /// <param name="lpWindowName">
        /// 窗口名称。 如果窗口样式指定标题栏，则 <param name="lpWindowName"> 指向的窗口标题将显示在标题栏中。 使用 InitializeWindow 创建控件（如按钮、复选框和静态控件）时，请使用 <param name="lpWindowName"> 指定控件的文本。 使用 SS_ICON 样式创建静态控件时，请使用 <param name="lpWindowName"> 指定图标名称或标识符。 若要指定标识符，请使用语法“#num”。
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
        /// 窗口的宽度（以设备单位为单位）。 对于重叠的窗口， <param name="nWidth"> 是窗口的宽度、屏幕坐标或 CW_USEDEFAULT。
        /// 如果 <param name="nWidth"> 是CW_USEDEFAULT，则系统会为窗口选择默认宽度和高度;默认宽度从初始 x 坐标扩展到屏幕的右边缘;默认高度从初始 y 坐标扩展到图标区域的顶部。 CW_USEDEFAULT 仅适用于重叠窗口;如果为弹出窗口或子窗口指定 了CW_USEDEFAULT ， 则 <param name="nWidth"> 和 <param name="nHeight"> 参数设置为零。
        /// </param>
        /// <param name="nHeight">
        /// 窗口的高度（以设备单位为单位）。 对于重叠窗口， <param name="nHeight"> 是窗口的高度（以屏幕坐标为单位）。
        /// 如果 <param name="nWidth"> 参数设置为 CW_USEDEFAULT，则系统将忽略 <param name="nHeight">。
        /// </param>
        /// <param name="hWndParent">
        /// 正在创建的窗口的父窗口或所有者窗口的句柄。 若要创建子窗口或拥有的窗口，请提供有效的窗口句柄。 对于弹出窗口，此参数是可选的。
        /// </param>
        /// <param name="hMenu">
        /// 菜单的句柄，或指定子窗口标识符，具体取决于窗口样式。 对于重叠或弹出窗口， <param name="hMenu"> 标识要与窗口一起使用的菜单；如果使用类菜单，则为 NULL 。
        /// 对于子窗口， <param name="hMenu"> 指定子窗口标识符，即对话框控件用来通知其父级事件的整数值。
        /// 应用程序确定子窗口标识符;对于具有相同父窗口的所有子窗口，它必须是唯一的。
        /// </param>
        /// <param name="hInstance">要与窗口关联的模块实例的句柄。</param>
        /// <param name="lpParam">
        /// 指向通过 CREATESTRUCT 结构传递给窗口的值的指针， (lpCreateParams 成员) <see cref="WindowMessage.WM_CREATE"> 消息的 <param name="lpParam"> 参数所指向的值。 此消息在返回之前由此函数发送到创建的窗口。
        /// </param>
        /// <returns>如果函数成功，则返回值是新窗口的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
           WindowStylesEx dwExStyle,
           string lpClassName,
           string lpWindowName,
           WindowStyles dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        /// <summary>
        /// 调用默认窗口过程，为应用程序未处理的任何窗口消息提供默认处理。 此函数可确保处理每个消息。 使用窗口过程收到的相同参数调用 <see cref="DefWindowProc">。
        /// </summary>
        /// <param name="hWnd">接收消息的窗口过程的句柄。</param>
        /// <param name="Msg">其他消息信息。</param>
        /// <param name="wParam">其他消息信息。 此参数的内容取决于 <param name="Msg"> 参数的值。</param>
        /// <param name="lParam">其他消息信息。 此参数的内容取决于 <param name="Msg"> 参数的值。</param>
        /// <returns>返回值是消息处理的结果，取决于消息。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "DefWindowProc", SetLastError = false)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 销毁指定的窗口。 该函数将 <see cref="WindowMessage.WM_DESTROY"> 和 <see cref="WindowMessage.WM_NCDESTROY"> 消息发送到窗口以停用它，并从中删除键盘焦点。 该函数还会销毁窗口的菜单、刷新线程消息队列、销毁计时器、删除剪贴板所有权，如果窗口位于查看器链顶部) ，则中断剪贴板查看器链。
        /// 如果指定的窗口是父窗口或所有者窗口， 则 <see cref="DestroyWindow"> 会在销毁父窗口或所有者窗口时自动销毁关联的子窗口或拥有窗口。
        /// 该函数首先销毁子窗口或拥有的窗口，然后销毁父窗口或所有者窗口。
        /// <see cref="DestroyWindow"> 还会销毁 CreateDialog 函数创建的无模式对话框。
        /// </summary>
        /// <param name="hwnd">要销毁的窗口的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "DestroyWindow", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport(User32, SetLastError = true)]
        public static extern IntPtr DispatchMessage(ref MSG lpmsg);

        /// <summary>
        /// 检索鼠标光标的位置（以屏幕坐标为单位）。
        /// </summary>
        /// <param name="lpPoint">指向接收光标屏幕坐标的 <see cref="PointInt32"> 结构的指针。</param>
        /// <returns>如果成功，则返回非零值，否则返回零。 </returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "GetCursorPos", SetLastError = false)]
        public static extern bool GetCursorPos(out PointInt32 lpPoint);

        /// <summary>Retrieves the handle to the ancestor of the specified window.</summary>
        /// <param name="hWnd">
        ///     A handle to the window whose ancestor is to be retrieved. If this parameter is the desktop window,
        ///     the function returns <see cref="IntPtr.Zero" />.
        /// </param>
        /// <param name="gaFlags">The ancestor to be retrieved.</param>
        /// <returns>The handle to the ancestor window.</returns>
        [DllImport(nameof(User32), SetLastError = true)]
        public static extern IntPtr GetAncestor(IntPtr hWnd, GetAncestorFlags gaFlags);

        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, WindowMessage wMsgFilterMin, WindowMessage wMsgFilterMax);

        /// <summary>
        /// Determines the visibility state of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be tested.</param>
        /// <returns>
        /// If the specified window, its parent window, its parent's parent window, and so forth, have the WS_VISIBLE style, the return value is true, otherwise it is false.
        /// Because the return value specifies whether the window has the WS_VISIBLE style, it may be nonzero even if the window is totally obscured by other windows.
        /// </returns>
        /// <remarks>
        /// The visibility state of a window is indicated by the WS_VISIBLE style bit.
        /// When WS_VISIBLE is set, the window is displayed and subsequent drawing into it is displayed as long as the window has the WS_VISIBLE style.
        /// Any drawing to a window with the WS_VISIBLE style will not be displayed if the window is obscured by other windows or is clipped by its parent window.
        /// </remarks>
        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

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
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "LoadImage", SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, ImageType type, int cx, int cy, LoadImageFlags fuLoad);

        /// <summary>
        /// 注册一个窗口类，以便在对 CreateWindow 或 <see cref="CreateWindowEx"> 函数的调用中随后使用。
        /// </summary>
        /// <param name="lpWndClass">指向 <see cref="WindowClass"> 结构的指针。 在将结构传递给函数之前，必须用相应的类属性填充结构。</param>
        /// <return>如果函数成功，则返回值是唯一标识所注册类的类原子。 如果函数失败，则返回值为零。</return>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "RegisterClassW", SetLastError = true)]
        public static extern short RegisterClass(ref WindowClass lpWndClass);

        /// <summary>
        /// 定义保证在整个系统中唯一的新窗口消息。 发送或发布消息时可以使用消息值。
        /// </summary>
        /// <param name="lpString">要注册的消息。</param>
        /// <returns>如果成功注册消息，则返回值是范围0xC000到0xFFFF的消息标识符。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "RegisterWindowMessageW", SetLastError = false)]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        /// <summary>
        /// 将创建指定窗口的线程引入前台并激活窗口。 键盘输入将定向到窗口，并为用户更改各种视觉提示。 系统向创建前台窗口的线程分配略高于其他线程的优先级。
        /// </summary>
        /// <param name="hWnd">应激活并带到前台的窗口的句柄。</param>
        /// <returns>如果窗口被带到前台，则返回值为非零。如果未将窗口带到前台，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "SetForegroundWindow", SetLastError = false)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "SendMessage", SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(User32, CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow([In] IntPtr hWnd, WindowShowStyle nCmdShow);

        [DllImport(User32, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UpdateWindow([In] IntPtr hWnd);

        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage(ref MSG lpMsg);
    }
}
