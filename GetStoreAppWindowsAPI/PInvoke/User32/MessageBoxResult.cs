namespace GetStoreAppWindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 表示MessageBox 函数返回的可能值
    /// </summary>
    public enum MessageBoxResult : uint
    {
        /// <summary>
        /// 已选择 “确定 ”按钮。
        /// </summary>
        IDOK = 1,

        /// <summary>
        /// 已选择“ 取消 ”按钮。
        /// </summary>
        IDCANCEL = 2,

        /// <summary>
        /// 已选择 “中止 ”按钮。
        /// </summary>
        IDABORT = 3,

        /// <summary>
        /// 已选择 “重试 ”按钮。
        /// </summary>
        IDRETRY = 4,

        /// <summary>
        /// 已选择 “忽略 ”按钮。
        /// </summary>
        IDIGNORE = 5,

        /// <summary>
        /// 已选择 “是 ”按钮。
        /// </summary>
        IDYES = 6,

        /// <summary>
        /// 已选择 “无 ”按钮。
        /// </summary>
        IDNO = 7,

        /// <summary>
        /// 用户关闭了消息框。
        /// </summary>
        IDCLOSE = 8,

        /// <summary>
        /// 已选择 “帮助 ”按钮。
        /// </summary>
        IDHELP = 9,

        /// <summary>
        /// 已选择 “重试 ”按钮。
        /// </summary>
        IDTRYAGAIN = 10,

        /// <summary>
        /// 已选择 “继续 ”按钮。
        /// </summary>
        IDCONTINUE = 11,

        /// <summary>
        /// 用户未单击任何按钮，并且消息框超时。
        /// </summary>
        IDTIMEOUT = 32000,
    }
}
