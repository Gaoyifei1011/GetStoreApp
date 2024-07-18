using System;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum PEN_MASK
    {
        /// <summary>
        /// 默认。 任何可选字段都无效。
        /// </summary>
        PEN_MASK_NONE = 0x00000000,

        /// <summary>
        /// POINTER_PEN_INFO 结构的按压字段有效。
        /// </summary>
        PEN_MASK_PRESSURE = 0x00000001,

        /// <summary>
        /// POINTER_PEN_INFO 结构的旋转字段有效。
        /// </summary>
        PEN_MASK_ROTATION = 0x00000002,

        /// <summary>
        /// POINTER_PEN_INFO结构的 tiltX 字段有效。
        /// </summary>
        PEN_MASK_TILT_X = 0x00000004,

        /// <summary>
        /// POINTER_PEN_INFO 结构的 tiltY 字段有效。
        /// </summary>
        PEN_MASK_TILT_Y = 0x00000008
    }
}
