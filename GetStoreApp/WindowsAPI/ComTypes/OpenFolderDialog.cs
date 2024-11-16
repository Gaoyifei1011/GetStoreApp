using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI;
using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 文件夹选取框
    /// </summary>
    public partial class OpenFolderDialog : IDisposable
    {
        private readonly Guid CLSID_FileOpenDialog = new("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7");
        private IFileOpenDialog FileOpenDialog;
        private WindowId parentWindowId;

        public string Description { get; set; } = string.Empty;

        public string SelectedPath { get; set; } = string.Empty;

        public string RootFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public OpenFolderDialog(WindowId windowId)
        {
            if (windowId.Value is 0)
            {
                throw new Exception("窗口句柄无效");
            }

            parentWindowId = windowId;
            int result = Ole32Library.CoCreateInstance(CLSID_FileOpenDialog, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IFileOpenDialog).GUID, out IntPtr ppv);

            if (result is 0)
            {
                FileOpenDialog = (IFileOpenDialog)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.Unwrap);
            }

            FileOpenDialog.SetOptions(FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS);
            FileOpenDialog.SetTitle(Description);
            Shell32Library.SHCreateItemFromParsingName(RootFolder, IntPtr.Zero, typeof(IShellItem).GUID, out IntPtr initialFolder);
            FileOpenDialog.SetFolder((IShellItem)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(initialFolder, CreateObjectFlags.Unwrap));
        }

        ~OpenFolderDialog()
        {
            Dispose(false);
        }

        /// <summary>
        /// 显示文件夹选取对话框
        /// </summary>
        public bool ShowDialog()
        {
            try
            {
                if (FileOpenDialog is not null)
                {
                    int result = FileOpenDialog.Show(Win32Interop.GetWindowFromWindowId(parentWindowId));

                    if (result is not 0)
                    {
                        return false;
                    }

                    FileOpenDialog.GetResult(out IShellItem pItem);
                    pItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string pszString);
                    SelectedPath = pszString;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                FileOpenDialog = null;
                return false;
            }
        }

        /// <summary>
        /// 释放打开文件夹选取对话框所需的资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            lock (this)
            {
                FileOpenDialog = null;
            }
        }
    }
}
