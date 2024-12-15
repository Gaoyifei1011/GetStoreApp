using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 为 捆绑包清单中的 <Package> 元素提供只读对象模型。
    /// </summary>
    [GeneratedComInterface, Guid("54CD06C1-268F-40BB-8ED2-757A9EBAEC8D")]
    public partial interface IAppxBundleManifestPackageInfo
    {
        /// <summary>
        /// 检索由包信息表示的包的类型。
        /// </summary>
        /// <param name="packageType">包的类型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPackageType(out APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE packageType);

        /// <summary>
        /// 检索表示应用包标识的对象。
        /// </summary>
        /// <param name="packageId">包标识符。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPackageId([MarshalAs(UnmanagedType.Interface)] out IAppxManifestPackageId packageId);

        /// <summary>
        /// 检索包的文件名属性。
        /// </summary>
        /// <param name="fileName">一个字符串，其中包含包的文件名。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string fileName);

        /// <summary>
        /// 检索包相对于捆绑包开头的偏移量。
        /// </summary>
        /// <param name="offset">包的偏移量（以字节为单位）。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetOffset(out ulong offset);

        /// <summary>
        /// 检索包的大小（以字节为单位）。
        /// </summary>
        /// <param name="size">包的大小（以字节为单位）。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetSize(out ulong size);

        /// <summary>
        /// 检索循环访问应用包清单中定义的所有 <Resource> 元素的枚举器。
        /// </summary>
        /// <param name="resources">循环访问资源的枚举器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetResources([MarshalAs(UnmanagedType.Interface)] out IAppxManifestQualifiedResourcesEnumerator resources);
    }
}
