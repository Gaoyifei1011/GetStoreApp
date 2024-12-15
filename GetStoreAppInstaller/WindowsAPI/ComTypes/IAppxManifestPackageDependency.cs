using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 描述一个包在另一个包上的依赖关系。
    /// </summary>
    [GeneratedComInterface, Guid("E4946B59-733E-43F0-A724-3BDE4C1285A0")]
    public partial interface IAppxManifestPackageDependency
    {
        /// <summary>
        /// 获取当前包具有依赖项的包的名称。
        /// </summary>
        /// <param name="name">包的名称。</param>
        /// <returns>如果方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetName([MarshalAs(UnmanagedType.LPWStr)] out string name);

        /// <summary>
        /// 获取生成当前包所依赖的包的发布者的名称。
        /// </summary>
        /// <param name="publisher">发行者的名称。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPublisher([MarshalAs(UnmanagedType.LPWStr)] out string publisher);

        /// <summary>
        /// 获取当前包具有依赖项的包的最低版本。
        /// </summary>
        /// <param name="minVersion">包的最低版本。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetMinVersion(out ulong minVersion);
    }
}
