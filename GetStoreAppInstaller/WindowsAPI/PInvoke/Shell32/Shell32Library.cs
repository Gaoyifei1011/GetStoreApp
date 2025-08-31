using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// Shell32.dll 函数库
    /// </summary>
    public static partial class Shell32Library
    {
        public const string Shell32 = "shell32.dll";

        /// <summary>
        /// 注册窗口是否接受已删除的文件。
        /// </summary>
        /// <param name="hwnd">正在注册是否接受已删除文件的窗口的标识符。</param>
        /// <param name="fAccept">一个值，该值指示 hWnd 参数标识的窗口是否接受已删除的文件。 如果接受已删除的文件，则此值为 TRUE ;如果值为 FALSE ，则表示停止接受已删除的文件。</param>
        [LibraryImport(Shell32, EntryPoint = "DragAcceptFiles", SetLastError = false), PreserveSig]
        public static partial void DragAcceptFiles(nint hwnd, [MarshalAs(UnmanagedType.Bool)] bool fAccept);

        /// <summary>
        /// 检索由于成功拖放操作而删除的文件的名称。
        /// </summary>
        /// <param name="hDrop">包含已删除文件的文件名的结构的标识符。</param>
        /// <param name="iFile">要查询的文件的索引。 如果此参数的值为 0xFFFFFFFF， 则 DragQueryFile 将返回已删除的文件计数。 如果此参数的值介于零和删除的文件总数之间， 则 DragQueryFile 会将具有相应值的文件名复制到 lpszFile 参数指向的缓冲区。</param>
        /// <param name="lpszFile">在函数返回时接收已删除文件的文件名的缓冲区的地址。 此文件名是以 null 结尾的字符串。 如果此参数为 NULL，DragQueryFile 将返回此缓冲区的所需大小（以字符为单位）。</param>
        /// <param name="cch">lpszFile 缓冲区的大小（以字符为单位）。</param>
        /// <returns>
        /// 非零值表示调用成功。
        /// 当函数将文件名复制到缓冲区时，返回值是复制的字符计数，不包括终止 null 字符。
        /// 如果索引值为0xFFFFFFFF，则返回值是已删除文件的计数。 请注意，索引变量本身返回不变，因此保持0xFFFFFFFF。
        /// 如果索引值介于零和已删除文件总数之间，并且 lpszFile 缓冲区地址为 NULL，则返回值是缓冲区所需的大小（以字符为单位）， 不包括 终止 null 字符。
        /// </returns>
        [LibraryImport(Shell32, EntryPoint = "DragQueryFileW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        public static partial uint DragQueryFile(nuint hDrop, uint iFile, [Out, MarshalAs(UnmanagedType.LPArray)] char[] lpszFile, uint cch);

        /// <summary>
        /// 检索在拖放操作期间删除文件时鼠标指针的位置。
        /// </summary>
        /// <param name="hDrop">述已删除文件的放置结构的句柄。</param>
        /// <param name="lppt">指向 POINT 结构的指针，当此函数成功返回时，该结构接收删除文件时鼠标指针的坐标。</param>
        /// <returns>如果删除发生在窗口的工作区中，则为 TRUE;否则为 FALSE。</returns>
        [LibraryImport(Shell32, EntryPoint = "DragQueryPoint", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DragQueryPoint(nuint hDrop, out PointInt32 lppt);

        /// <summary>
        /// 描述已删除的文件的结构的标识符。 此句柄是从WM_DROPFILES消息的 wParam 参数检索的。
        /// </summary>
        /// <param name="hDrop">释放系统分配用于将文件名传输到应用程序的内存。</param>
        [LibraryImport(Shell32, EntryPoint = "DragFinish", SetLastError = false)]
        public static partial void DragFinish(nuint hDrop);
    }
}
