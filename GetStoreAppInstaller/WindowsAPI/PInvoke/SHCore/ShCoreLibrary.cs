using GetStoreAppInstaller.WindowsAPI.ComTypes;
using System;
using System.Runtime.InteropServices;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.SHCore
{
    public static partial class ShCoreLibrary
    {
        private const string ShCore = "shCore.dll";

        /// <summary>
        /// 围绕 IStream 基本实现创建Windows 运行时随机访问流。
        /// </summary>
        /// <param name="stream">要封装的 COM 流。</param>
        /// <param name="options">BSOS_OPTIONS 选项之一，用于指定封装流的 RandomAccessStream 的行为。</param>
        /// <param name="riid">对要通过 ppv 检索的接口的 IID 的引用，通常IID_RandomAccessStream。</param>
        /// <param name="ppv">此方法成功返回时，包含指向 RandomAccessStream 的接口指针，该指针封装 riid 中请求的流。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [LibraryImport(ShCore, EntryPoint = "CreateRandomAccessStreamOverStream", SetLastError = false), PreserveSig]
        public static partial int CreateRandomAccessStreamOverStream([MarshalAs(UnmanagedType.Interface)] IStream stream, BSOS_OPTIONS options, Guid riid, out nint ppv);

        /// <summary>
        /// 围绕 Windows 运行时 IRandomAccessStream 对象创建 IStream。
        /// </summary>
        /// <param name="randomAccessStream">源 IRandomAccessStream。</param>
        /// <param name="riid">对要通过 ppv 检索的接口的 IID 的引用，通常IID_IStream。 此对象封装 randomAccessStream。</param>
        /// <param name="ppv">此方法成功返回时，包含 riid 中请求的接口指针，通常为 IStream。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [LibraryImport(ShCore, EntryPoint = "CreateStreamOverRandomAccessStream", SetLastError = false), PreserveSig]
        public static partial int CreateStreamOverRandomAccessStream(nint randomAccessStream, Guid riid, [MarshalAs(UnmanagedType.Interface)] out IStream ppv);
    }
}
