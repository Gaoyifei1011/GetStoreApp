using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// User32.dll 函数库。
    /// </summary>
    public static partial class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// 将消息信息传递给指定的窗口过程。
        /// </summary>
        /// <param name="lpPrevWndFunc">
        /// 上一个窗口过程。 如果通过调用设置为GWL_WNDPROC或DWL_DLGPROC的 nIndex 参数的 GetWindowLong 函数来获取此值，
        /// 则它实际上是窗口或对话框过程的地址，或者仅对 <see cref="CallWindowProc"> 有意义的特殊内部值。</param>
        /// <param name="hWnd">用于接收消息的窗口过程的句柄。</param>
        /// <param name="Msg">消息。</param>
        /// <param name="wParam">其他的消息特定信息。 此参数的内容取决于 <param name="Msg"> 参数的值。</param>
        /// <param name="lParam">其他的消息特定信息。 此参数的内容取决于 <param name="Msg"> 参数的值。</param>
        /// <returns>返回值指定消息处理的结果，具体取决于发送的消息。</returns>
        [LibraryImport(User32, EntryPoint = "CallWindowProcW", SetLastError = false)]
        public static partial IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

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
        [LibraryImport(User32, EntryPoint = "CreateWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr CreateWindowEx(
           WindowStyleEx dwExStyle,
           string lpClassName,
           string lpWindowName,
           WindowStyle dwStyle,
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
        /// <param name="wParam">其他消息信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <param name="lParam">其他消息信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <returns>返回值是消息处理的结果，取决于消息。</returns>
        [LibraryImport(User32, EntryPoint = "DefWindowProcW", SetLastError = false)]
        public static partial IntPtr DefWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 销毁图标并释放图标占用的任何内存。
        /// </summary>
        /// <param name="hIcon">要销毁的图标的句柄。 图标不得使用。</param>
        /// <returns>如果该函数成功，则返回值为非零值。</returns>
        [LibraryImport(User32, EntryPoint = "DestroyIcon", SetLastError = true)]
        public static partial int DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// 销毁指定的窗口。 该函数将 WM_DESTROY 和 WM_NCDESTROY 消息发送到窗口以停用它，并从中删除键盘焦点。 该函数还会销毁窗口的菜单、刷新线程消息队列、
        /// 销毁计时器、删除剪贴板所有权，如果窗口位于查看器链顶部) ，则中断剪贴板查看器链。
        /// 如果指定的窗口是父窗口或所有者窗口， 则 DestroyWindow 会在销毁父窗口或所有者窗口时自动销毁关联的子窗口或拥有窗口。
        /// 该函数首先销毁子窗口或拥有的窗口，然后销毁父窗口或所有者窗口。
        /// DestroyWindow 还会销毁 CreateDialog 函数创建的无模式对话框。
        /// </summary>
        /// <param name="hwnd">要销毁的窗口的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "DestroyWindow", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DestroyWindow(IntPtr hwnd);

        /// <summary>
        /// 检索一个窗口的句柄，该窗口的类名和窗口名称与指定的字符串匹配。 该函数搜索子窗口，从指定子窗口后面的子窗口开始。 此函数不执行区分大小写的搜索。
        /// </summary>
        /// <param name="hWndParent">要搜索其子窗口的父窗口的句柄。如果 hwndParent 为 NULL，则该函数使用桌面窗口作为父窗口。 函数在桌面的子窗口之间搜索。 如果 hwndParent 为HWND_MESSAGE，则函数将搜索所有 仅消息窗口。</param>
        /// <param name="hWndChildAfter">子窗口的句柄。 搜索从 Z 顺序中的下一个子窗口开始。 子窗口必须是 hwndParent 的直接子窗口，而不仅仅是子窗口。 如果 hwndChildAfter 为 NULL，则搜索从 hwndParent 的第一个子窗口开始。请注意，如果 hwndParent 和 hwndChildAfter 均为 NULL，则该函数将搜索所有顶级窗口和仅消息窗口。</param>
        /// <param name="lpszClass">类名或上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的类名或类原子。 原子必须置于 lpszClass 的低序单词中;高阶单词必须为零。如果 lpszClass 是字符串，则指定窗口类名。 类名可以是注册到 RegisterClass 或 RegisterClassEx 的任何名称，也可以是预定义的控件类名称，也可以是 MAKEINTATOM(0x8000)。 在此后一种情况下，0x8000是菜单类的原子。 </param>
        /// <param name="lpszWindow">窗口名称 (窗口的标题) 。 如果此参数为 NULL，则所有窗口名称都匹配。</param>
        /// <returns>如果函数成功，则返回值是具有指定类和窗口名称的窗口的句柄。如果函数失败，则返回值为 NULL。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(User32, EntryPoint = "FindWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// 检索鼠标光标的位置（以屏幕坐标为单位）。
        /// </summary>
        /// <param name="lpPoint">指向接收光标屏幕坐标的 <see cref="PointInt32"> 结构的指针。</param>
        /// <returns>如果成功，则返回非零值，否则返回零。 </returns>
        [LibraryImport(User32, EntryPoint = "GetCursorPos", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe partial bool GetCursorPos(PointInt32* lpPoint);

        /// <summary>
        /// 返回指定窗口的每英寸点 (dpi) 值。
        /// </summary>
        /// <param name="hwnd">要获取相关信息的窗口。</param>
        /// <returns>窗口的 DPI，无效 的 <param name="hwnd"> 值将导致返回值 0。</returns>
        [LibraryImport(User32, EntryPoint = "GetDpiForWindow", SetLastError = false)]
        public static partial int GetDpiForWindow(IntPtr hwnd);

        /// <summary>
        /// <see cref="GetMonitorInfo"> 函数检索有关显示监视器的信息。
        /// </summary>
        /// <param name="hMonitor">感兴趣的显示监视器的句柄。</param>
        /// <param name="lpmi">
        /// 指向 <see cref="MONITORINFO"> 或 MONITORINFOEX 结构的指针，该结构接收有关指定显示监视器的信息。
        /// 在调用 GetMonitorInfo 函数之前，必须将结构的 cbSize 成员设置为 sizeof (MONITORINFO) 或 sizeof (MONITORINFOEX) 。 这样，函数就可以确定要传递给它的结构类型。
        /// <see cref="MONITORINFO"> 结构是 MONITORINFO 结构的超集。 它具有一个附加成员：一个字符串，其中包含显示监视器的名称。 大多数应用程序没有用于显示监视器名称，因此可以使用 MONITORINFO 结构保存某些字节。
        /// </param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "GetMonitorInfoW", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetMonitorInfo(IntPtr hMonitor, out MONITORINFO lpmi);

        /// <summary>
        /// 检索创建指定窗口的线程的标识符，以及（可选）创建窗口的进程的标识符。
        /// </summary>
        /// <param name="hwnd">窗口的句柄。</param>
        /// <param name="ID">指向接收进程标识符的变量的指针。如果此参数不为 NULL，则 <see cref="GetWindowThreadProcessId"> 将进程的标识符复制到变量;否则，它不会。</param>
        /// <returns>返回值是创建窗口的线程的标识符。</returns>
        [LibraryImport(User32, EntryPoint = "GetWindowThreadProcessId", SetLastError = true)]
        public static partial int GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

        /// <summary>
        /// <see cref="MonitorFromWindow"> 函数检索显示监视器的句柄，该监视器具有与指定窗口的边界矩形相交的最大区域。
        /// </summary>
        /// <param name="hwnd">感兴趣的窗口的句柄。</param>
        /// <param name="dwFlags">如果窗口不与任何显示监视器相交，则确定函数的返回值。</param>
        /// <returns>如果窗口与一个或多个显示监视器矩形相交，则返回值是显示监视器的 HMONITOR 句柄，该监视器具有与窗口的最大交集区域。
        /// 如果窗口不与显示监视器相交，则返回值取决于 <param name="dwFlags"> 的值。</returns>
        [LibraryImport(User32, EntryPoint = "MonitorFromWindow", SetLastError = false)]
        public static partial IntPtr MonitorFromWindow(IntPtr hwnd, MonitorFlags dwFlags);

        /// <summary>
        /// 创建从指定文件中提取的图标的句柄数组。
        /// </summary>
        /// <param name="lpszFile">要从中提取图标的文件的路径和名称。</param>
        /// <param name="nIconIndex">要提取的第一个图标的从零开始的索引。 例如，如果此值为零，函数将提取指定文件中的第一个图标。</param>
        /// <param name="cxIcon">所需的水平图标大小。 </param>
        /// <param name="cyIcon">所需的垂直图标大小。</param>
        /// <param name="phicon">指向返回的图标句柄数组的指针。</param>
        /// <param name="piconid">
        /// 指向最适合当前显示设备的图标返回的资源标识符的指针。 如果标识符不可用于此格式，则返回的标识符0xFFFFFFFF。 如果无法获取标识符，则返回的标识符为 0。
        /// </param>
        /// <param name="nIcons">要从文件中提取的图标数。 此参数仅在从.exe和.dll文件时有效。</param>
        /// <param name="flags">指定控制此函数的标志。 这些标志是 LoadImage 函数使用的 LR_* 标志。</param>
        /// <returns>
        /// 如果 <param name="phicon"> 参数为 NULL 且此函数成功，则返回值为文件中的图标数。 如果函数失败，则返回值为 0。 如果 <param name="phicon"> 参数不是 NULL 且函数成功，则返回值是提取的图标数。 否则，如果未找到文件，则返回值0xFFFFFFFF。
        /// </returns>
        [LibraryImport(User32, EntryPoint = "PrivateExtractIconsW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int PrivateExtractIcons(
            string lpszFile,
            int nIconIndex,
            int cxIcon,
            int cyIcon,
            IntPtr[] phicon,
            int[] piconid,
            int nIcons,
            int flags
        );

        /// <summary>
        /// 注册一个窗口类，以便在对 CreateWindow 或 CreateWindowEx 函数的调用中随后使用。
        /// </summary>
        /// <param name="lpWndClass">指向 WNDCLASS 结构的指针。 在将结构传递给函数之前，必须用相应的类属性填充结构。</param>
        /// <return>如果函数成功，则返回值是唯一标识所注册类的类原子。 如果函数失败，则返回值为零。</return>
        [LibraryImport(User32, EntryPoint = "RegisterClassW", SetLastError = true)]
        public static partial short RegisterClass(IntPtr lpWndClass);

        /// <summary>
        /// 定义保证在整个系统中唯一的新窗口消息。 发送或发布消息时可以使用消息值。
        /// </summary>
        /// <param name="lpString">要注册的消息。</param>
        /// <returns>如果成功注册消息，则返回值是范围0xC000到0xFFFF的消息标识符。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "RegisterWindowMessageW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
        public static partial uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        /// <summary>
        /// 将创建指定窗口的线程引入前台并激活窗口。 键盘输入将定向到窗口，并为用户更改各种视觉提示。 系统为创建前台窗口的线程分配的优先级略高于其他线程的优先级。
        /// </summary>
        /// <param name="hWnd">应激活并带到前台的窗口的句柄。</param>
        /// <returns>如果将窗口带到前台，则返回值为非零值。如果未将窗口带到前台，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "SetForegroundWindow", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 将指定的消息发送到窗口或窗口。 <see cref="SendMessage"> 函数调用指定窗口的窗口过程，在窗口过程处理消息之前不会返回。
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
        [LibraryImport(User32, EntryPoint = "SendMessageW", SetLastError = false)]
        public static partial IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, int wParam, IntPtr lParam);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的32位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定 32 位整数的上一个值。如果函数失败，则返回值为零。 </returns>
        [LibraryImport(User32, EntryPoint = "SetWindowLongW", SetLastError = false)]
        public static partial IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex, WndProc newProc);

        [LibraryImport(User32, EntryPoint = "SetWindowLongW", SetLastError = false)]
        public static partial IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex, WindowStyleEx styleEx);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的64位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定偏移量的上一个值。如果函数失败，则返回值为零。 </returns>
        [LibraryImport(User32, EntryPoint = "SetWindowLongPtrW", SetLastError = false)]
        public static partial IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WndProc newProc);

        [LibraryImport(User32, EntryPoint = "SetWindowLongPtrW", SetLastError = false)]
        public static partial IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WindowStyleEx styleEx);
    }
}
