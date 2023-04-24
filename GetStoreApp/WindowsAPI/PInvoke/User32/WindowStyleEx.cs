using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 扩展窗口样式。
    /// </summary>
    [Flags]
    public enum WindowStyleEx : uint
    {
        /// <summary>
        /// 窗口接受拖放文件。
        /// </summary>
        WS_EX_ACCEPTFILES = 0x00000010,

        /// <summary>
        /// 当窗口可见时，将顶级窗口强制到任务栏上。
        /// </summary>
        WS_EX_APPWINDOW = 0x00040000,

        /// <summary>
        /// 窗口有一个边框，带有沉没边缘。
        /// </summary>
        WS_EX_CLIENTEDGE = 0x00000200,

        /// <summary>
        /// 使用双缓冲按从下到上绘制顺序绘制窗口的所有后代。 从下到上绘制顺序允许后代窗口具有半透明 (alpha) 和透明度 (颜色键) 效果，但前提是后代窗口还设置了 <see cref="WS_EX_TRANSPARENT"> 位。 双缓冲允许不闪烁地绘制窗口及其后代。 如果窗口的 类样式 为 CS_OWNDC 或 CS_CLASSDC，则不能使用此样式。
        /// Windows 2000：不支持此样式。
        /// </remarks>
        WS_EX_COMPOSITED = 0x02000000,

        /// <summary>
        /// 窗口的标题栏包含问号。 当用户单击问号时，光标将更改为带有指针的问号。 如果用户单击子窗口，子窗口将收到 <see cref="WindowMessage.WM_HELP"> 消息。 子窗口应将消息传递给父窗口过程，该过程应使用 HELP_WM_HELP 命令调用 WinHelp 函数。 帮助应用程序会显示一个弹出窗口，该窗口通常包含子窗口的帮助。
        /// <see cref="WS_EX_CONTEXTHELP"> 不能与 <see cref="WindowStyles.WS_MAXIMIZEBOX"> 或 <see cref="WindowStyles.WS_MINIMIZEBOX"> 样式一起使用
        /// </summary>
        WS_EX_CONTEXTHELP = 0x00000400,

        /// <summary>
        /// 窗口本身包含应参与对话框导航的子窗口。 如果指定了此样式，则执行导航操作（例如处理 TAB 键、箭头键或键盘助记键）时，对话框管理器将递归到此窗口的子级。
        /// </summary>
        WS_EX_CONTROLPARENT = 0x00010000,

        /// <summary>
        /// 窗口具有双边框;可以选择使用标题栏创建窗口，方法是在 dwStyle 参数中指定 <see cref="WindowStyles.WS_CAPTION"> 样式。
        /// </summary>
        WS_EX_DLGMODALFRAME = 0x00000001,

        /// <summary>
        /// 窗口是分层 窗口。 如果窗口的 类样式 为 CS_OWNDC 或 CS_CLASSDC，则不能使用此样式。
        /// Windows 8：顶级窗口和子窗口支持 <see cref="WS_EX_LAYERED"> 样式。 以前的Windows版本仅支持顶级窗口 <see cref="WS_EX_LAYERED">。
        /// </summary>
        WS_EX_LAYERED = 0x00080000,

        /// <summary>
        /// 如果 shell 语言是希伯来语、阿拉伯语或支持阅读顺序对齐的另一种语言，则窗口的水平原点位于右边缘。 将水平值增大到左侧。
        /// </summary>
        WS_EX_LAYOUTRTL = 0x00400000,

        /// <summary>
        /// 该窗口具有泛型左对齐属性。 这是默认值。
        /// </summary>
        WS_EX_LEFT = 0x00000000,

        /// <summary>
        /// 如果 shell 语言是希伯来语、阿拉伯语或支持阅读顺序对齐的另一种语言，则垂直滚动条 (如果存在) 位于工作区左侧。 对于其他语言，将忽略该样式。
        /// </summary>
        WS_EX_LEFTSCROLLBAR = 0x00004000,

        /// <summary>
        /// 窗口文本使用从左到右的阅读顺序属性显示。 这是默认值。
        /// </summary>
        WS_EX_LTRREADING = 0x00000000,

        /// <summary>
        /// 窗口是 MDI 子窗口。
        /// </summary>
        WS_EX_MDICHILD = 0x00000040,

        /// <summary>
        /// 	当用户单击该样式时，使用此样式创建的顶级窗口不会成为前台窗口。 当用户最小化或关闭前台窗口时，系统不会将此窗口引入前台。
        /// 不应通过编程访问或通过辅助技术（如讲述人）通过键盘导航激活窗口。
        /// 若要激活窗口，请使用 SetActiveWindow 或 <see cref="User32Library.SetForegroundWindow"> 函数。
        /// 默认情况下，该窗口不会显示在任务栏上。 若要强制窗口显示在任务栏上，请使用 <see cref="WS_EX_APPWINDOW"> 样式。
        /// </summary>
        WS_EX_NOACTIVATE = 0x08000000,

        /// <summary>
        /// 该窗口不将其窗口布局传递给其子窗口。
        /// </summary>
        WS_EX_NOINHERITLAYOUT = 0x00100000,

        /// <summary>
        /// 使用此样式创建的子窗口不会在创建或销毁时将 <see cref="WindowMessage.WM_PARENTNOTIFY"> 消息发送到其父窗口。
        /// </summary>
        WS_EX_NOPARENTNOTIFY = 0x00000004,

        /// <summary>
        /// 窗口不会呈现到重定向图面。 这是对于没有可见内容的窗口，或者使用表面以外的机制来提供视觉对象。
        /// </summary>
        WS_EX_NOREDIRECTIONBITMAP = 0x00200000,

        /// <summary>
        /// 窗口是重叠的窗口。
        /// </summary>
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

        /// <summary>
        /// 窗口是调色板窗口，它是一个无模式对话框，用于显示命令数组。
        /// </summary>
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

        /// <summary>
        /// 该窗口具有泛型“右对齐”属性。 这依赖于窗口类。 仅当 shell 语言为希伯来语、阿拉伯语或支持阅读顺序对齐的另一种语言时，此样式才有效;否则，将忽略样式。
        /// 对静态控件或编辑控件使用 <see cref="WS_EX_RIGHT"> 样式的效果与分别使用 SS_RIGHT 或 ES_RIGHT 样式的效果相同。 将此样式与按钮控件结合使用的效果与使用 BS_RIGHT 和 BS_RIGHTBUTTON 样式的效果相同。
        /// </summary>
        WS_EX_RIGHT = 0x00001000,

        /// <summary>
        /// 如果存在) 位于工作区右侧，则垂直滚动条 (。 这是默认值。
        /// </summary>
        WS_EX_RIGHTSCROLLBAR = 0x00000000,

        /// <summary>
        /// 如果 shell 语言是希伯来语、阿拉伯语或支持阅读顺序对齐的另一种语言，则使用从右到左的阅读顺序属性显示窗口文本。 对于其他语言，将忽略该样式。
        /// </summary>
        WS_EX_RTLREADING = 0x00002000,

        /// <summary>
        /// 该窗口具有一个三维边框样式，用于不接受用户输入的项目。
        /// </summary>
        WS_EX_STATICEDGE = 0x00020000,

        /// <summary>
        /// 该窗口旨在用作浮动工具栏。 工具窗口具有短于普通标题栏的标题栏和使用较小的字体绘制的窗口标题。 工具窗口不会显示在任务栏中，也不会显示在用户按下 Alt+TAB 时出现的对话框中。 如果工具窗口有系统菜单，则其图标不会显示在标题栏上。 但是，可以通过右键单击或键入 Alt+SPACE 来显示系统菜单。
        /// </summary>
        WS_EX_TOOLWINDOW = 0x00000080,

        /// <summary>
        /// 该窗口应放置在所有非最顶层窗口上方，并且应保持其上方，即使窗口已停用也是如此。 若要添加或删除此样式，请使用 SetWindowPos 函数。
        /// </summary>
        WS_EX_TOPMOST = 0x00000008,

        /// <summary>
        /// 在绘制同一线程) 创建的窗口下方的同级 (之前，不应绘制窗口。 窗口显示为透明，因为已绘制基础同级窗口的位。
        /// 若要在不使用这些限制的情况下实现透明度，请使用 SetWindowRgn 函数。
        /// </summary>
        WS_EX_TRANSPARENT = 0x00000020,

        /// <summary>
        /// 窗口具有带有凸起边缘的边框。
        /// </summary>
        WS_EX_WINDOWEDGE = 0x00000100,
    }
}
