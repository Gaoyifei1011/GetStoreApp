using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs
{
    /// <summary>
    /// 文件夹选取框
    /// </summary>
    public class FolderBrowserDialog
    {
        private static readonly Guid CLSID_FileOpenDialog = new Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7");

        private static readonly Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        private IFileOpenDialog FileOpenDialog;

        public string Path { get; set; }

        public string Title { get; set; }

        public unsafe FolderBrowserDialog()
        {
            try
            {
                fixed (Guid* CLSID_FileOpenDialog_Ptr = &CLSID_FileOpenDialog, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_FileOpenDialog_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, IID_IUnknown_Ptr, out IntPtr obj);

                    if (obj != IntPtr.Zero)
                    {
                        FileOpenDialog = (IFileOpenDialog)Marshal.GetTypedObjectForIUnknown(obj, typeof(IFileOpenDialog));
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "FolderBrowserDialog(IFileOpenDialog) initialize failed.", e);
                return;
            }
        }

        /// <summary>
        /// 显示对话框（模态窗口）
        /// </summary>
        public bool ShowDialog(IntPtr hwnd)
        {
            try
            {
                if (FileOpenDialog is not null)
                {
                    // 设置选项：选择文件夹
                    FileOpenDialog.SetOptions(FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS);

                    // 设置首选目录
                    if (!string.IsNullOrEmpty(Path))
                    {
                        unsafe
                        {
                            Guid iShellItemGuid = typeof(IShellItem).GUID;
                            Shell32Library.SHCreateItemFromParsingName(Path, IntPtr.Zero, &iShellItemGuid, out IntPtr ppv);
                            IShellItem initialFolder = (IShellItem)Marshal.GetObjectForIUnknown(ppv);
                            FileOpenDialog.SetFolder(initialFolder);
                            Marshal.ReleaseComObject(initialFolder);
                        }
                    }

                    // 设置标题
                    if (!string.IsNullOrEmpty(Title))
                    {
                        FileOpenDialog.SetTitle(Title);
                    }

                    int result = FileOpenDialog.Show(hwnd);

                    if (result is not 0)
                    {
                        return false;
                    }

                    FileOpenDialog.GetResult(out IShellItem pItem);
                    pItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out IntPtr pszString);
                    Path = Marshal.PtrToStringUni(pszString);
                    Marshal.ReleaseComObject(pItem);
                    if (FileOpenDialog is not null)
                    {
                        Marshal.FinalReleaseComObject(FileOpenDialog);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "FolderBrowserDialog(IFileOpenDialog) initialize failed.", e);
                if (FileOpenDialog is not null)
                {
                    Marshal.FinalReleaseComObject(FileOpenDialog);
                }
                return false;
            }
        }
    }
}
