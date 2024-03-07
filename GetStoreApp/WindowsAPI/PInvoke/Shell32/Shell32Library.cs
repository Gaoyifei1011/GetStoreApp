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
        /// 显示一个对话框，使用户能够选择 Shell 文件夹。
        /// </summary>
        /// <param name="lpbi">指向 BROWSEINFO 结构的指针，该结构包含用于显示对话框的信息。</param>
        /// <returns>
        /// 返回一个 PIDL，该值指定所选文件夹相对于命名空间根目录的位置。 如果用户在对话框中选择“ 取消 ”按钮，则返回值为 NULL。
        /// 返回的 PIDL 可能是文件夹快捷方式而不是文件夹的。 有关此情况的完整讨论，请参阅备注部分。
        /// </returns>
        [LibraryImport(Shell32, EntryPoint = "SHBrowseForFolderW", SetLastError = false)]
        public static partial IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

        /// <summary>
        /// 将项标识符列表转换为文件系统路径。
        /// </summary>
        /// <param name="pidl">(项标识符列表的地址，该列表指定相对于桌面) 命名空间根目录文件或目录位置。</param>
        /// <param name="pszPath">要接收文件系统路径的缓冲区的地址。 此缓冲区的大小必须至少为 MAX_PATH 个字符。</param>
        /// <returns>如果成功，则返回 TRUE ;否则为 FALSE。</returns>
        [LibraryImport(Shell32, EntryPoint = "SHGetPathFromIDListW", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe partial bool SHGetPathFromIDList(IntPtr pidl, char* pszPath);
    }
}
