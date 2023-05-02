using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含 RegisterClass 函数注册的窗口类属性。
    /// 此结构已被用于 RegisterClassEx 函数的 WNDCLASSEX 结构取代。 如果不需要设置与窗口类关联的小图标，仍然可以使用 WNDCLASS 和 RegisterClass 。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WNDCLASS
    {
        /// <summary>
        /// 类样式。此成员可以是 类样式的任意组合。
        /// </summary>
        public uint style;

        /// <summary>
        /// 指向窗口过程的指针。 必须使用 <see cref="User32Library.CallWindowProc"> 函数调用窗口过程。 有关详细信息，请参阅 WndProc。
        /// </summary>
        public WndProc lpfnWndProc;

        /// <summary>
        /// 要按照窗口类结构分配的额外字节数。 系统将字节初始化为零。
        /// </summary>
        public int cbClsExtra;

        /// <summary>
        /// 在窗口实例之后分配的额外字节数。 系统将字节初始化为零。 如果应用程序使用 WNDCLASS 注册通过使用资源文件中 的 CLASS 指令创建的对话框，则必须将此成员设置为 DLGWINDOWEXTRA。
        /// </summary>
        public int cbWndExtra;

        /// <summary>
        /// 包含类的窗口过程的实例的句柄。
        /// </summary>
        public IntPtr hInstance;

        /// <summary>
        /// 类图标的句柄。 此成员必须是图标资源的句柄。 如果此成员为 NULL，则系统提供默认图标。
        /// </summary>
        public IntPtr hIcon;

        /// <summary>
        /// 类游标的句柄。 此成员必须是游标资源的句柄。 如果此成员为 NULL，则每当鼠标移动到应用程序的窗口中时，应用程序都必须显式设置光标形状。
        /// </summary>
        public IntPtr hCursor;

        /// <summary>
        /// 类游标的句柄。 此成员必须是游标资源的句柄。 如果此成员为 NULL，则每当鼠标移动到应用程序的窗口中时，应用程序都必须显式设置光标形状。
        /// </summary>
        public IntPtr hbrBackground;

        /// <summary>
        /// 类菜单的资源名称，因为名称显示在资源文件中。 如果使用整数标识菜单，请使用 MAKEINTRESOURCE 宏。 如果此成员为 NULL，则属于此类的窗口没有默认菜单。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr, SizeConst = 128)]
        public string lpszMenuName;

        /// <summary>
        /// 指向空终止字符串或原子的指针。 如果此参数是 atom，则它必须是由上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的类原子。 atom 必须位于 lpszClassName 的低序单词中;高序单词必须为零。
        /// 如果 lpszClassName 是字符串，则指定窗口类名称。 类名称可以是注册到 RegisterClass 或 RegisterClassEx 的任何名称，也可以是预定义的控件类名称。
        /// lpszClassName 的最大长度为 256。 如果 lpszClassName 大于最大长度， 则 RegisterClass 函数将失败。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr, SizeConst = 256)]
        public string lpszClassName;
    }
}
