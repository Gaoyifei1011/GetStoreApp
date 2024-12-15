using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.System;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供对应用包标识的访问权限。
    /// </summary>
    [GeneratedComInterface, Guid("2256999D-D617-42F1-880E-0BA4542319D5")]
    public partial interface IAppxManifestPackageId2 : IAppxManifestPackageId
    {
        /// <summary>
        /// 获取清单中定义的处理器体系结构。
        /// </summary>
        /// <param name="architecture">为包指定的体系结构。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetArchitecture2(out ProcessorArchitecture architecture);
    }
}
