using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 指定创建时进程的主窗口的窗口工作站、桌面、标准句柄和外观。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe partial struct STARTUPINFO
    {
        /// <summary>
        /// 结构大小（以字节为单位）。
        /// </summary>
        public int cb;

        /// <summary>
        /// 保留;必须为 NULL。
        /// </summary>
        public char* lpReserved;

        /// <summary>
        /// 桌面的名称，或此过程的桌面和窗口工作站的名称。 字符串中的反斜杠指示字符串包括桌面和窗口工作站名称。有关详细信息，请参阅 与桌面的线程连接。
        /// </summary>
        public char* lpDesktop;

        /// <summary>
        /// 对于控制台进程，如果创建新的控制台窗口，则这是标题栏中显示的标题。 如果为 NULL，则可执行文件的名称将改为用作窗口标题。 对于不创建新控制台窗口的 GUI 或控制台进程，此参数必须为 NULL。
        /// </summary>
        public char* lpTitle;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USEPOSITION">，则如果创建新窗口（以像素为单位），则此成员是窗口左上角的 x 偏移量。 否则，将忽略此成员。
        /// 偏移量来自屏幕左上角。 对于 GUI 进程，新进程首次调用 <see cref="User32Library.CreateWindowEx"> 以创建重叠窗口（如果 <see cref="User32Library.CreateWindowEx"> 的 x 参数CW_USEDEFAULT）时，将使用指定的位置。
        /// </summary>
        public int dwX;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USEPOSITION">，则如果创建新窗口（以像素为单位），则此成员是窗口左上角的 y 偏移量。 否则，将忽略此成员。
        /// 偏移量来自屏幕左上角。 对于 GUI 进程，新进程首次调用 <see cref="User32Library.CreateWindowEx"> 以创建重叠窗口（如果 <see cref="User32Library.CreateWindowEx"> 的 y 参数CW_USEDEFAULT）时，将使用指定的位置。
        /// </summary>
        public int dwY;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USESIZE">，则如果创建新窗口（以像素为单位），则此成员是窗口的宽度。 否则，将忽略此成员。
        /// 对于 GUI 进程，这仅在新进程调用 <see cref="User32Library.CreateWindowEx"> 时首次调用 CreateWindow 以创建重叠窗口（如果 <see cref="User32Library.CreateWindowEx"> 的 nWidth 参数CW_USEDEFAULT）。
        /// </summary>
        public int dwXSize;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USESIZE">，则如果创建新窗口（以像素为单位），则此成员是窗口的高度。 否则，将忽略此成员。
        /// 对于 GUI 进程，仅当新进程调用 <see cref="User32Library.CreateWindowEx"> 以创建重叠窗口（如果 <see cref="User32Library.CreateWindowEx"> 的 nHeight 参数CW_USEDEFAULT）时，才使用此方法。
        /// </summary>
        public int dwYSize;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USECOUNTCHARS">，如果在控制台进程中创建了一个新的控制台窗口，则此成员以字符列指定屏幕缓冲区宽度。 否则，将忽略此成员。
        /// </summary>
        public int dwXCountChars;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USECOUNTCHARS">，如果在控制台进程中创建了一个新的控制台窗口，则此成员在字符行中指定屏幕缓冲区高度。 否则，将忽略此成员。
        /// </summary>
        public int dwYCountChars;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USEFILLATTRIBUTE">，则如果控制台应用程序中创建了新的控制台窗口，则此成员是初始文本和背景色。 否则，将忽略此成员。
        /// 此值可以是以下值的任意组合：FOREGROUND_BLUE、FOREGROUND_GREEN、FOREGROUND_RED、FOREGROUND_INTENSITY、BACKGROUND_BLUE、BACKGROUND_GREEN、BACKGROUND_RED和BACKGROUND_INTENSITY。 例如，以下值组合在白色背景上生成红色文本：
        /// FOREGROUND_RED| BACKGROUND_RED| BACKGROUND_GREEN| BACKGROUND_BLUE
        /// </summary>
        public uint dwFillAttribute;

        /// <summary>
        /// 一个位字段，用于确定进程创建窗口时是否使用某些 <see cref="STARTUPINFO"> 成员。 此成员可以是以下一个或多个值。
        /// </summary>
        public STARTF dwFlags;

        /// <summary>
        /// If <see cref="dwFlags"/> specifies <see cref="STARTF.STARTF_USESHOWWINDOW"/>, 则此成员可以是在 <see cref="User32Library.ShowWindow"> 函数的 nCmdShow 参数中指定的任何值，但 <see cref="WindowShowStyle.SW_SHOWDEFAULT"> 除外。 否则，将忽略此成员。
        /// 对于 GUI 进程，首次调用 <see cref="User32Library.ShowWindow"> 时，将忽略其 nCmdShow 参数 wShowWindow 指定默认值。 在对 <see cref="User32Library.ShowWindow"> 的后续调用中，如果将 <see cref="User32Library.ShowWindow"> 的 nCmdShow 参数设置为<see cref="WindowShowStyle.SW_SHOWDEFAULT">，将使用 wShowWindow 成员。
        /// </summary>
        public WindowShowStyle wShowWindow;

        /// <summary>
        /// 保留供 C 运行时使用;必须为零。
        /// </summary>
        public ushort cbReserved2;

        /// <summary>
        /// 保留供 C 运行时使用;必须为 NULL。
        /// </summary>
        public IntPtr lpReserved2;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USESTDHANDLES"/>, 则此成员是进程的标准输入句柄。 如果未指定 <see cref="STARTF.STARTF_USESTDHANDLES"/>，则标准输入的默认值为键盘缓冲区
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USEHOTKEY"/>, 则此成员指定一个热键值，该值作为 <see cref="WindowMessage.WM_SETHOTKEY"> 消息的 wParam 参数发送到拥有该过程的应用程序创建的第一个符合条件的顶级窗口。如果使用 WindowStyle.WS_POPUP 窗口样式创建窗口，则它不符合条件，除非还设置了 WindowStyleEx.WS_EX_APPWINDOW 扩展窗口样式。 有关详细信息，请参阅 <see cref="User32Library.CreateWindowEx">。
        /// 否则，将忽略此成员。
        /// </summary>
        public IntPtr hStdInput;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USESTDHANDLES"/>, 则此成员是进程的标准输出句柄。 否则，将忽略此成员，标准输出的默认值为控制台窗口的缓冲区。
        /// 如果进程从任务栏或跳转列表启动，系统将 <see cref="hStdOutput"/> 设置为包含用于启动进程的任务栏或跳转列表的监视器的句柄。 有关详细信息，请参阅“备注”。
        /// Windows 7、Windows Server 2008 R2、Windows Vista、Windows Server 2008、Windows XP 和 Windows Server 2003： 此行为是在Windows 8和Windows Server 2012中引入的。
        /// </summary>
        public IntPtr hStdOutput;

        /// <summary>
        /// 如果 <see cref="dwFlags"/> 指定 <see cref="STARTF.STARTF_USESTDHANDLES"/>, 则此成员是进程的标准错误句柄。 否则，将忽略此成员，标准错误的默认值为控制台窗口的缓冲区。
        /// </summary>
        public IntPtr hStdError;
    }
}
