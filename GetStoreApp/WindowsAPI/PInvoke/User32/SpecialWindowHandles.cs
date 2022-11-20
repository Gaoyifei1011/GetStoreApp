using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// Special window handles.
    /// </summary>
    public static class SpecialWindowHandles
    {
        /// <summary>
        /// 将窗口放在 Z 顺序的顶部。
        /// </summary>
        public static readonly IntPtr HWND_TOP = new IntPtr(0);

        /// <summary>
        /// 将窗口放在 Z 顺序的底部。如果 hWnd 参数标识最顶层的窗口，则该窗口将失去其最顶层的状态，并放置在所有其他窗口的底部。
        /// </summary>
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        /// <summary>
        /// 将窗口置于所有非最顶层窗口的上方。即使停用窗口，窗口也会保持其最顶层位置。
        /// </summary>
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        /// <summary>
        /// 将窗口置于所有非最顶层窗口的上方（即，在所有最顶层窗口的后面）。如果窗口已经是非最顶层的窗口，则此标志不起作用。
        /// </summary>
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    }
}
