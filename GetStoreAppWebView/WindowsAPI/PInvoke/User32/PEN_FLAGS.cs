namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 可在 POINTER_PEN_INFO 结构的 penFlags 字段中显示的值。
    /// </summary>
    public enum PEN_FLAGS
    {
        /// <summary>
        /// 没有笔标志，这是默认设置。
        /// </summary>
        PEN_FLAG_NONE = 0x00000000,

        /// <summary>
        /// 已按下筒状按钮。
        /// </summary>
        PEN_FLAG_BARREL = 0x00000001,

        /// <summary>
        /// 笔是倒置的。
        /// </summary>
        PEN_FLAG_INVERTED = 0x00000002,

        /// <summary>
        /// 按下橡皮擦按钮。
        /// </summary>
        PEN_FLAG_ERASER = 0x00000004
    }
}
