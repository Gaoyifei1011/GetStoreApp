using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 表示包清单的对象模型，该模型提供用于访问清单元素和属性的方法。
    /// </summary>
    [GeneratedComInterface, Guid("D06F67BC-B31D-4EBA-A8AF-638E73E77B4D")]
    public partial interface IAppxManifestReader2 : IAppxManifestReader
    {
        /// <summary>
        /// 获取一个枚举器，该枚举器循环访问清单中定义的限定资源。
        /// </summary>
        /// <param name="resources">循环访问限定资源的枚举器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetQualifiedResources([MarshalAs(UnmanagedType.Interface)] out IAppxManifestQualifiedResourcesEnumerator resources);
    }
}
