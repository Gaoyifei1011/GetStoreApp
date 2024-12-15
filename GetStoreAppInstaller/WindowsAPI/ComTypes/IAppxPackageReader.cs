using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 为应用包提供只读对象模型。
    /// </summary>
    [GeneratedComInterface, Guid("B5C49650-99BC-481C-9A34-3D53A4106708")]
    public partial interface IAppxPackageReader
    {
        /// <summary>
        /// 检索包的块映射对象模型。
        /// </summary>
        /// <param name="blockMapReader">包的块映射的对象模型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetBlockMap(out IntPtr blockMapReader);

        /// <summary>
        /// 从包中检索占用文件。
        /// </summary>
        /// <param name="type">要检索的占用文件的类型。</param>
        /// <param name="file">对应于类型为的占用文件的文件对象。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetFootprintFile(APPX_FOOTPRINT_FILE_TYPE type, [MarshalAs(UnmanagedType.Interface)] out IAppxFile file);

        /// <summary>
        /// 从包中检索有效负载文件。
        /// </summary>
        /// <param name="fileName">要检索的有效负载文件的名称。</param>
        /// <param name="file">对应于 fileName 的文件对象。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPayloadFile([MarshalAs(UnmanagedType.LPWStr)] string fileName, [MarshalAs(UnmanagedType.Interface)] out IAppxFile file);

        /// <summary>
        /// 检索循环访问包中的有效负载文件的枚举器。
        /// </summary>
        /// <param name="filesEnumerator">包中所有有效负载文件的枚举器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPayloadFiles([MarshalAs(UnmanagedType.Interface)] out IAppxFilesEnumerator filesEnumerator);

        /// <summary>
        /// 检索包的应用清单的对象模型。
        /// </summary>
        /// <param name="manifestReader">应用清单的对象模型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetManifest([MarshalAs(UnmanagedType.Interface)] out IAppxManifestReader manifestReader);
    }
}
