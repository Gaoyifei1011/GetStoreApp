using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 为捆绑包清单提供只读对象模型。
    /// </summary>
    [GeneratedComInterface, Guid("CF0EBBC1-CC99-4106-91EB-E67462E04FB0")]
    public partial interface IAppxBundleManifestReader
    {
        /// <summary>
        /// 检索表示根 Bundle 元素下的 <Identity> 元素的 对象。
        /// </summary>
        /// <param name="packageId">包标识符。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPackageId([MarshalAs(UnmanagedType.Interface)] out IAppxManifestPackageId packageId);

        /// <summary>
        /// 检索对 Packages 元素下的所有 <Package> 元素的枚举器。
        /// </summary>
        /// <param name="packageInfoItems">捆绑包的 <Package> 元素中所有有效负载包的枚举器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPackageInfoItems([MarshalAs(UnmanagedType.Interface)] out IAppxBundleManifestPackageInfoEnumerator packageInfoItems);

        /// <summary>
        /// 获取不带任何预处理的原始 XML 文档。
        /// </summary>
        /// <param name="manifestStream">表示清单的 XML 内容的只读流。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetStream([MarshalAs(UnmanagedType.Interface)] out IStream manifestStream);
    }
}
