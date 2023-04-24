namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 要发送的 Appbar 消息值。
    /// </summary>
    public enum AppBarMessage : uint
    {
        /// <summary>
        /// 注册新的应用栏，并指定系统应用来将通知消息发送到应用栏的消息标识符。
        /// </summary>
        ABM_NEW = 0x00000000,

        /// <summary>
        /// 取消注册应用栏，从系统的内部列表中删除该栏。
        /// </summary>
        ABM_REMOVE = 0x00000001,

        /// <summary>
        /// 请求应用栏的大小和屏幕位置。
        /// </summary>
        ABM_QUERYPOS = 0x00000002,

        /// <summary>
        /// 设置应用栏的大小和屏幕位置。
        /// </summary>
        ABM_SETPOS = 0x00000003,

        /// <summary>
        /// 检索 Windows 任务栏的自动隐藏状态和始终处于顶部状态。
        /// </summary>
        ABM_GETSTATE = 0x00000004,

        /// <summary>
        /// 检索 Windows 任务栏的边界矩形。 请注意，这仅适用于系统任务栏。 其他对象（尤其是提供第三方软件的工具栏）也可以存在。 因此，Windows 任务栏未覆盖的某些屏幕区域可能对用户不可见。 若要检索任务栏和其他应用栏（应用程序可用的工作区）未覆盖的屏幕区域，请使用 GetMonitorInfo 函数。
        /// </summary>
        ABM_GETTASKBARPOS = 0x00000005,

        /// <summary>
        /// 通知系统激活或停用应用栏。 pData 指向的 <see cref="APPBARDATA"> 的 lParam 成员设置为 TRUE 以激活或 FALSE 以停用。
        /// </summary>
        ABM_ACTIVATE = 0x00000006,

        /// <summary>
        /// 检索与屏幕特定边缘关联的自动隐藏应用栏的句柄。
        /// </summary>
        ABM_GETAUTOHIDEBAR = 0x00000007,

        /// <summary>
        /// 注册或取消注册屏幕边缘的自动隐藏应用栏。
        /// </summary>
        ABM_SETAUTOHIDEBAR = 0x00000008,

        /// <summary>
        /// 在应用栏的位置发生更改时通知系统。
        /// </summary>
        ABM_WINDOWPOSCHANGED = 0x00000009,

        /// <summary>
        /// Windows XP 及更高版本： 设置应用栏的自动隐藏和始终置顶属性的状态。
        /// </summary>
        ABM_SETSTATE = 0x0000000A,

        /// <summary>
        /// Windows XP 及更高版本： 检索与特定监视器的特定边缘关联的自动隐藏应用栏的句柄。
        /// </summary>
        ABM_GETAUTOHIDEBAREX = 0x0000000B,

        /// <summary>
        /// Windows XP 及更高版本： 注册或取消注册特定监视器边缘的自动隐藏应用栏。
        /// </summary>
        ABM_SETAUTOHIDEBAREX = 0x0000000C
    }
}
