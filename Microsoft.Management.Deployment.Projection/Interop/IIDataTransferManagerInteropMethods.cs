using System;
using WinRT;

namespace Windows.ApplicationModel.DataTransfer
{
    /// <summary>
    /// 等效调用 IDataTransferManagerInterop 接口的方法
    /// </summary>
    internal static class IDataTransferManagerInteropMethods
    {
        internal static unsafe DataTransferManager GetForWindow(IObjectReference _obj, IntPtr appWindow, in Guid riid)
        {
            IntPtr thisPtr = _obj.ThisPtr;
            IntPtr ptr = new();

            try
            {
                // IDataTransferManagerInterop 继承了 IUnknown (3 个函数)，并提供了 GetForWindow，总共提供了5个函数
                ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, Guid, out IntPtr, int>**)thisPtr)[3](thisPtr, appWindow, riid, out ptr));
                return MarshalInspectable<DataTransferManager>.FromAbi(ptr);
            }
            finally
            {
                MarshalInspectable<DataTransferManager>.DisposeAbi(ptr);
            }
        }

        internal static unsafe void ShowShowShareUIForWindow(IObjectReference _obj, IntPtr appWindow)
        {
            IntPtr thisPtr = _obj.ThisPtr;

            // IDataTransferManagerInterop 继承了 IUnknown (3 个函数)，并提供了 ShowShowShareUIForWindow，总共提供了5个函数
            ExceptionHelpers.ThrowExceptionForHR((*(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int>**)thisPtr)[4](thisPtr, appWindow));
        }
    }
}
