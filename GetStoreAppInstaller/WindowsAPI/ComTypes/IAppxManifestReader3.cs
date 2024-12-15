using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 表示包清单的对象模型，该模型提供用于访问清单元素和属性的方法。
    /// </summary>
    [GeneratedComInterface, Guid("C43825AB-69B7-400A-9709-CC37F5A72D24")]
    public partial interface IAppxManifestReader3 : IAppxManifestReader2
    {
        /// <summary>
        /// 获取一个枚举器，该枚举器循环访问清单中定义的功能。
        /// </summary>
        /// <param name="capabilityClass">功能类型。</param>
        /// <param name="capabilities">迭代 capabilities 的枚举器。</param>
        /// <returns>如果此方法成功，则返回 S_OK。否则，它将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetCapabilitiesByCapabilityClass(APPX_CAPABILITY_CLASS_TYPE capabilityClass, [MarshalAs(UnmanagedType.Interface)] out IAppxManifestCapabilitiesEnumerator capabilities);

        /// <summary>
        /// 获取一个枚举器，该枚举器循环访问清单中定义的目标设备系列。
        /// </summary>
        /// <param name="targetDeviceFamilies">循环访问目标设备系列的枚举器。</param>
        /// <returns>如果此方法成功，则返回 S_OK。否则，它将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetTargetDeviceFamilies([MarshalAs(UnmanagedType.Interface)] out IAppxManifestTargetDeviceFamiliesEnumerator targetDeviceFamilies);
    }
}
