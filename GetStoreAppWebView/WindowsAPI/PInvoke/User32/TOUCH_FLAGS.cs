using System;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 可在 POINTER_TOUCH_INFO 结构的 touchFlags 字段中显示的值。
    /// </summary>
    [Flags]
    public enum TOUCH_FLAGS
    {
        /// <summary>
        /// 默认值。
        /// </summary>
        TOUCH_FLAG_NONE = 0x00000000,
    }
}
