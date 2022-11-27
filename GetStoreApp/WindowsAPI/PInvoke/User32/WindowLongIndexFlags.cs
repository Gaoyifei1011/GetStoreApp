using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum WindowLongIndexFlags : int
    {
        /// <summary>
        /// 设置新的 扩展窗口样式。
        /// </summary>
        GWL_EXSTYLE = -20,

        /// <summary>
        /// 设置新的应用程序实例句柄。
        /// </summary>
        GWLP_HINSTANCE = -6,

        GWLP_HWNDPARENT = -8,

        /// <summary>
        /// 设置新的应用程序实例句柄。
        /// </summary>
        GWL_ID = -12,

        GWLP_ID = GWL_ID,

        /// <summary>
        /// 设置新的 窗口样式。
        /// </summary>
        GWL_STYLE = -16,

        GWL_USERDATA = -21,

        /// <summary>
        /// 设置与窗口关联的用户数据。 此数据供创建窗口的应用程序使用。 其值最初为零。
        /// </summary>
        GWLP_USERDATA = GWL_USERDATA,

        /// <summary>
        /// 设置窗口过程的新地址。如果窗口不属于与调用线程相同的进程，则无法更改此属性。
        /// </summary>
        GWL_WNDPROC = -4,

        GWLP_WNDPROC = GWL_WNDPROC,
        DWLP_USER = 0x8,
        DWLP_MSGRESULT = 0x0,
        DWLP_DLGPROC = 0x4,
    }
}
