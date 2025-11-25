using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 删除文件辅助类
    /// </summary>
    public static class DeleteFileHelper
    {
        private static readonly Guid CLSID_FileOperation = new("3AD05575-8857-4850-9277-11B85BDB8E09");
        private static readonly IFileOperation fileOperation = null;

        static DeleteFileHelper()
        {
            int result = Ole32Library.CoCreateInstance(CLSID_FileOperation, nint.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IFileOperation).GUID, out nint ppv);

            if (result is 0)
            {
                fileOperation = (IFileOperation)new StrategyBasedComWrappers().GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.None);
            }
        }

        /// <summary>
        /// 删除文件到回收站
        /// </summary>
        public static bool DeleteFileToRecycleBin(string filePath)
        {
            bool deleteResult = false;

            try
            {
                if (fileOperation is not null && File.Exists(filePath))
                {
                    fileOperation.SetOperationFlags(FileOperationFlags.FOF_ALLOWUNDO);
                    if (Shell32Library.SHCreateItemFromParsingName(filePath, nint.Zero, typeof(IShellItem).GUID, out IShellItem shellItem) is 0)
                    {
                        fileOperation.DeleteItem(shellItem, nint.Zero);
                        deleteResult = fileOperation.PerformOperations() is 0 || fileOperation.GetAnyOperationsAborted(out bool isAborted) is 0 && !isAborted;
                    }
                }
                else
                {
                    deleteResult = true;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeleteFileHelper), nameof(DeleteFileToRecycleBin), 1, e);
            }
            return deleteResult;
        }

        /// <summary>
        /// 删除多个文件到回收站
        /// </summary>
        public static bool DeleteFilesToRecycleBin(List<string> filePathList)
        {
            bool deleteResult = false;

            try
            {
                if (fileOperation is not null)
                {
                    if (filePathList.Count > 0)
                    {
                        fileOperation.SetOperationFlags(FileOperationFlags.FOF_ALLOWUNDO);
                        foreach (string filePath in filePathList)
                        {
                            if (Shell32Library.SHCreateItemFromParsingName(filePath, nint.Zero, typeof(IShellItem).GUID, out IShellItem shellItem) is 0)
                            {
                                fileOperation.DeleteItem(shellItem, nint.Zero);
                            }
                        }
                        deleteResult = fileOperation.PerformOperations() is 0 || fileOperation.GetAnyOperationsAborted(out bool isAborted) is 0 && !isAborted;
                    }
                    else
                    {
                        deleteResult = true;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeleteFileHelper), nameof(DeleteFilesToRecycleBin), 1, e);
            }
            return deleteResult;
        }
    }
}
