using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 从 AppxManifest.xml 检索有关目标设备系列的信息。
    /// </summary>
    [GeneratedComInterface, Guid("9091B09B-C8D5-4F31-8687-A338259FAEFB")]
    public partial interface IAppxManifestTargetDeviceFamily
    {
        /// <summary>
        /// 从 AppxManifest.xml 获取目标设备系列的名称。
        /// </summary>
        /// <param name="name">目标设备系列名称。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetName([MarshalAs(UnmanagedType.LPWStr)] out string name);

        /// <summary>
        /// 从 AppxManifest.xml 获取目标设备系列的最低版本。
        /// </summary>
        /// <param name="minVersion">最低版本。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetMinVersion(out ulong minVersion);

        /// <summary>
        /// 从 AppxManifest.xml 获取测试的最高版本。
        /// </summary>
        /// <param name="maxVersionTested">测试的最大版本属性。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetMaxVersionTested(out ulong maxVersionTested);
    }
}
