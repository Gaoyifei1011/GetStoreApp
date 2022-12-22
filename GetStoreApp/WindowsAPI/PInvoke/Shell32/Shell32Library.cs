using GetStoreApp.WindowsAPI.Dialogs.FileDialog.Native;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    public static class Shell32Library
    {
        private const string Shell32 = "shell32.dll";

        /// <summary>返回与指定文件路径关联的 ITEMIDLIST 结构。</summary>
        /// <param name="pszPath">指向包含路径的 null 终止 Unicode 字符串的指针。 此字符串长度不应超过 MAX_PATH 个字符，包括终止 null 字符。</param>
        /// <returns>返回指向对应于路径的 ITEMIDLIST 结构的指针。</returns>
        [DllImport(Shell32, EntryPoint = "ILCreateFromPathW", SetLastError = true)]
        public static extern IntPtr ILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath);

        /// <summary>释放 Shell 分配的 ITEMIDLIST 结构。</summary>
        /// <param name="pidl">指向要释放的 ITEMIDLIST 结构的指针。 此参数可以为 NULL。</param>
        /// <returns>无</returns>
        [DllImport(Shell32, ExactSpelling = true, SetLastError = false)]
        public static extern void ILFree(IntPtr pidlList);

        /// <summary>
        /// 从分析名称创建和初始化命令行管理程序项对象。
        /// </summary>
        /// <param name="pszPath">指向显示名称的指针。</param>
        /// <param name="pbc">
        /// 自选。指向绑定上下文的指针，用于将参数作为输入和输出传递给分析函数。
        /// 这些传递的参数通常特定于数据源，并由数据源所有者记录。
        /// 例如，文件系统数据源接受正在使用STR_FILE_SYS_BIND_DATA绑定上下文参数分析的名称（作为WIN32_FIND_DATA结构）。
        /// 可以传递STR_PARSE_PREFER_FOLDER_BROWSING以指示在可能的情况下使用文件系统数据源分析 URL。
        /// 使用 CreateBindCtx 构造绑定上下文对象，并使用IBindCtx::RegisterObjectParam 填充值。
        /// 有关这些键的完整列表，请参阅绑定上下文字符串键。有关使用此参数的示例，请参阅使用参数进行分析示例。
        /// 如果没有数据传递到分析函数或从分析函数接收任何数据，则此值可以为NULL。
        /// </param>
        /// <param name="riid">对接口的 IID 的引用，以通过ppv（通常为IID_IShellItem或IID_IShellItem2）进行检索。</param>
        /// <returns>此方法成功返回时，包含riid 中请求的接口指针。这通常是IShellItem或IShellItem2。</returns>
        [DllImport(Shell32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern IShellItem SHCreateItemFromParsingName(
            string pszPath,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        /// <summary>
        /// 对指定文件执行操作。
        /// </summary>
        /// <param name="hwnd">用于显示 UI 或错误消息的父窗口的句柄。 如果操作未与窗口关联，则此值可以为 NULL 。</param>
        /// <param name="lpOperation">指向 null 终止字符串的指针，在本例中称为 谓词，用于指定要执行的操作。</param>
        /// <param name="lpFile">
        /// 指向 null 终止字符串的指针，该字符串指定要对其执行指定谓词的文件或对象。
        /// 若要指定 Shell 命名空间对象，请传递完全限定分析名称。
        /// </param>
        /// <param name="lpParameters">如果 lpFile 指定可执行文件，则此参数是指向 一个 null 终止的字符串的指针，该字符串指定要传递给应用程序的参数。</param>
        /// <param name="lpDirectory">指向 null 终止字符串的指针，指定操作的默认 (工作) 目录。 如果此值为 NULL，则使用当前工作目录。</param>
        /// <param name="nShowCmd">
        /// 指定在打开应用程序时如何显示它的标志。 如果 lpFile 指定文档文件，则标志将直接传递给关联的应用程序。
        /// 由应用程序决定如何处理它。
        /// </param>
        /// <returns>如果函数成功，则返回大于 32 的值。 如果函数失败，它将返回一个错误值，该值指示失败的原因。</returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode)]
        public static extern IntPtr ShellExecute(
            IntPtr hwnd,
            string lpOperation,
            string lpFile,
            string lpParameters,
            string lpDirectory,
            WindowShowStyle nShowCmd);

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
        [DllImport(Shell32, CharSet = CharSet.Unicode)]
        public static extern bool Shell_NotifyIcon(NotifyCommand cmd, [In] ref NotifyIconData data);

        /// <summary>打开一个 Windows 资源管理器窗口，其中选定了特定文件夹中的指定项目。</summary>
        /// <param name="pidlFolder">指向指定文件夹的完全限定项 ID 列表的指针。</param>
        /// <param name="cidl">
        /// 选择数组中的项计数 apidl。 如果 cidl 为零， 则 pidlFolder 必须指向描述要选择的单个项的完全指定的 ITEMIDLIST 。 此函数将打开父文件夹并选择该项目。
        /// </param>
        /// <param name="apidl">指向 PIDL 结构的数组的指针，每个结构都是在 pidlFolder 引用的目标文件夹中选择要选择的项。</param>
        /// <param name="dwFlags">可选标志。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，它将返回 HRESULT 错误代码。</returns>
        [DllImport(Shell32, ExactSpelling = true)]
        public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, IntPtr apidl, uint dwFlags);
    }
}
