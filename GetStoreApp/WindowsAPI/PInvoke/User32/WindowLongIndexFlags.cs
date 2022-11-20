using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum WindowLongIndexFlags : int
    {
        GWL_EXSTYLE = -20,
        GWLP_HINSTANCE = -6,
        GWLP_HWNDPARENT = -8,
        GWL_ID = -12,
        GWLP_ID = GWL_ID,
        GWL_STYLE = -16,
        GWL_USERDATA = -21,
        GWLP_USERDATA = GWL_USERDATA,
        GWL_WNDPROC = -4,
        GWLP_WNDPROC = GWL_WNDPROC,
        DWLP_USER = 0x8,
        DWLP_MSGRESULT = 0x0,
        DWLP_DLGPROC = 0x4,
    }
}
