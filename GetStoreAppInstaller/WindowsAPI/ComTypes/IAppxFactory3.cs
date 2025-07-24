using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("776B2C05-E21D-4E24-BA1A-CD529A8BFDBB")]
    public partial interface IAppxFactory3
    {
        /// <summary>
        /// 使用可选参数从 IStream 提供的内容创建只读包读取器，用于指定包的预期摘要。 此方法不验证数字签名。
        /// </summary>
        /// <param name="inputStream">提供用于读取的包的输入流。 流必须支持 ISequentialStream：：Read、 IStream：：Seek 和 IStream：：Stat。如果这些方法失败，则其错误代码可能会传递到此方法并由此方法返回。</param>
        /// <param name="expectedDigest">包含预期摘要的 LPCWSTR，包文件的哈希表示形式。</param>
        /// <param name="packageReader">创建的包读取器。</param>
        /// <returns>如果该方法成功，则它会返回 S_OK。 否则，它将返回一个错误代码。</returns>
        [PreserveSig]
        int CreatePackageReader2([MarshalAs(UnmanagedType.Interface)] IStream inputStream, [MarshalAs(UnmanagedType.LPWStr)] string expectedDigest, [MarshalAs(UnmanagedType.Interface)] out IAppxPackageReader packageReader);

        /// <summary>
        /// 使用可选参数根据 IStream 提供的内容创建只读清单对象模型，用于指定清单的预期摘要。
        /// </summary>
        /// <param name="inputStream">传递清单 XML 以供读取的输入流。 流必须支持 ISequentialStream：：Read、 IStream：：Seek 和 IStream：：Stat。如果这些方法失败，则其错误代码可能会传递给此方法并由此方法返回。</param>
        /// <param name="expectedDigest">包含预期摘要的 LPCWSTR，清单文件的哈希表示形式。</param>
        /// <param name="manifestReader">创建的清单读取器。</param>
        /// <returns>如果该方法成功，则它会返回 S_OK。 否则，它将返回一个错误代码。</returns>
        [PreserveSig]
        int CreateManifestReader2([MarshalAs(UnmanagedType.Interface)] IStream inputStream, [MarshalAs(UnmanagedType.LPWStr)] string expectedDigest, [MarshalAs(UnmanagedType.Interface)] out IAppxManifestReader manifestReader);

        /// <summary>
        /// 使用可选参数创建 IAppInstallerReader 的实例，用于指定应用安装程序文件的预期摘要。
        /// </summary>
        /// <param name="inputStream">提供应用安装程序文件内容的 IStream 。</param>
        /// <param name="expectedDigest">包含预期摘要的 LPCWSTR，应用安装程序文件的哈希表示形式。</param>
        /// <param name="appInstallerReader">接收创建的 IAppInstallerReader 实例。</param>
        /// <returns>如果该方法成功，则它会返回 S_OK。 否则，它将返回一个错误代码。</returns>
        [PreserveSig]
        int CreateAppInstallerReader([MarshalAs(UnmanagedType.Interface)] IStream inputStream, [MarshalAs(UnmanagedType.LPWStr)] string expectedDigest, [MarshalAs(UnmanagedType.Interface)] out nint appInstallerReader);
    }
}
