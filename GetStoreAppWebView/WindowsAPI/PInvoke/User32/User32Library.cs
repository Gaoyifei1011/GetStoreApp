using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// User32.dll 函数库
    /// </summary>
    public static partial class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// 将指定点的工作区坐标转换为屏幕坐标。
        /// </summary>
        /// <param name="hWnd">其工作区用于转换的窗口的句柄。</param>
        /// <param name="lpPoint">指向 POINT 结构的指针，该结构包含要转换的客户端坐标。 如果函数成功，则新的屏幕坐标将复制到此结构中。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "ClientToScreen", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ClientToScreen(IntPtr hWnd, ref PointInt32 lpPoint);

        /// <summary>
        /// 创建具有扩展窗口样式的重叠窗口、弹出窗口窗口或子窗口;否则，此函数与 CreateWindow 函数相同。 有关创建窗口的详细信息以及 CreateWindowEx 的其他参数的完整说明，请参阅 CreateWindow。
        /// </summary>
        /// <param name="dwExStyle">正在创建的窗口的扩展窗口样式。</param>
        /// <param name="lpClassName">
        /// 由上一次调用 RegisterClass 或 RegisterClassEx 函数创建的以 null 结尾的字符串或类原子。原子必须位于 lpClassName 的低序字中；高序字必须为零。
        /// 如果 lpClassName 是字符串，则指定窗口类名称。类名可以是使用 RegisterClass 或 RegisterClassEx 注册的任何名称，前提是注册该类的模块也是创建窗口的模块。类名也可以是任何预定义的系统类名称。</param>
        /// <param name="lpWindowName">
        /// 窗口名称。如果窗口样式指定标题栏，则 lpWindowName 指向的窗口标题将显示在标题栏中。
        /// 使用 CreateWindow 创建控件（如按钮、检查框和静态控件）时，请使用 lpWindowName 指定控件的文本。
        /// 使用 SS_ICON 样式创建静态控件时，请使用 lpWindowName 指定图标名称或标识符。 若要指定标识符，请使用语法“#num”。
        /// </param>
        /// <param name="dwStyle">
        /// 正在创建的窗口的样式。 此参数可以是窗口样式值以及“备注”部分中指示的控件样式的组合。
        /// </param>
        /// <param name="x">
        /// 窗口的初始水平位置。 对于重叠或弹出窗口， x 参数是窗口左上角的初始 x 坐标（以屏幕坐标表示）。对于子窗口，x 是窗口左上角相对于父窗口工作区左上角的 x 坐标。如果 x 设置为 CW_USEDEFAULT，则系统会选择窗口左上角的默认位置，并忽略 y 参数。CW_USEDEFAULT 仅对重叠窗口有效;如果为弹出窗口或子窗口指定，则 x 和 y 参数设置为零。
        /// </param>
        /// <param name="y">
        /// 窗口的初始垂直位置。对于重叠或弹出窗口，y 参数是窗口左上角的初始 y 坐标（以屏幕坐标表示）。对于子窗口，y 是子窗口左上角相对于父窗口工作区左上角的初始 y 坐标。对于列表框 ，y 是列表框工作区左上角相对于父窗口工作区左上角的初始 y 坐标。
        /// 如果使用 WS_VISIBLE 样式位创建重叠窗口，并且 x 参数设置为 CW_USEDEFAULT，则 y 参数确定窗口的显示方式。 如果 y 参数CW_USEDEFAULT，则窗口管理器在创建窗口后使用SW_SHOW标志调用 ShowWindow。 如果 y 参数是其他某个值，则窗口管理器调用 ShowWindow ，该值作为 nCmdShow 参数。
        /// </param>
        /// <param name="nWidth">
        /// 窗口的宽度（以设备单位为单位）。对于重叠窗口， nWidth 是窗口的宽度、屏幕坐标或 CW_USEDEFAULT。如果 nWidth 是 CW_USEDEFAULT，系统将为窗口选择默认宽度和高度；默认宽度从初始 x 坐标扩展到屏幕的右边缘；默认高度从初始 y 坐标扩展到图标区域的顶部。 CW_USEDEFAULT 仅对重叠窗口有效；如果为弹出窗口或子窗口指定 了CW_USEDEFAULT ，则 nWidth 和 nHeight 参数设置为零。
        /// </param>
        /// <param name="nHeight">
        /// 窗口的高度（以设备单位为单位）。对于重叠窗口， nHeight 是窗口的高度（以屏幕坐标为单位）。如果 nWidth 参数设置为 CW_USEDEFAULT，则系统将忽略 nHeight。
        /// </param>
        /// <param name="hWndParent">
        /// 正在创建的窗口的父窗口或所有者窗口的句柄。若要创建子窗口或拥有的窗口，请提供有效的窗口句柄。 此参数对于弹出窗口是可选的。
        /// 若要创建仅消息窗口，请向现有仅消息窗口提供 HWND_MESSAGE 或句柄。</param>
        /// <param name="hMenu">
        /// 菜单句柄，或指定子窗口标识符，具体取决于窗口样式。对于重叠或弹出窗口， hMenu 标识要与窗口一起使用的菜单;如果要使用类菜单，则它可以为 NULL 。
        /// 对于子窗口， hMenu 指定子窗口标识符，即对话框控件用于通知其父级事件的整数值。 应用程序确定子窗口标识符;对于具有相同父窗口的所有子窗口，它必须是唯一的。
        /// </param>
        /// <param name="hInstance">要与窗口关联的模块实例的句柄。</param>
        /// <param name="lpParam">
        /// 指向要通过 CREATESTRUCT 结构传递到窗口的值的指针，（WM_CREATE消息的 lParam 参数指向的 lpCreateParams 成员)）。 此消息在返回之前由此函数发送到创建的窗口。
        /// 如果应用程序调用 CreateWindow 来创建 MDI 客户端窗口，lpParam 应指向 CLIENTCREATESTRUCT 结构。如果 MDI 客户端窗口调用 CreateWindow 来创建 MDI 子窗口， lpParam 应指向 MDICREATESTRUCT 结构。如果不需要其他数据，lpParam 可能为 NULL。</param>
        /// <returns>如果函数成功，则返回值是新窗口的句柄。如果函数失败，则返回值为 NULL。</returns>
        [LibraryImport(User32, EntryPoint = "CreateWindowExW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        public static partial IntPtr CreateWindowEx(uint dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, uint dwStyle, uint x, uint y, uint nWidth, uint nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        /// <summary>
        /// 调用默认窗口过程，为应用程序不处理的任何窗口消息提供默认处理。 此函数确保处理每条消息。 使用窗口过程接收的相同参数调用 DefWindowProc。
        /// </summary>
        /// <param name="hWnd">接收消息的窗口过程的句柄。</param>
        /// <param name="msg">消息。</param>
        /// <param name="wParam">其他消息信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <param name="lParam">其他消息信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <returns>返回值是消息处理的结果，取决于消息。</returns>
        [LibraryImport(User32, EntryPoint = "DefWindowProcW", SetLastError = false), PreserveSig]
        public static partial IntPtr DefWindowProc(IntPtr hWnd, WindowMessage msg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 销毁指定的窗口。 函数将 WM_DESTROY 和 WM_NCDESTROY 消息发送到窗口，以停用窗口并从窗口中删除键盘焦点。 如果窗口位于查看器链) 的顶部，函数还会销毁窗口的菜单、销毁计时器、删除剪贴板所有权，并中断剪贴板查看器链 (。
        /// 如果指定的窗口是父窗口或所有者窗口， 则 DestroyWindow 会在销毁父窗口或所有者窗口时自动销毁关联的子窗口或拥有窗口。 函数首先销毁子窗口或拥有的窗口，然后销毁父窗口或所有者窗口。
        /// DestroyWindow 还会销毁 CreateDialog 函数创建的无模式对话框。
        /// </summary>
        /// <param name="hWnd">要销毁的窗口的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "DestroyWindow", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DestroyWindow(IntPtr hWnd);

        /// <summary>
        /// 如果窗口附加到调用线程的消息队列，则检索具有键盘焦点的窗口的句柄。
        /// </summary>
        /// <returns>返回值是具有键盘焦点的窗口的句柄。 如果调用线程的消息队列没有与键盘焦点关联的窗口，则返回值为 NULL。</returns>
        [LibraryImport(User32, EntryPoint = "GetFocus", SetLastError = false), PreserveSig]
        public static partial IntPtr GetFocus();

        /// <summary>
        /// 注册一个窗口类，以便在调用 CreateWindow 或 CreateWindowEx 函数时使用。
        /// </summary>
        /// <param name="lpWndClass">指向 WNDCLASS 结构的指针。 在将结构传递给函数之前，必须使用相应的类属性填充结构。</param>
        /// <returns>
        /// 如果函数成功，则返回值是唯一标识所注册类的类原子。 此原子只能由 CreateWindow、 CreateWindowEx、 GetClassInfo、 GetClassInfoEx、 FindWindow、 FindWindowEx 和 UnregisterClass 函数和 IActiveIMMap：：FilterClientWindows 方法使用。
        /// 如果函数失败，则返回值为零。
        /// </returns>
        [LibraryImport(User32, EntryPoint = "RegisterClassW", SetLastError = false), PreserveSig]
        public static partial ushort RegisterClass(in WNDCLASS lpWndClass);

        /// <summary>
        /// 将指定的消息发送到窗口或窗口。 SendMessageW 函数调用指定窗口的窗口过程，在窗口过程处理消息之前不会返回。
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
        [LibraryImport(User32, EntryPoint = "SendMessageW", SetLastError = false), PreserveSig]
        public static partial int SendMessage(IntPtr hWnd, WindowMessage wMsg, UIntPtr wParam, IntPtr lParam);
    }
}
