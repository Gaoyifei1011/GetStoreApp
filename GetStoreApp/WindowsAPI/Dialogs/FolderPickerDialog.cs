using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.Dialogs.FileDialog;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
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

                // 设置选项：选择文件夹
                dialog.SetOptions(FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS);

                // 设置首选目录
                if (!string.IsNullOrEmpty(Path))
                {
                    unsafe
                    {
                        Guid iShellItemGuid = typeof(IShellItem).GUID;
                        Shell32Library.SHCreateItemFromParsingName(Path, IntPtr.Zero, &iShellItemGuid, out IntPtr ppv);
                        IShellItem initialFolder = (IShellItem)Marshal.GetObjectForIUnknown(ppv);
                        dialog.SetFolder(initialFolder);
                        Marshal.ReleaseComObject(initialFolder);
                    }
                }

                // 设置标题
                if (!string.IsNullOrEmpty(Title))
                {
                    dialog.SetTitle(Title);
                }

                int result = dialog.Show(hwnd);

                if (result != 0)
                {
                    return false;
                }

                dialog.GetResult(out IShellItem pItem);
                pItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out IntPtr pszString);
                Path = Marshal.PtrToStringUni(pszString);
                Marshal.ReleaseComObject(pItem);
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Show Folder picker(IFileDialog) failed.", e);
                Marshal.FinalReleaseComObject(dialog);
                return false;
            }
        }
    }
}
