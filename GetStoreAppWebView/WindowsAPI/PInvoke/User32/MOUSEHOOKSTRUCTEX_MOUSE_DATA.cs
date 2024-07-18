using System;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum MOUSEHOOKSTRUCTEX_MOUSE_DATA : uint
    {
        /// <summary>
        /// 按下或释放第一个 X 按钮。
        /// </summary>
        XBUTTON1 = 0x00000001,

        /// <summary>
        /// 按下或释放第二个 X 按钮。
        /// </summary>
        XBUTTON2 = 0x00000002,
    }
}
