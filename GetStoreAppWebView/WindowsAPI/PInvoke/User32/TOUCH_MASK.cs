namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 可在 POINTER_TOUCH_INFO 结构的 touchMask 字段中显示的值。
    /// </summary>
    public enum TOUCH_MASK
    {
        /// <summary>
        /// 默认。任何可选字段都无效。
        /// </summary>
        TOUCH_MASK_NONE = 0x00000000,

        /// <summary>
        /// POINTER_TOUCH_INFO 结构的 rcContact 字段有效。
        /// </summary>
        TOUCH_MASK_CONTACTAREA = 0x00000001,

        /// <summary>
        /// POINTER_TOUCH_INFO 结构的 orientation 字段有效。
        /// </summary>
        TOUCH_MASK_ORIENTATION = 0x00000002,

        /// <summary>
        /// POINTER_TOUCH_INFO 结构的 pressure 字段有效。
        /// </summary>
        TOUCH_MASK_PRESSURE = 0x00000004
    }
}
