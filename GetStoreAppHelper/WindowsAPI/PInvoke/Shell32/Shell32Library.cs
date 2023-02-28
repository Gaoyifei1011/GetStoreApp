using System.Runtime.InteropServices;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// shell32.dll 函数库
    /// </summary>
    public static partial class Shell32Library
    {
        private const string Shell32 = "shell32.dll";

        /// <summary>
        /// 向任务栏的状态区域发送消息。
        /// </summary>
        /// <param name="cmd">一个值，该值指定要由此函数执行的操作</param>
        /// <param name="data">
        /// 指向 <see cref="NOTIFYICONDATA"> 结构的指针。 结构的内容取决于 cmd 的值。
        /// 它可以定义一个图标以添加到通知区域，导致该图标显示通知，或标识要修改或删除的图标。
        /// </param>
        /// <returns>
        /// 如果成功，则返回 TRUE ;否则返回 FALSE 。 如果 dwMessage 设置为 NIM_SETVERSION，则函数在成功更改版本时返回 TRUE ;
        /// 如果请求的版本不受支持，则 返回 FALSE 。
        /// </returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "Shell_NotifyIcon", SetLastError = false)]
        public static extern bool Shell_NotifyIcon(NotifyCommand cmd, [In] ref NOTIFYICONDATA data);
    }
}
