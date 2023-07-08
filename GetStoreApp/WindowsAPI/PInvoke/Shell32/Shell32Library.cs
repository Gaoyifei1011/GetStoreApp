using GetStoreApp.WindowsAPI.Dialogs.FileDialog;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// shell32.dll 函数库
    /// </summary>
    public static partial class Shell32Library
    {
        private const string Shell32 = "shell32.dll";

        /// <summary>
        /// 将应用栏消息发送到系统。
        /// </summary>
        /// <param name="dwMessage">要发送的 Appbar 消息值。</param>
        /// <param name="pData">
        /// 指向 <see cref="APPBARDATA"> 结构的指针。 入口和退出结构的内容取决于 <param name="dwMessage"> 参数中设置的值。 有关具体信息，请参阅单独的邮件页。
        /// </param>
        /// <returns>此函数返回依赖于消息的值。 </returns>
        [LibraryImport(Shell32, EntryPoint = "SHAppBarMessage", SetLastError = false)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
        public static partial uint SHAppBarMessage(AppBarMessage dwMessage, ref APPBARDATA pData);

        /// <summary>
        /// 从分析名称创建和初始化命令行管理程序项对象。
        /// </summary>
        /// <param name="pszPath">指向显示名称的指针。</param>
        /// <param name="pbc">
        /// 自选。指向绑定上下文的指针，用于将参数作为输入和输出传递给分析函数。
        /// 这些传递的参数通常特定于数据源，并由数据源所有者记录。
        /// 例如，文件系统数据源接受正在使用STR_FILE_SYS_BIND_DATA绑定上下文参数分析的名称（作为 <see cref="WIN32_FIND_DATA"> 结构）。
        /// 可以传递STR_PARSE_PREFER_FOLDER_BROWSING以指示在可能的情况下使用文件系统数据源分析 URL。
        /// 使用 CreateBindCtx 构造绑定上下文对象，并使用IBindCtx::RegisterObjectParam 填充值。
        /// 有关这些键的完整列表，请参阅绑定上下文字符串键。有关使用此参数的示例，请参阅使用参数进行分析示例。
        /// 如果没有数据传递到分析函数或从分析函数接收任何数据，则此值可以为NULL。
        /// </param>
        /// <param name="riid">对接口的 IID 的引用，以通过ppv（通常为IID_IShellItem或IID_IShellItem2）进行检索。</param>
        /// <param name="ppv">此方法成功返回时，包含 riid 中请求的接口指针。这通常是IShellItem或IShellItem2。</param>
        /// <returns>此方法成功返回时，包含riid 中请求的接口指针。这通常是 <see cref="IShellItem"> 或IShellItem2。</returns>
        [LibraryImport(Shell32, EntryPoint = "SHCreateItemFromParsingName", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
        public static unsafe partial void SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, Guid* riid, out IntPtr ppv);

        /// <summary>
        /// 向任务栏的状态区域发送消息。
        /// </summary>
        /// <param name="cmd">一个值，该值指定要由此函数执行的操作</param>
        /// <param name="data">
        /// 指向 NOTIFYICONDATA 结构的指针。 结构的内容取决于 cmd 的值。
        /// 它可以定义一个图标以添加到通知区域，导致该图标显示通知，或标识要修改或删除的图标。
        /// </param>
        /// <returns>
        /// 如果成功，则返回 TRUE ;否则返回 FALSE 。 如果 dwMessage 设置为 NIM_SETVERSION，则函数在成功更改版本时返回 TRUE ;
        /// 如果请求的版本不受支持，则 返回 FALSE 。
        /// </returns>
        [DllImport(Shell32, EntryPoint = "Shell_NotifyIconW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Shell_NotifyIcon(NotifyIconMessage dwMessage, ref NOTIFYICONDATA lpData);
    }
}
