using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 描述一个包在另一个包上的依赖项。
    /// </summary>
    [GeneratedComInterface, Guid("DDA0B713-F3FF-49D3-898A-2786780C5D98")]
    public partial interface IAppxManifestPackageDependency2 : IAppxManifestPackageDependency
    {
        /// <summary>
        /// 返回测试与当前包兼容的包的最大主版本号。
        /// </summary>
        /// <param name="maxMajorVersionTested">经测试与当前包兼容的依赖项包的最大主版本号。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetMaxMajorVersionTested(out ushort maxMajorVersionTested);
    }
}
