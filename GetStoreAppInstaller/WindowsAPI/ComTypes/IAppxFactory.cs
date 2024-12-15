using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 创建用于读取和写入应用包的对象。
    /// </summary>
    [GeneratedComInterface, Guid("BEB94909-E451-438B-B5A7-D79E767B75D8")]
    public partial interface IAppxFactory
    {
        /// <summary>
        /// 创建可向其添加文件的只写包对象。
        /// </summary>
        /// <param name="outputStream">接收序列化包数据的输出流。 流必须至少支持 Write 方法。</param>
        /// <param name="settings">此包的生产设置。</param>
        /// <param name="packageWriter">此方法创建的包编写器。</param>
        /// <returns>如果方法成功，则返回 S_OK。 否则，它将返回一个错误代码。</returns>
        [PreserveSig]
        int CreatePackageWriter([MarshalAs(UnmanagedType.Interface)] IStream outputStream, IntPtr settings, out IntPtr packageWriter);

        /// <summary>
        /// 从 IStream 提供的内容创建只读包读取器。 此方法不验证数字签名。
        /// </summary>
        /// <param name="inputStream">提供包内容以供读取的输入流。 流必须支持 Read、 Seek 和 Stat。如果这些方法失败，则其错误代码可能会传递到此方法并由此方法返回。</param>
        /// <param name="packageReader">包读取器。</param>
        /// <returns>如果方法成功，则返回 S_OK。 否则，它将返回一个错误代码。</returns>
        [PreserveSig]
        int CreatePackageReader([MarshalAs(UnmanagedType.Interface)] IStream inputStream, [MarshalAs(UnmanagedType.Interface)] out IAppxPackageReader packageReader);

        /// <summary>
        /// 根据 IStream 提供的内容创建只读清单对象模型。
        /// </summary>
        /// <param name="inputStream">提供清单 XML 以供读取的输入流。 流必须支持 Read、 Seek 和 Stat。如果这些方法失败，则其错误代码可能会传递到此方法并由此方法返回。</param>
        /// <param name="manifestReader">清单读取器。</param>
        /// <returns>如果方法成功，则返回 S_OK。 否则，它将返回一个错误代码。</returns>
        [PreserveSig]
        int CreateManifestReader([MarshalAs(UnmanagedType.Interface)] IStream inputStream, [MarshalAs(UnmanagedType.Interface)] out IAppxManifestReader2 manifestReader);

        /// <summary>
        /// 根据 IStream 提供的内容创建只读块映射对象模型。
        /// </summary>
        /// <param name="inputStream">传送块映射 XML 以供读取的流。 流必须支持 Read、 Seek 和 Stat。如果这些方法失败，则其错误代码可能会传递给此方法并由此方法返回。</param>
        /// <param name="blockMapReader">块映射读取器。</param>
        /// <returns>如果方法成功，则返回 S_OK。 否则，它将返回一个错误代码。</returns>
        [PreserveSig]
        int CreateBlockMapReader([MarshalAs(UnmanagedType.Interface)] IStream inputStream, out IntPtr blockMapReader);

        /// <summary>
        /// 根据 IStream 和数字签名提供的内容创建只读块映射对象模型。
        /// </summary>
        /// <param name="blockMapStream">传送块映射 XML 以供读取的流。 流必须支持 Read、 Seek 和 Stat。</param>
        /// <param name="signatureFileName">包含用于验证输入流内容的数字签名的文件。</param>
        /// <param name="blockMapReader">块映射读取器。</param>
        /// <returns>如果方法成功，则返回 S_OK。 否则，它将返回一个错误代码。</returns>
        [PreserveSig]
        int CreateValidatedBlockMapReader([MarshalAs(UnmanagedType.Interface)] IStream blockMapStream, [MarshalAs(UnmanagedType.LPWStr)] string signatureFileName, out IntPtr blockMapReader);
    }
}
