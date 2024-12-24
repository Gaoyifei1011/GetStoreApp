using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 创建用于读取和写入捆绑包的对象。
    /// </summary>
    [GeneratedComInterface, Guid("7325B83D-0185-42C4-82AC-BE34AB1A2A8A")]
    public partial interface IAppxBundleFactory2
    {
        /// <summary>
        /// 创建一个只读捆绑包对象，该对象从 IStream 对象读取其内容。
        /// </summary>
        /// <param name="inputStream">提供包内容以供读取的输入流。 流必须支持 Read、 Seek 和 Stat。如果这些方法失败，则其错误代码可能会传递到此方法并由此方法返回。</param>
        /// <param name="expectedDigest">包含预期摘要的 LPCWSTR，捆绑包文件的哈希表示形式。</param>
        /// <param name="bundleReader">捆绑包读取器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int CreateBundleReader2([MarshalAs(UnmanagedType.Interface)] IStream inputStream, [MarshalAs(UnmanagedType.LPWStr)] string expectedDigest, [MarshalAs(UnmanagedType.Interface)] out IAppxBundleReader bundleReader);
    }
}
