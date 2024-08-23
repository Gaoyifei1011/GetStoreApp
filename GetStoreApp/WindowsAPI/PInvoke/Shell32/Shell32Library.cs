using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// Shell32.dll 函数库
    /// </summary>
    public static partial class Shell32Library
    {
        public const string Shell32 = "shell32.dll";

        /// <summary>
        /// 从分析名称创建和初始化命令行管理程序项对象。
        /// </summary>
        /// <param name="pszPath">指向显示名称的指针。</param>
        /// <param name="pbc">
        /// 自选。指向绑定上下文的指针，用于将参数作为输入和输出传递给分析函数。
        /// 这些传递的参数通常特定于数据源，并由数据源所有者记录。
        /// 例如，文件系统数据源接受正在使用STR_FILE_SYS_BIND_DATA绑定上下文参数分析的名称（作为 WIN32_FIND_DATA 结构）。
        /// 可以传递STR_PARSE_PREFER_FOLDER_BROWSING以指示在可能的情况下使用文件系统数据源分析 URL。
        /// 使用 CreateBindCtx 构造绑定上下文对象，并使用IBindCtx::RegisterObjectParam 填充值。
        /// 有关这些键的完整列表，请参阅绑定上下文字符串键。有关使用此参数的示例，请参阅使用参数进行分析示例。
        /// 如果没有数据传递到分析函数或从分析函数接收任何数据，则此值可以为NULL。
        /// </param>
        /// <param name="riid">对接口的 IID 的引用，以通过ppv（通常为IID_IShellItem或IID_IShellItem2）进行检索。</param>
        /// <param name="ppv">此方法成功返回时，包含 riid 中请求的接口指针。这通常是IShellItem或IShellItem2。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [LibraryImport(Shell32, EntryPoint = "SHCreateItemFromParsingName", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        public static partial int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, out IntPtr ppv);
    }
}
