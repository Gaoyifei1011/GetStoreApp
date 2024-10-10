using GetStoreApp.WindowsAPI.PInvoke.User32;
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
        public static partial int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, in Guid riid, out IntPtr ppv);

        /// <summary>
        /// 对指定文件执行操作。
        /// </summary>
        /// <param name="hwnd">用于显示 UI 或错误消息的父窗口的句柄。 如果操作未与窗口关联，则此值可以为 NULL 。</param>
        /// <param name="lpOperation">指向以 null 结尾的字符串（在本例中称为 谓词）的指针，指定要执行的操作。 可用谓词集取决于特定的文件或文件夹。 通常，对象的快捷菜单中可用的操作是可用的谓词。</param>
        /// <param name="lpFile">指向 以 null 结尾的字符串的指针，该字符串指定要对其执行指定谓词的文件或对象。 若要指定 Shell 命名空间对象，请传递完全限定分析名称。 请注意，并非所有对象都支持所有谓词。 例如，并非所有文档类型都支持“print”谓词。 如果将相对路径用于 lpDirectory 参数，请不要对 lpFile 使用相对路径。</param>
        /// <param name="lpParameters">如果 lpFile 指定可执行文件，则此参数是指向以 null 结尾的字符串的指针，该字符串指定要传递给应用程序的参数。 此字符串的格式由要调用的谓词决定。 如果 lpFile 指定文档文件， 则 lpParameters 应为 NULL。</param>
        /// <param name="lpDirectory">指向 以 null 结尾的字符串的指针，该字符串指定操作) 目录的默认 (。 如果此值为 NULL，则使用当前工作目录。 如果在 lpFile 中提供了相对路径，请不要对 lpDirectory 使用相对路径。</param>
        /// <param name="nShowCmd">指定应用程序在打开时如何显示应用程序的标志。 如果 lpFile 指定文档文件，则标志将直接传递给关联的应用程序。 由应用程序决定如何处理它。 它可以是在 ShowWindow 函数的 nCmdShow 参数中指定的任何值。</param>
        /// <returns>如果函数成功，则返回大于 32 的值。 如果函数失败，它将返回一个错误值，该值指示失败的原因。 返回值转换为 HINSTANCE，以便与 16 位 Windows 应用程序向后兼容。</returns>
        [LibraryImport(Shell32, EntryPoint = "ShellExecuteW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        public static partial int ShellExecute(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string lpOperation, [MarshalAs(UnmanagedType.LPWStr)] string lpFile, string lpParameters, [MarshalAs(UnmanagedType.LPWStr)] string lpDirectory, WindowShowStyle nShowCmd);
    }
}
