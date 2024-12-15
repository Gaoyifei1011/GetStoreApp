using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.System;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供对包标识的访问权限。
    /// </summary>
    [GeneratedComInterface, Guid("283CE2D7-7153-4A91-9649-7A0F7240945F")]
    public partial interface IAppxManifestPackageId
    {
        /// <summary>
        /// 获取清单中定义的包的名称。
        /// </summary>
        /// <param name="name">包的名称。</param>
        /// <returns>如果方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetName([MarshalAs(UnmanagedType.LPWStr)] out string name);

        /// <summary>
        /// 为包指定的体系结构。
        /// </summary>
        /// <param name="architecture">如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</param>
        /// <returns>
        /// 处理器体系结构信息是使用包清单中 Identity 元素的 ProcessorArchitecture 属性指定的。
        /// 如果未在清单中定义体系结构，此方法将返回 APPX_PACKAGE_ARCHITECTURE 枚举的 APPX_PACKAGE_ARCHITECTURE_NEUTRAL 值。
        /// </returns>
        [PreserveSig]
        int GetArchitecture(out ProcessorArchitecture architecture);

        /// <summary>
        /// 获取清单中定义的包发布者的名称。
        /// </summary>
        /// <param name="publisher">包的发布者。</param>
        /// <returns>如果方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetPublisher([MarshalAs(UnmanagedType.LPWStr)] out string publisher);

        /// <summary>
        /// 获取清单中定义的包的版本。
        /// </summary>
        /// <param name="packageVersion">包的版本。</param>
        /// <returns>如果该方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetVersion(out ulong packageVersion);

        /// <summary>
        /// 获取清单中定义的包资源标识符。
        /// </summary>
        /// <param name="resourceId">包的资源标识符。</param>
        /// <returns>如果该方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetResourceId([MarshalAs(UnmanagedType.LPWStr)] out string resourceId);

        /// <summary>
        /// 将指定的发布服务器与清单中定义的发布服务器进行比较。
        /// </summary>
        /// <param name="other">要比较的发布者名称。</param>
        /// <param name="isSame">如果指定的发布服务器与包发布者匹配，则为 TRUE;否则为 FALSE。</param>
        /// <returns>如果方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int ComparePublisher([MarshalAs(UnmanagedType.LPWStr)] string other, [MarshalAs(UnmanagedType.Bool)] out bool isSame);

        /// <summary>
        /// 获取包全名。
        /// </summary>
        /// <param name="packageFullName">包全名。</param>
        /// <returns>如果方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetPackageFullName([MarshalAs(UnmanagedType.LPWStr)] out string packageFullName);

        /// <summary>
        /// 获取包系列名称。
        /// </summary>
        /// <param name="packageFamilyName">包系列名称。</param>
        /// <returns>如果方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetPackageFamilyName([MarshalAs(UnmanagedType.LPWStr)] out string packageFamilyName);
    }
}
