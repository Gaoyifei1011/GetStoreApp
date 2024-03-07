using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 包含 SHBrowseForFolder 函数的参数，并接收有关用户选择的文件夹的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct BROWSEINFO
    {
        /// <summary>
        /// 对话框的所有者窗口的句柄。
        /// </summary>
        public IntPtr hwndOwner;

        /// <summary>
        /// 一个 PIDL，指定要从中开始浏览的根文件夹的位置。 对话框中仅显示命名空间层次结构中的指定文件夹及其子文件夹。 此成员可以为 NULL;在这种情况下，将使用默认位置。
        /// </summary>
        public IntPtr pidlRoot;

        /// <summary>
        /// 指向缓冲区的指针，用于接收用户选择的文件夹的显示名称。 假定此缓冲区的大小为MAX_PATH个字符。
        /// </summary>
        public IntPtr pszDisplayName;

        /// <summary>
        /// 指向以 null 结尾的字符串的指针，该字符串显示在对话框中的树视图控件上方。 此字符串可用于向用户指定指令。
        /// </summary>
        public IntPtr lpszTitle;

        /// <summary>
        /// 指定对话框选项的标志。 此成员可以是 0，也可以是以下值的组合。 版本号是指 SHBrowseForFolder 识别在更高版本中添加的标志所需的最低版本 Shell32.dll。 有关详细信息 ，请参阅 Shell 和公共控件版本 。
        /// </summary>
        public BROWSEINFOFLAGS ulFlags;

        /// <summary>
        /// 指向发生事件时对话框调用的应用程序定义函数的指针。 有关详细信息，请参阅 BrowseCallbackProc 函数。 此成员可以为 NULL。
        /// </summary>
        public IntPtr lpfn;

        /// <summary>
        /// 对话框传递给回调函数的应用程序定义值（如果在 lpfn 中指定）。
        /// </summary>
        public IntPtr lParam;

        /// <summary>
        /// 一个整数值，用于接收与存储在系统映像列表中的所选文件夹关联的图像的索引。
        /// </summary>
        public int iImage;
    }
}
