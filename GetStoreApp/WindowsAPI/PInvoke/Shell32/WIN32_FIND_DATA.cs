using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 包含有关 由 FindFirstFile、 FindFirstFileEx 或 FindNextFile 函数找到的文件的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WIN32_FIND_DATA
    {
        public const int MAX_PATH = 260;

        /// <summary>
        /// 文件的文件属性。如果文件的任何流曾经稀疏，则设置文件上的 FILE_ATTRIBUTE_SPARSE_FILE 属性。
        /// </summary>
        private uint dwFileAttributes;

        /// <summary>
        /// <see cref="FILETIME"> 结构，指定文件或目录的创建时间。如果基础文件系统不支持创建时间，则此成员为零。
        /// </summary>
        private FILETIME ftCreationTime;

        /// <summary>
        /// <see cref="FILETIME"> 结构。对于文件，结构指定上次读取、写入文件或可执行文件的运行时间。对于目录，结构指定何时创建目录。
        /// 如果基础文件系统不支持上次访问时间，则此成员为零。在 FAT 文件系统上，文件和目录的指定日期正确，但一天中的时间始终设置为午夜。
        /// </summary>
        private FILETIME ftLastAccessTime;

        /// <summary>
        /// <see cref="FILETIME"> 结构。对于文件， 结构指定文件上次写入、截断或覆盖时间，例如，何时使用 WriteFile 或 SetEndOfFile 。
        /// 更改文件属性或安全描述符时，不会更新日期和时间。对于目录，结构指定何时创建目录。 如果基础文件系统不支持上次写入时间，则此成员为零。
        /// </summary>
        private FILETIME ftLastWriteTime;

        /// <summary>
        /// 文件大小的高阶 DWORD 值（以字节为单位）。 除非文件大小大于 MAXDWORD，否则此值为零。文件大小等于(nFileSizeHigh* (MAXDWORD+1) ) + nFileSizeLow。
        /// </summary>
        private uint nFileSizeHigh;

        /// <summary>
        /// 文件大小的低序 DWORD 值（以字节为单位）。
        /// </summary>
        private uint nFileSizeLow;

        /// <summary>
        /// 如果 dwFileAttributes 成员包含 FILE_ATTRIBUTE_REPARSE_POINT 属性，则此成员指定重分析点标记。否则，此值未定义，不应使用。
        /// </summary>
        private uint dwReserved0;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        private string cFileName;
    }
}
