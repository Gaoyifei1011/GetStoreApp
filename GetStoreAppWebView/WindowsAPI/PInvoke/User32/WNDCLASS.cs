using System;
using System.Runtime.InteropServices;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含由 RegisterClass 函数注册的窗口类属性。
    /// 此结构已被与 RegisterClassEx 函数一起使用的 WNDCLASSEX结构所 取代。 如果不需要设置与窗口类关联的小图标，仍然可以使用 WNDCLASS 和 RegisterClass 。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WNDCLASS
    {
        /// <summary>
        /// 窗口类样式。此成员可以是类样式的任意组合。
        /// </summary>
        public uint style;

        /// <summary>
        /// 指向窗口过程的指针。 必须使用 CallWindowProc 函数调用窗口过程。 有关详细信息，请参阅 WindowProc。
        /// </summary>
        public IntPtr lpfnWndProc;

        /// <summary>
        /// 要根据窗口类结构分配的额外字节数。 系统将字节初始化为零。
        /// </summary>
        public int cbClsExtra;

        /// <summary>
        /// 在窗口实例之后分配的额外字节数。 系统将字节初始化为零。 如果应用程序使用 WNDCLASS 注册使用资源文件中的 CLASS 指令创建的对话框，则必须将此成员设置为 DLGWINDOWEXTRA。
        /// </summary>
        public int cbWndExtra;

        /// <summary>
        /// 实例的句柄，该实例包含类的窗口过程。
        /// </summary>
        public IntPtr hInstance;

        /// <summary>
        /// 类图标的句柄。 此成员必须是图标资源的句柄。 如果此成员为 NULL，则系统会提供默认图标。
        /// </summary>
        public IntPtr hIcon;

        /// <summary>
        /// 类游标的句柄。 此成员必须是游标资源的句柄。 如果此成员为 NULL，则每当鼠标移动到应用程序的窗口中时，应用程序都必须显式设置光标形状。
        /// </summary>
        public IntPtr hCursor;

        /// <summary>
        /// 类背景画笔的句柄。 此成员可以是用于绘制背景的物理画笔的句柄，也可以是颜色值。 颜色值必须是以下标准系统颜色之一， (必须将值 1 添加到所选颜色) 。
        /// 使用 UnregisterClass 取消注册类时，系统会自动删除类背景画笔。 应用程序不应删除这些画笔。
        /// 如果此成员为 NULL，则每当请求应用程序在其工作区中绘制时，都必须绘制其自己的背景。 若要确定是否必须绘制背景，应用程序可以处理WM_ERASEBKGND消息或测试 BeginPaint 函数填充的 PAINTSTRUCT 结构的 fErase 成员。
        /// </summary>
        public IntPtr hbrBackground;

        /// <summary>
        /// 类菜单的资源名称，该名称显示在资源文件中。 如果使用整数来标识菜单，请使用 MAKEINTRESOURCE 宏。 如果此成员为 NULL，则属于此类的窗口没有默认菜单。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;

        /// <summary>
        /// 指向以 null 结尾的字符串的指针或 是原子。 如果此参数是 atom，则它必须是先前调用 RegisterClass 或 RegisterClassEx 函数创建的类原子。 原子必须位于 lpszClassName 的低序字中;高序字必须为零。
        /// 如果 lpszClassName 是字符串，则指定窗口类名。 类名可以是使用 RegisterClass 或 RegisterClassEx 注册的任何名称，也可以是任何预定义的控件类名称。
        /// lpszClassName 的最大长度为 256。 如果 lpszClassName 大于最大长度， 则 RegisterClass 函数将失败。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }
}
