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
        public static partial int ShellExecute(nint hwnd, [MarshalAs(UnmanagedType.LPWStr)] string lpOperation, [MarshalAs(UnmanagedType.LPWStr)] string lpFile, string lpParameters, [MarshalAs(UnmanagedType.LPWStr)] string lpDirectory, WindowShowStyle nShowCmd);
    }
}
