using GetStoreApp.WindowsAPI.Dialogs.FileDialog;
using System;
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
        /// 测试当前用户是否是管理员组的成员。
        /// </summary>
        /// <returns>如果用户是管理员组的成员，则返回 TRUE ;否则为 FALSE。</returns>
        [LibraryImport(Shell32, EntryPoint = "IsUserAnAdmin", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool IsUserAnAdmin();

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
        /// <returns>此方法成功返回时，包含riid 中请求的接口指针。这通常是 <see cref="IShellItem"> 或IShellItem2。</returns>
        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "SHCreateItemFromParsingName", ExactSpelling = true, PreserveSig = false, SetLastError = false)]
        public static extern IShellItem SHCreateItemFromParsingName(string pszPath, IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
    }
}
