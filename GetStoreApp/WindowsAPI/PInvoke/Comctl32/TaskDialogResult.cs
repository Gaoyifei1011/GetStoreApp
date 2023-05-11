namespace GetStoreApp.WindowsAPI.PInvoke.Comctl32
{
    public enum TaskDialogResult
    {
        /// <summary>
        /// 函数调用失败。 有关详细信息，请参阅返回值。
        /// </summary>
        None = 0,

        /// <summary>
        /// 已选择“确定”按钮。
        /// </summary>
        IDOK = 1,

        /// <summary>
        /// 已选择“取消”按钮，按 Alt-F4，已按下转义，或者用户单击关闭窗口按钮。
        /// </summary>
        IDCANCEL = 2,

        IDABORT = 3,

        /// <summary>
        /// 已选择“重试”按钮。
        /// </summary>
        IDRETRY = 4,

        IDIGNORE = 5,

        /// <summary>
        /// 已选择“是”按钮。
        /// </summary>
        IDYES = 6,

        /// <summary>
        /// 未选择任何按钮。
        /// </summary>
        IDNO = 7,

        IDCLOSE = 8
    }
}
