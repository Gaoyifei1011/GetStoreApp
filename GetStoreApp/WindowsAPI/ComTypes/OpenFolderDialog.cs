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
        private IFileOpenDialog fileOpenDialog;
        private WindowId parentWindowId;

        public string Description { get; set; } = string.Empty;

        public string SelectedPath { get; private set; } = string.Empty;

        public string RootFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public OpenFolderDialog(WindowId windowId)
        {
            if (windowId.Value is 0)
            {
                throw new Exception("窗口句柄无效");
            }

            parentWindowId = windowId;
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
                int result = Ole32Library.CoCreateInstance(CLSID_FileOpenDialog, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IFileOpenDialog).GUID, out IntPtr ppv);

                if (result is 0)
                {
                    fileOpenDialog = (IFileOpenDialog)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.Unwrap);

                    fileOpenDialog.SetOptions(FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS);
                    fileOpenDialog.SetTitle(Description);
                    Shell32Library.SHCreateItemFromParsingName(RootFolder, IntPtr.Zero, typeof(IShellItem).GUID, out IntPtr initialFolder);
                    fileOpenDialog.SetFolder((IShellItem)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(initialFolder, CreateObjectFlags.Unwrap));

                    result = fileOpenDialog.Show(Win32Interop.GetWindowFromWindowId(parentWindowId));

                    if (result is not 0)
                    {
                        return false;
                    }

                    fileOpenDialog.GetResult(out IShellItem shellItem);
                    shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string name);
                    SelectedPath = name;
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
                fileOpenDialog = null;
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
                fileOpenDialog = null;
            }
        }
    }
}
