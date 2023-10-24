using System;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.DwmApi
{
    /// <summary>
    /// DwmGetWindowAttribute 和 DwmSetWindowAttribute 函数使用的选项。
    /// </summary>
    [Flags]
    public enum DWMWINDOWATTRIBUTE : uint
    {
        /// <summary>
        /// 与 DwmGetWindowAttribute 一起使用。 发现是否启用了非客户端呈现。 检索到的值的类型为 BOOL。 如果启用非客户端呈现，则为 TRUE;否则为 FALSE。
        /// </summary>
        DWMWA_NCRENDERING_ENABLED = 1,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 设置非客户端呈现策略。 pvAttribute 参数指向 DWMNCRENDERINGPOLICY 枚举中的值。
        /// </summary>
        DWMWA_NCRENDERING_POLICY = 2,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 启用或强制禁用 DWM 转换。 pvAttribute 参数指向 BOOL 类型的值。 如果为 TRUE ，则禁用转换; 如果为 FALSE ，则启用转换。
        /// </summary>
        DWMWA_TRANSITIONS_FORCEDISABLED = 3,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 使非工作区中呈现的内容在 DWM 绘制的框架上可见。 pvAttribute 参数指向 BOOL 类型的值。 如果为 TRUE ，则使非工作区中呈现的内容在框架上可见;否则为 FALSE。
        /// </summary>
        DWMWA_ALLOW_NCPAINT = 4,

        /// <summary>
        /// 与 DwmGetWindowAttribute 一起使用。 检索窗口相对空间中标题按钮区域的边界。 检索到的值的类型为 RECT。 如果窗口最小化或对用户不可见，则检索到的 RECT 的值是未定义的。 应检查检索到的 RECT 是否包含可以使用的边界，如果它不包含，则可以断定窗口已最小化或不可见。
        /// </summary>
        DWMWA_CAPTION_BUTTON_BOUNDS = 5,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 指定非客户端内容是否为从右到左 (RTL) 镜像。 pvAttribute 参数指向 BOOL 类型的值。 如果非客户端内容从右到左 (RTL) 镜像，则为 TRUE;否则为 FALSE。
        /// </summary>
        DWMWA_NONCLIENT_RTL_LAYOUT = 6,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 强制窗口 (静态位图) 显示图标缩略图或速览表示形式，即使窗口的实时或快照表示形式可用也是如此。 此值通常在创建窗口期间设置，在窗口的整个生存期内不会更改。 但是，某些方案可能需要值随时间推移而更改。 pvAttribute 参数指向 BOOL 类型的值。 如果为 TRUE ，则需要图标缩略图或速览表示形式;否则为 FALSE。
        /// </summary>
        DWMWA_FORCE_ICONIC_REPRESENTATION = 7,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 设置 Flip3D 处理窗口的方式。 pvAttribute 参数指向 DWMFLIP3DWINDOWPOLICY 枚举中的值。.
        /// </summary>
        DWMWA_FLIP3D_POLICY = 8,

        /// <summary>
        /// 与 DwmGetWindowAttribute 一起使用。 检索屏幕空间中的扩展框架边界矩形。 检索到的值的类型为 RECT。
        /// </summary>
        DWMWA_EXTENDED_FRAME_BOUNDS = 9,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 窗口将提供一个位图供 DWM 用作图标缩略图或速览表示形式， (窗口的静态位图) 。 可以使用 DWMWA_FORCE_ICONIC_REPRESENTATION 指定DWMWA_HAS_ICONIC_BITMAP。 DWMWA_HAS_ICONIC_BITMAP 通常在创建窗口期间设置，在窗口的整个生存期内不会更改。 但是，某些方案可能需要值随时间推移而更改。 pvAttribute 参数指向 BOOL 类型的值。 如果为 TRUE ，则告知 DWM 窗口将提供图标缩略图或速览表示形式;否则为 FALSE
        /// Windows Vista 及更早版本： 不支持此值。
        /// </summary>
        DWMWA_HAS_ICONIC_BITMAP = 10,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 不显示窗口的速览预览。 当鼠标悬停在任务栏中的窗口缩略图上时，速览视图将显示窗口的全尺寸预览。 如果设置了此属性，将鼠标指针悬停在窗口缩略图上会消除速览 (以防组中的另一个窗口具有显示) 的速览预览。 pvAttribute 参数指向 BOOL 类型的值。 如果为 TRUE ，则阻止速览功能，如果为 FALSE ，则允许此功能。
        /// Windows Vista 及更早版本： 不支持此值。
        /// </summary>
        DWMWA_DISALLOW_PEEK = 11,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 防止窗口在调用 peek 时褪色到玻璃板。 pvAttribute 参数指向 BOOL 类型的值。 如果为 TRUE ，则防止窗口在另一个窗口的速览期间褪色;如果为 FALSE ，则表示正常行为。
        /// Windows Vista 及更早版本： 不支持此值。
        /// </summary>
        DWMWA_EXCLUDED_FROM_PEEK = 12,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 遮罩窗口，使其对用户不可见。 窗口仍由 DWM 组成。
        /// 将 与 DirectComposition 配合使用： 当通过与分层子窗口关联的 DirectComposition 视觉对象对窗口内容的表示形式进行动画处理时，请使用 DWMWA_CLOAK 标志遮蔽分层子窗口。 有关此用例的更多详细信息，请参阅 如何对分层子窗口的位图进行动画处理。
        /// Windows 7 及更早版本： 不支持此值。
        /// </summary>
        DWMWA_CLOAK = 13,

        /// <summary>
        /// 与 DwmGetWindowAttribute 一起使用。 如果窗口是隐藏的， 请提供以下值之一来解释原因。
        /// DWM_CLOAKED_APP (值0x0000001) 。 窗口已被其所有者应用程序遮蔽。
        /// DWM_CLOAKED_SHELL (值0x0000002) 。 窗户被壳牌遮蔽了。
        /// DWM_CLOAKED_INHERITED (值0x0000004) 。 隐藏值继承自其所有者窗口。
        /// Windows 7 及更早版本： 不支持此值。
        /// </summary>
        DWMWA_CLOAKED = 14,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 使用当前视觉对象冻结窗口的缩略图。 不要对缩略图进行进一步的实时更新，以匹配窗口的内容。
        /// Windows 7 及更早版本： 不支持此值。
        /// </summary>
        DWMWA_FREEZE_REPRESENTATION = 15,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 启用深色模式系统设置时，允许以深色模式颜色绘制此窗口的窗口框架。 出于兼容性原因，无论系统设置如何，所有窗口都默认为浅色模式。 pvAttribute 参数指向 BOOL 类型的值。 TRUE 表示窗口采用深色模式， FALSE 表示始终使用浅色模式。
        /// </summary>
        DWMWA_PASSIVE_UPDATE_MODE = 16,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 允许非 UWP 窗口使用主机背景画笔。 如果设置了此标志，则调用 Windows：：UI：：Composition API 的 Win32 应用可以使用主机背景画笔生成透明度效果 (请参阅 Compositor.CreateHostBackdropBrush) 。 pvAttribute 参数指向 BOOL 类型的值。 如果为 TRUE ，则为窗口启用主机背景画笔，如果为 FALSE ，则禁用它。
        /// 从 Windows 11 版本 22000 开始支持此值。
        /// </summary>
        DWMWA_USE_HOSTBACKDROPBRUSH = 17,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 启用深色模式系统设置时，允许以深色模式颜色绘制此窗口的窗口框架。 出于兼容性原因，无论系统设置如何，所有窗口都默认为浅色模式。 pvAttribute 参数指向 BOOL 类型的值。 TRUE 表示窗口采用深色模式， FALSE 表示始终使用浅色模式。
        /// </summary>
        DMWA_USE_IMMERSIVE_DARK_MODE_OLD = 19,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 启用深色模式系统设置时，允许以深色模式颜色绘制此窗口的窗口框架。 出于兼容性原因，无论系统设置如何，所有窗口都默认为浅色模式。 pvAttribute 参数指向 BOOL 类型的值。 TRUE 表示窗口采用深色模式， FALSE 表示始终使用浅色模式。
        /// 从 Windows 11 版本 22000 开始支持此值。
        /// </summary>
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 指定窗口的圆角首选项。 pvAttribute 参数指向 DWM_WINDOW_CORNER_PREFERENCE 类型的值。
        /// 从 Windows 11 版本 22000 开始支持此值。
        /// </summary>
        DWMWA_WINDOW_CORNER_PREFERENCE = 33,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 指定窗口边框的颜色。 pvAttribute 参数指向 COLORREF 类型的值。 应用负责根据状态更改（例如窗口激活中的更改）更改边框颜色。
        /// 从 Windows 11 内部版本 22000 开始支持此值。
        /// </summary>
        DWMWA_BORDER_COLOR = 34,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 指定标题的颜色。 pvAttribute 参数指向 COLORREF 类型的值。
        /// 从 Windows 11 内部版本 22000 开始支持此值。
        /// </summary>
        DWMWA_CAPTION_COLOR = 35,

        /// <summary>
        /// 与 DwmSetWindowAttribute 一起使用。 指定标题文本的颜色。 pvAttribute 参数指向 COLORREF 类型的值。
        /// 从 Windows 11 内部版本 22000 开始支持此值。
        /// </summary>
        DWMWA_TEXT_COLOR = 36,

        /// <summary>
        /// 与 DwmGetWindowAttribute 一起使用。 检索 DWM 将围绕此窗口绘制的外部边框的宽度。 该值可能因窗口的 DPI 而异。 pvAttribute 参数指向 UINT 类型的值。
        /// 从 Windows 11 内部版本 22000 开始支持此值。
        /// </summary>
        DWMWA_VISIBLE_FRAME_BORDER_THICKNESS = 37,

        /// <summary>
        /// 与 DwmGetWindowAttribute 或 DwmSetWindowAttribute 一起使用。 检索或指定窗口的系统绘制背景材料，包括在非工作区后面。 pvAttribute 参数指向 DWM_SYSTEMBACKDROP_TYPE 类型的值。
        /// 从 Windows 11 内部版本 22621 开始支持此值。
        /// </summary>
        DWMWA_SYSTEMBACKDROP_TYPE = 38,

        /// <summary>
        /// 指示窗口是否应使用云母效果。
        /// 从 Windows 11 内部版本 22621 开始支持此值。
        /// </summary>
        DWMWA_MICA_EFFECT = 1029
    }
}
