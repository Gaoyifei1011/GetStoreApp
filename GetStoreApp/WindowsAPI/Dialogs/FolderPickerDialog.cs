using GetStoreApp.WindowsAPI.Dialogs.FileDialog;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs
{
    /// <summary>
    /// 文件夹选取框
    /// </summary>
    public class FolderPickerDialog
    {
        public string Path { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 显示对话框（模态窗口）
        /// </summary>
        public bool ShowDialog(IntPtr hwnd)
        {
            IFileOpenDialog dialog = new FileOpenDialog() as IFileOpenDialog;
            try
            {
                if (hwnd == IntPtr.Zero) return false;

                FILEOPENDIALOGOPTIONS option = dialog.GetOptions();

                // 设置选项：选择文件夹，确保返回的项目是文件系统项目
                dialog.SetOptions(option | FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS | FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM);

                // 设置首选目录
                IShellItem item;
                if (!string.IsNullOrEmpty(Path))
                {
                    item = PInvoke.Shell32.Shell32Library.SHCreateItemFromParsingName(Path, IntPtr.Zero, typeof(IShellItem).GUID);
                    dialog.SetFolder(item);
                }

                // 设置标题
                if (!string.IsNullOrEmpty(Title))
                {
                    dialog.SetTitle(Title);
                }

                int hr = dialog.Show(hwnd);

                if (hr == BitConverter.ToInt32(BitConverter.GetBytes(0x800704C7), 0))
                {
                    return false;
                }

                if (hr != 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                dialog.GetResult(out item);
                item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string name);
                Path = name;

                return true;
            }
            finally
            {
                Marshal.FinalReleaseComObject(dialog);
            }
        }
    }
}
