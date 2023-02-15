using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 通过添加特定于打开对话框的方法扩展 IFileDialog 接口。
    /// </summary>
    [ComImport]
    [Guid("42f85136-db7e-439c-85f1-e4075d135fc8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileOpenDialog
    {
        [PreserveSig]
        int Show([In] IntPtr hwnd);

        void SetFileTypes();

        void SetFileTypeIndex();

        void GetFileTypeIndex();

        /// <summary>
        /// 分配侦听来自对话的事件的事件处理程序。
        /// </summary>
        void Advise();

        void Unadvise();

        void SetOptions([In] FILEOPENDIALOGOPTIONS fos);

        FILEOPENDIALOGOPTIONS GetOptions();

        void SetDefaultFolder();

        void SetFolder(IShellItem psi);

        void GetFolder();

        void GetCurrentSelection();

        void SetFileName();

        void GetFileName();

        void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        void SetOkButtonLabel();

        void SetFileNameLabel();

        void GetResult(out IShellItem ppsi);

        /// <summary>
        /// 将文件夹添加到可供用户打开或保存项的位置列表中。
        /// </summary>
        void AddPlace();

        void SetDefaultExtension();

        void Close();

        void SetClientGuid();

        void ClearClientData();

        void SetFilter();

        /// <summary>
        /// 在允许多项选择的对话框中获取用户的选择。
        /// </summary>
        void GetResults();

        /// <summary>
        /// 获取对话框中当前选定的项。这些项目可能是在视图中选择的项目，也可能是在文件名编辑框中选择的文本。
        /// </summary>

        void GetSelectedItems();
    }
}
