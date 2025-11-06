using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 删除文件辅助类
    /// </summary>
    public static class DeleteFileHelper
    {
        private static readonly Guid CLSID_FileOperation = new("3AD05575-8857-4850-9277-11B85BDB8E09");

        /// <summary>
        /// 删除文件到回收站
        /// </summary>
        public static bool DeleteFileToRecycleBin(string filePath)
        {
            try
            {
                IFileOperation fileOperation = null;

                int result = Ole32Library.CoCreateInstance(CLSID_FileOperation, nint.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IFileOperation).GUID, out nint ppv);

                if (result is 0)
                {
                    fileOperation = (IFileOperation)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.Unwrap);
                }

                if (fileOperation is not null)
                {
                    fileOperation.SetOperationFlags(FileOperationFlags.FOF_ALLOWUNDO);
                    if (Shell32Library.SHCreateItemFromParsingName(filePath, nint.Zero, typeof(IShellItem).GUID, out IShellItem shellItem) is 0)
                    {
                        fileOperation.DeleteItem(shellItem, nint.Zero);
                        return fileOperation.PerformOperations() is 0 || fileOperation.GetAnyOperationsAborted(out bool isAborted) is 0 && !isAborted;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeleteFileHelper), nameof(DeleteFileToRecycleBin), 1, e);
                return false;
            }
        }
    }
}
