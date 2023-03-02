using System;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 主窗口风格。只要需要窗口样式，就可以指定以下样式。
    /// 创建控件后，不能修改这些样式，除非有说明。
    /// </summary>
    [Flags]
    public enum WindowStyles : uint
    {
        /// <summary>
        /// 窗口具有细线边框
        /// </summary>
        WS_BORDER = 0x800000,

        /// <summary>
        /// 窗口具有标题栏 (包括 <see cref="WS_BORDER"> 样式) 。
        /// </summary>
        WS_CAPTION = 0xc00000,

        /// <summary>
        /// 窗口是子窗口。 具有此样式的窗口不能有菜单栏。
        /// 此样式不能与 <see cref="WS_POPUP"> 样式一 起使用。
        /// </summary>
        WS_CHILD = 0x40000000,

        /// <summary>
        /// 与 <see cref="WS_CHILD"> 样式相同。
        /// </summary>
        WS_CHILDWINDOW = 0x40000000,

        /// <summary>
        /// 在父窗口中绘制时，排除子窗口占用的区域。 创建父窗口时会使用此样式。
        /// </summary>
        WS_CLIPCHILDREN = 0x2000000,

        /// <summary>
        /// 将子窗口相对于彼此剪裁;也就是说，当特定子窗口收到 <see cref="WindowMessage.WM_PAINT"> 消息时， <see cref="WS_CLIPSIBLINGS"> 样式会将所有其他重叠子窗口剪辑到要更新的子窗口区域。 如果未指定 <see cref="WS_CLIPSIBLINGS"> 并且子窗口重叠，则当在子窗口的工作区内绘制时，可以在相邻子窗口的工作区内绘制。
        /// </summary>
        WS_CLIPSIBLINGS = 0x4000000,

        /// <summary>
        /// 窗口最初处于禁用状态。 禁用的窗口无法从用户接收输入。 若要在创建窗口后进行更改，请使用 EnableWindow 函数。
        /// </summary>
        WS_DISABLED = 0x8000000,

        /// <summary>
        /// 窗口具有通常与对话框一起使用的样式边框。 具有此样式的窗口不能有标题栏。
        /// </summary>
        WS_DLGFRAME = 0x400000,

        /// <summary>
        /// 窗口是一组控件的第一个控件。 该组包含此第一个控件及其之后定义的所有控件，最多包含 <see cref="WS_GROUP"> 样式的下一个控件。 每个组中的第一个控件通常具有 <see cref="WS_TABSTOP"> 样式，以便用户可以从组移动到组。 用户随后可以使用方向键将组中的一个控件中的键盘焦点更改为组中的下一个控件。可以打开和关闭此样式以更改对话框导航。 若要在创建窗口后更改此样式，请使用 SetWindowLong 函数。
        /// </summary>
        WS_GROUP = 0x20000,

        /// <summary>
        /// 窗口具有水平滚动条。
        /// </summary>
        WS_HSCROLL = 0x100000,

        /// <summary>
        /// 窗口最初最小化。 与 <see cref="WS_MINIMIZE"> 样式相同。
        /// </summary>
        WS_ICONIC = 0x20000000,

        /// <summary>
        /// 窗口最初最大化。
        /// </summary>
        WS_MAXIMIZE = 0x1000000,

        /// <summary>
        /// 窗口具有最大化按钮。 不能与 <see cref="WindowStylesEx.WS_EX_CONTEXTHELP"> 样式组合。 还必须指定 <see cref="WS_SYSMENU"> 样式。
        /// </summary>
        WS_MAXIMIZEBOX = 0x10000,

        /// <summary>
        /// 窗口最初最小化。 与 <see cref="WS_ICONIC"> 样式相同。
        /// </summary>
        WS_MINIMIZE = 0x20000000,

        /// <summary>
        /// 窗口具有最小化按钮。 不能与 <see cref="WindowStylesEx.WS_EX_CONTEXTHELP"> 样式组合。 还必须指定 <see cref="WS_SYSMENU"> 样式。
        /// </summary>
        WS_MINIMIZEBOX = 0x20000,

        /// <summary>
        /// 窗口是重叠的窗口。
        /// </summary>
        WS_OVERLAPPED = 0x0,

        /// <summary>
        /// 窗口是重叠的窗口。 
        /// </summary>
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

        /// <summary>
        /// 窗口是弹出窗口。 此样式不能与 <see cref="WS_CHILD"> 样式一 起使用。
        /// </summary>
        WS_POPUP = 0x80000000u,

        /// <summary>
        /// 窗口是弹出窗口。 必须组合 <see cref="WS_CAPTION"> 和 <see cref="WS_POPUPWINDOW"> 样式以使窗口菜单可见。
        /// </summary>
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

        /// <summary>
        /// 窗口具有大小调整边框。
        /// </summary>
        WS_SIZEFRAME = 0x40000,

        /// <summary>
        /// 窗口的标题栏上有一个窗口菜单。 还必须指定 <see cref="WS_CAPTION"> 样式。
        /// </summary>
        WS_SYSMENU = 0x80000,

        /// <summary>
        /// 窗口是一个控件，当用户按下 TAB 键时，可以接收键盘焦点。 按 Tab 键会将键盘焦点更改为 具有 <see cref="WS_TABSTOP"> 样式的下一个控件。可以打开和关闭此样式以更改对话框导航。 若要在创建窗口后更改此样式，请使用 SetWindowLong 函数。 若要使用户创建的窗口和无模式对话框使用制表位，请更改消息循环以调用 IsDialogMessage 函数。
        /// </summary>
        WS_TABSTOP = 0x10000,

        /// <summary>
        /// 窗口具有大小调整边框。
        /// </summary>
        WS_THICKFRAME = 0x00040000,

        /// <summary>
        /// 窗口是重叠的窗口。 重叠窗口具有标题栏和边框。
        /// </summary>
        WS_TILED = 0x00000000,

        /// <summary>
        /// 窗口最初可见。 可以使用 <see cref="User32Library.ShowWindow"> 或 SetWindowPos 函数打开和关闭此样式。
        /// </summary>
        WS_VISIBLE = 0x10000000,

        /// <summary>
        /// 该窗口具有垂直滚动条。
        /// </summary>
        WS_VSCROLL = 0x200000,
    }
}
