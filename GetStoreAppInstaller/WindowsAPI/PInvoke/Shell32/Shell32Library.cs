using GetStoreAppInstaller.WindowsAPI.ComTypes;
using System.Runtime.InteropServices;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// Shell32.dll 函数库
    /// </summary>
    public static partial class Shell32Library
    {
        private const string Shell32 = "shell32.dll";

        /// <summary>
        /// 打开或创建文件，并检索要读取或写入该文件的流。
        /// </summary>
        /// <param name="pszFile">指向以 null 结尾的字符串的指针，该字符串指定文件名。</param>
        /// <param name="grfMode">用于指定文件访问模式以及如何创建和删除公开流的对象的一个或多个 STGM 值。</param>
        /// <param name="dwAttributes">一个或多个标志值，用于在创建新文件时指定文件属性。 有关可能值的完整列表，请参阅 CreateFile 函数的 dwFlagsAndAttributes 参数。</param>
        /// <param name="fCreate">一个 BOOL 值，可帮助与 grfMode 一起指定在创建流时应如何处理现有文件。</param>
        /// <param name="pstmTemplate">保留。</param>
        /// <param name="ppstm">接收与文件关联的流的 IStream 接口指针。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [LibraryImport(Shell32, EntryPoint = "SHCreateStreamOnFileEx", SetLastError = false), PreserveSig]
        public static partial int SHCreateStreamOnFileEx([MarshalAs(UnmanagedType.LPWStr)] string pszFile, STGM grfMode, uint dwAttributes, [MarshalAs(UnmanagedType.Bool)] bool fCreate, [MarshalAs(UnmanagedType.Interface)] IStream pstmTemplate, [MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
    }
}
