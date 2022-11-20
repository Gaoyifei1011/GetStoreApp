using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 定义由调用 Messagebox 函数显示的标准消息框的外观和行为的标志
    /// </summary>
    [Flags]
    public enum MessageBoxOptions : uint
    {
        /// Group1
        /// 若要指示消息框中显示的按钮，请指定以下值之一。

        /// <summary>
        /// 消息框包含一个按钮： 确定。 这是默认值。
        /// </summary>
        MB_OK = 0x000000,

        /// <summary>
        /// 消息框包含两个推送按钮： “确定 ”和 “取消”。
        /// </summary>
        MB_OKCANCEL = 0x000001,

        /// <summary>
        /// 消息框包含三个推送按钮： 中止、 重试和 忽略。
        /// </summary>
        MB_ABORTRETRYIGNORE = 0x000002,

        /// <summary>
        /// 消息框包含三个按钮： 是、 否和 取消。
        /// </summary>
        MB_YESNOCANCEL = 0x000003,

        /// <summary>
        /// 消息框包含两个按下按钮： 是 和 否。
        /// </summary>
        MB_YESNO = 0x000004,

        /// <summary>
        /// 消息框包含两个按下按钮： 重试 和 取消。
        /// </summary>
        MB_RETRYCANCEL = 0x000005,

        /// <summary>
        /// 消息框包含三个按下按钮： “取消”、“ 重试”、“ 继续”。 使用此消息框类型，而不是MB_ABORTRETRYIGNORE。
        /// </summary>
        MB_CANCELTRYCONTINUE = 0x000006,

        /// <summary>
        /// 向消息框添加 “帮助 ”按钮。 当用户单击 “帮助 ”按钮或按 F1 时，系统会向所有者发送 WM_HELP 消息。
        /// </summary>
        MB_HELP = 0x00004000,

        /// Group2
        /// 若要在消息框中显示图标，请指定以下值之一。

        /// <summary>
        /// 消息框中会显示一个停止符号图标。
        /// </summary>
        MB_ICONSTOP = 0x000010,

        /// <summary>
        /// 消息框中会显示一个停止符号图标。
        /// </summary>
        MB_ICONERROR = 0x00000010,

        /// <summary>
        /// 消息框中会显示一个停止符号图标。
        /// </summary>
        MB_ICONHAND = 0x00000010,

        /// <summary>
        /// 消息框中会显示一个问号图标。 不再建议使用问号消息图标，因为这种图标无法清楚地表示特定类型的消息，
        /// 并且作为问题的消息表述可应用于任何消息类型。 此外，用户可能会将问号消息符号与帮助信息混淆。
        /// 因此，不要在消息框中使用问号消息符号。 系统继续支持它包含的内容，只为满足反向兼容性。
        /// <summary>
        MB_ICONQUESTION = 0x000020,

        /// <summary>
        /// 消息框中会显示一个感叹号图标。
        /// </summary>
        MB_ICONWARNING = 0x000030,

        /// <summary>
        /// 消息框中会显示一个感叹号图标。
        /// </summary>
        MB_ICONEXCLAMATION = 0x000030,

        /// <summary>
        /// 图标由圆圈中的小写字母 i 组成，显示在消息框中。
        /// </summary>
        MB_ICONASTERISK = 0x000040,

        /// <summary>
        /// 图标由圆圈中的小写字母 i 组成，显示在消息框中。
        /// </summary>
        MB_ICONINFORMATION = 0x00000040,

        /// <summary>
        /// 用户自定义的图标
        /// </summary>
        MB_USERICON = 0x000080,

        /// Group3
        /// 若要指示默认按钮，请指定以下值之一。

        /// <summary>
        /// 第一个按钮是默认按钮。
        /// 除非指定了MB_DEFBUTTON2、MB_DEFBUTTON3或MB_DEFBUTTON4，否则MB_DEFBUTTON1是默认值。
        /// </summary>
        MB_DEFBUTTON1 = 0x000000,

        /// <summary>
        /// 第二个按钮是默认按钮。
        /// </summary>
        MB_DEFBUTTON2 = 0x000100,

        /// <summary>
        /// 第三个按钮是默认按钮。
        /// </summary>
        MB_DEFBUTTON3 = 0x000200,

        /// <summary>
        /// 第四个按钮是默认按钮。
        /// </summary>
        MB_DEFBUTTON4 = 0x000300,

        /// Group4
        /// 若要指示对话框的形式，请指定以下值之一。

        /// <summary>
        /// 在 hWnd 参数标识的窗口中继续工作之前，用户必须响应消息框。 但是，用户可以移动到其他线程的窗口，并在这些窗口中工作。
        /// 根据应用程序中的窗口层次结构，用户可能能够移动到线程中的其他窗口。 消息框父级的所有子窗口都将自动禁用，但弹出窗口不是。
        /// 如果未指定MB_SYSTEMMODAL或MB_TASKMODAL，则MB_APPLMODAL为默认值。
        /// </summary>
        MB_APPLMODAL = 0x000000,

        /// <summary>
        /// 与MB_APPLMODAL相同，消息框具有 WS_EX_TOPMOST 样式。 使用系统模式消息框通知用户严重、潜在的破坏性错误，
        /// 例如， (立即引起注意，) 内存不足。 此标志不会影响用户与 与 hWnd 关联的窗口以外的窗口交互的能力。
        /// </summary>
        MB_SYSTEMMODAL = 0x001000,

        /// <summary>
        /// 与 MB_APPLMODAL 相同，如果 hWnd 参数为 NULL，则禁用属于当前线程的所有顶级窗口。
        /// 当调用应用程序或库没有可用的窗口句柄时，请使用此标志，但仍需要防止输入到调用线程中的其他窗口，而不会挂起其他线程。
        /// </summary>
        MB_TASKMODAL = 0x002000,

        /// Group5
        /// 若要指定其他选项，请使用以下一个或多个值。

        /// <summary>
        /// Undocumented
        /// </summary>
        MB_NOFOCUS = 0x008000,

        /// <summary>
        /// 消息框将成为前台窗口。 在内部，系统调用消息框的 SetForegroundWindow 函数。
        /// </summary>
        MB_SETFOREGROUND = 0x00010000,

        /// <summary>
        /// 与交互式窗口工作站的桌面相同。 有关详细信息，请参阅 窗口工作站。
        /// 如果当前输入桌面不是默认桌面，则在用户切换到默认桌面之前， MessageBox 不会返回。
        /// </summary>
        MB_DEFAULT_DESKTOP_ONLY = 0x00020000,

        /// <summary>
        /// 消息框是使用 WS_EX_TOPMOST 窗口样式创建的。
        /// </summary>
        MB_TOPMOST = 0x00040000,

        /// <summary>
        /// 文本是右对齐的。
        /// </summary>
        MB_RIGHT = 0x00080000,

        /// <summary>
        /// 在希伯来语和阿拉伯语系统上使用从右到左阅读顺序显示消息和标题文本。
        /// </summary>
        MB_RTLREADING = 0x00100000,

        /// <summary>
        /// 调用方是通知用户某个事件的服务。 该函数在当前活动桌面上显示一个消息框，即使没有用户登录到计算机也是如此。
        /// 如果设置了此标志， 则 hWnd 参数必须为 NULL。 这样，消息框可以出现在桌面上，而不是与 hWnd 对应的桌面。
        /// </summary>
        /// <remarks>终端服务： 如果调用线程具有模拟令牌，该函数会将消息框定向到模拟令牌中指定的会话。</remarks>
        MB_SERVICE_NOTIFICATION = 0x00200000,
    }
}
