namespace GetStoreApp.WindowsAPI.Controls.Taskbar
{
    /// <summary>
    /// <see cref="THUMBBUTTON"> 结构用于指定该结构的成员包含有效数据。
    /// </summary>
    internal enum THUMBBUTTONMASK
    {
        /// <summary>
        /// <see cref="THUMBBUTTON.iBitmap"> 成员包含有效信息。
        /// </summary>
        THB_BITMAP = 0x1,

        /// <summary>
        /// <see cref="THUMBBUTTON.hIcon"> 成员包含有效信息。
        /// </summary>
        THB_ICON = 0x2,

        /// <summary>
        /// <see cref="THUMBBUTTON.szTip"> 成员包含有效信息。
        /// </summary>
        THB_TOOLTIP = 0x4,

        /// <summary>
        /// <see cref="THUMBBUTTON.dwFlags"> 成员包含有效信息。
        /// </summary>
        THB_FLAGS = 0x8
    }
}
