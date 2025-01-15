using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Ole32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Foundation.Diagnostics;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 文件选取框
    /// </summary>
    public partial class OpenFileDialog : IDisposable
    {
        private readonly Guid CLSID_FileOpenDialog = new("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7");
        private IFileOpenDialog fileOpenDialog;
        private WindowId parentWindowId;

        public bool AllowMultiSelect { get; set; } = false;

        public string Description { get; set; } = string.Empty;

        public string SelectedFile { get; private set; } = string.Empty;

        public List<string> SelectedFileList { get; } = [];

        public List<string> FileTypeFilter { get; } = ["*"];

        public string RootFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public unsafe OpenFileDialog(WindowId windowId)
        {
            if (windowId.Value is 0)
            {
                throw new Exception("窗口句柄无效");
            }

            parentWindowId = windowId;
        }

        ~OpenFileDialog()
        {
            Dispose(false);
        }

        /// <summary>
        /// 显示文件夹选取对话框
        /// </summary>
        public unsafe bool ShowDialog()
        {
            try
            {
                int result = Ole32Library.CoCreateInstance(CLSID_FileOpenDialog, nint.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IFileOpenDialog).GUID, out nint ppv);

                if (result is 0)
                {
                    fileOpenDialog = (IFileOpenDialog)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.Unwrap);

                    if (FileTypeFilter.Count is 0)
                    {
                        throw new Exception("请添加要过滤的文件类型");
                    }

                    fileOpenDialog.SetTitle(Description);

                    if (AllowMultiSelect)
                    {
                        fileOpenDialog.SetOptions(FILEOPENDIALOGOPTIONS.FOS_ALLOWMULTISELECT);
                    }

                    COMDLG_FILTERSPEC[] comdlgFilterSpecArray = new COMDLG_FILTERSPEC[FileTypeFilter.Count];

                    for (int index = 0; index < FileTypeFilter.Count; index++)
                    {
                        COMDLG_FILTERSPEC comdlgFilterSpec = new()
                        {
                            pszSpec = (char*)Marshal.StringToHGlobalUni(string.Empty),
                            pszName = (char*)Marshal.StringToHGlobalUni(FileTypeFilter[index])
                        };

                        comdlgFilterSpecArray[index] = comdlgFilterSpec;
                    }

                    fileOpenDialog.SetFileTypes((uint)FileTypeFilter.Count, comdlgFilterSpecArray);
                    Shell32Library.SHCreateItemFromParsingName(RootFolder, nint.Zero, typeof(IShellItem).GUID, out nint initialFolder);
                    fileOpenDialog.SetFolder((IShellItem)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(initialFolder, CreateObjectFlags.Unwrap));

                    result = fileOpenDialog.Show(Win32Interop.GetWindowFromWindowId(parentWindowId));

                    if (result is not 0)
                    {
                        return false;
                    }

                    if (AllowMultiSelect)
                    {
                        fileOpenDialog.GetResults(out IShellItemArray shellItemArray);
                        shellItemArray.GetCount(out uint count);
                        SelectedFileList.Clear();

                        for (uint index = 0; index < count; index++)
                        {
                            shellItemArray.GetItemAt(index, out IShellItem shellItem);
                            shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string name);
                            SelectedFileList.Add(name);
                        }
                    }
                    else
                    {
                        fileOpenDialog.GetResult(out IShellItem shellItem);
                        shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string name);
                        SelectedFile = name;
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
                LogService.WriteLog(LoggingLevel.Error, "OpenFileDialog(IFileOpenDialog) initialize failed.", e);
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
