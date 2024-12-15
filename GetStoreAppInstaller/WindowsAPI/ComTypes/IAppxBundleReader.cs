using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 为捆绑包提供只读对象模型。
    /// </summary>
    [GeneratedComInterface, Guid("DD75B8C0-BA76-43B0-AE0F-68656A1DC5C8")]
    public partial interface IAppxBundleReader
    {
        /// <summary>
        /// 从捆绑中检索指定类型的占用文件。
        /// </summary>
        /// <param name="fileType">要检索的占用空间文件的类型。</param>
        /// <param name="footprintFile">对应于 fileType 的占用空间文件的文件对象。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetFootprintFile(uint fileType, [MarshalAs(UnmanagedType.Interface)] out IAppxFile footprintFile);

        /// <summary>
        /// 从捆绑包中检索只读块映射对象。
        /// </summary>
        /// <param name="blockMapReader">捆绑包中包的块映射的对象模型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetBlockMap(out IntPtr blockMapReader);

        /// <summary>
        /// 从捆绑中检索只读清单对象。
        /// </summary>
        /// <param name="manifestReader">捆绑清单的对象模型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetManifest([MarshalAs(UnmanagedType.Interface)] out IAppxBundleManifestReader manifestReader);

        /// <summary>
        /// 检索循环访问捆绑包中所有有效负载包列表的枚举器。
        /// </summary>
        /// <param name="payloadPackages">捆绑包中所有有效负载包的枚举器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPayloadPackages([MarshalAs(UnmanagedType.Interface)] out IAppxFilesEnumerator payloadPackages);

        /// <summary>
        /// 检索具有指定文件名的有效负载包的 appx 文件对象。
        /// </summary>
        /// <param name="fileName">要检索的有效负载文件的名称。</param>
        /// <param name="payloadPackage">与 fileName 对应的有效负载文件对象。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPayloadPackage([MarshalAs(UnmanagedType.LPWStr)] string fileName, out IAppxFile payloadPackage);
    }
}
