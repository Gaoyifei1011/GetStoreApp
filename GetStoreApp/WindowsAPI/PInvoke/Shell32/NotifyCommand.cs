namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 在 <see cref="Shell32Library.Shell_NotifyIcon"/> 函数上执行的主要操作
    /// </summary>
    public enum NotifyCommand
    {
        /// <summary>
        /// 将图标添加到状态区域。 该图标在 lpdata 指向的 NOTIFYICONDATA 结构中提供标识符，该标识符是通过其 uID 或 guidItem 成员。
        /// 此标识符用于后续调用 Shell_NotifyIcon 以对图标执行后续操作。
        /// </summary>
        NIM_ADD = 0x00000000,

        /// <summary>
        /// 修改状态区域中的图标。 lpdata 指向的 NOTIFYICONDATA 结构使用最初分配给图标的 ID，将其添加到通知区域 (NIM_ADD) 以标识要修改的图标。
        /// </summary>
        NIM_MODIFY = 0x00000001,

        /// <summary>
        /// 从状态区域删除图标。 lpdata 指向的 NOTIFYICONDATA 结构使用最初分配给图标的 ID，将其添加到通知区域时 (NIM_ADD) 标识要删除的图标。
        /// </summary>
        NIM_DELETE = 0x00000002,

        /// <summary>
        /// 将焦点返回到任务栏通知区域。 通知区域图标在完成 UI 操作后应使用此消息。
        /// 每次 (NIM_ADD) 添加通知区域图标时，都必须调用NIM_SETVERSION >。 不需要使用NIM_MODIFY调用它。 用户注销后，版本设置不会持久保存。
        /// </summary>
        NIM_SETFOCUS = 0x00000003,

        /// <summary>
        /// 指示通知区域根据 lpdata 指向的结构的 uVersion 成员中指定的版本号的行为。 版本号指定识别的成员。
        /// </summary>
        NIM_SETVERSION = 0x00000004
    }
}
