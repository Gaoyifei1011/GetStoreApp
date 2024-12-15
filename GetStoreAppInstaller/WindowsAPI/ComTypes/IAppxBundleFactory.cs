using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 创建用于读取和写入捆绑包的对象。
    /// </summary>
    [GeneratedComInterface, Guid("BBA65864-965F-4A5F-855F-F074BDBF3A7B")]
    public partial interface IAppxBundleFactory
    {
        /// <summary>
        /// 创建可向其添加应用包的只写捆绑包对象。
        /// </summary>
        /// <param name="outputStream">接收序列化包数据的输出流。 流必须至少支持 Write 方法。</param>
        /// <param name="bundleVersion">
        /// 捆绑包的版本号。
        /// 如果设置为 0，CreateBundleWriter 会将捆绑包的版本号设置为派生自当前系统时间的值。 建议传递 0，以便自动生成版本号，并且每个连续调用都会生成更高的版本号。
        /// 例如，如果在 2013/12/23 AM UTC 3：45：00 AM UTC 调用 CreateBundleWriter ，并将 bundleVersion 设置为 0，则捆绑包的版本号变为 2013.1223.0345.0000。
        /// </param>
        /// <param name="bundleWriter">此方法创建的捆绑编写器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int CreateBundleWriter([MarshalAs(UnmanagedType.Interface)] IStream outputStream, ulong bundleVersion, out IntPtr bundleWriter);

        /// <summary>
        /// 创建一个只读捆绑包对象，该对象从 IStream 对象读取其内容。
        /// </summary>
        /// <param name="inputStream">提供包内容以供读取的输入流。 流必须支持 Read、 Seek 和 Stat。如果这些方法失败，则其错误代码可能会传递到此方法并由此方法返回。</param>
        /// <param name="bundleReader">捆绑包读取器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int CreateBundleReader([MarshalAs(UnmanagedType.Interface)] IStream inputStream, [MarshalAs(UnmanagedType.Interface)] out IAppxBundleReader bundleReader);

        /// <summary>
        /// 创建从独立流到 AppxBundleManifest.xml 的只读捆绑清单对象。
        /// </summary>
        /// <param name="inputStream">传递清单 XML 以供读取的输入流。 流必须支持 Read、 Seek 和 Stat。如果这些方法失败，则其错误代码可能会传递给此方法并由此方法返回。</param>
        /// <param name="manifestReader">清单读取器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int CreateBundleManifestReader([MarshalAs(UnmanagedType.Interface)] IStream inputStream, [MarshalAs(UnmanagedType.Interface)] out IAppxBundleManifestReader manifestReader);
    }
}
