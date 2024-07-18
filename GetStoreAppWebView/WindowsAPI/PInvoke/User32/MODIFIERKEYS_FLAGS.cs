using System;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum MODIFIERKEYS_FLAGS : uint
    {
        MK_LBUTTON = 1,
        MK_RBUTTON = 2,
        MK_SHIFT = 4,
        MK_CONTROL = 8,
        MK_MBUTTON = 16,
        MK_XBUTTON1 = 32,
        MK_XBUTTON2 = 64
    }
}
