using System;
using System.Runtime.InteropServices;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// <see cref="WindowClass">结构 - 表示单个窗口。用于接收窗口消息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClass
    {
        /// <summary>
        /// 类样式
        /// </summary>
        public uint style;

        /// <summary>
        /// 指向窗口过程的指针。 必须使用 <see cref="User32Library.CallWindowProc"> 函数调用窗口过程。
        /// </summary>
        public WindowProcedureHandler lpfnWndProc;

        /// <summary>
        /// 要按照窗口类结构分配的额外字节数。 系统将字节初始化为零。
        /// </summary>
        public int cbClsExtra;

        /// <summary>
        /// 在窗口实例之后分配的额外字节数。 系统将字节初始化为零。 如果应用程序使用 WNDCLASSEX 在资源文件中使用 CLASS 指令创建对话框，
        /// 则必须将此成员设置为 DLGWINDOWEXTRA。
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
        /// 指向一个 null 终止的字符串的指针，该字符串指定类菜单的资源名称，因为名称显示在资源文件中。
        /// 如果使用整数标识菜单，请使用 MAKEINTRESOURCE 宏。 如果此成员为 NULL，则属于此类的窗口没有默认菜单。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;

        /// <summary>
        /// 指向空终止字符串或原子的指针。 如果此参数是 atom，则它必须是由上一次对 <see cref="User32Library.RegisterClass"> 或 RegisterClassEx 函数的调用创建的类原子。
        /// atom 必须位于 <see cref="lpszClassName"> 的低序单词中;高序单词必须为零。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }
}
