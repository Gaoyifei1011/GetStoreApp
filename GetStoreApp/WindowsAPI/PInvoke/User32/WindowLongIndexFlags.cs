using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum WindowLongIndexFlags : int
    {
        /// <summary>
        /// 检索扩展窗口样式。
        /// </summary>
        GWL_EXSTYLE = -20,

        /// <summary>
        /// 检索应用程序实例的句柄。
        /// </summary>
        GWLP_HINSTANCE = -6,

        /// <summary>
        /// 检索父窗口的句柄（如果有）。
        /// </summary>
        GWLP_HWNDPARENT = -8,

        /// <summary>
        /// 设置新的应用程序实例句柄。
        /// </summary>
        GWL_ID = -12,

        /// <summary>
        /// 检索窗口的标识符。
        /// </summary>
        GWLP_ID = GWL_ID,

        /// <summary>
        /// 检索窗口样式。
        /// </summary>
        GWL_STYLE = -16,

        /// <summary>
        /// 检索与窗口关联的用户数据。 此数据供创建窗口的应用程序使用。 其值最初为零。
        /// </summary>
        GWL_USERDATA = -21,

        /// <summary>
        /// 检索窗口过程的地址，或表示窗口过程的地址的句柄。 必须使用 <see cref="User32Library.CallWindowProc(nint, nint, WindowMessage, nint, nint)"> 函数调用窗口过程。
        /// </summary>
        GWL_WNDPROC = -4,

        /// <summary>
        /// 检索应用程序专用的额外信息，例如句柄或指针。
        /// </summary>
        DWLP_USER = 0x8,

        /// <summary>
        /// 检索对话框中处理的消息的返回值。
        /// </summary>
        DWLP_MSGRESULT = 0x0,

        /// <summary>
        /// 检索对话框过程的地址，或表示对话框过程的地址的句柄。 必须使用 CallWindowProc 函数调用对话框过程。
        /// </summary>
        DWLP_DLGPROC = 0x4,
    }
}
