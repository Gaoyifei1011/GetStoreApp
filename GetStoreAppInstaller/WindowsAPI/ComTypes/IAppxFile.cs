using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 检索有关包中有效负载或占用空间文件的信息。
    /// </summary>
    [GeneratedComInterface, Guid("91DF827B-94FD-468F-827B-57F41B2F6F2E")]
    public partial interface IAppxFile
    {
        /// <summary>
        /// 检索用于在包中存储文件的压缩选项。
        /// </summary>
        /// <param name="compressionOption">描述文件在包中的存储方式的压缩选项。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetCompressionOption(out uint compressionOption);

        /// <summary>
        /// 检索文件的内容类型。
        /// </summary>
        /// <param name="contentType">文件的内容类型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetContentType([MarshalAs(UnmanagedType.LPWStr)] out string contentType);

        /// <summary>
        /// 检索文件的名称，包括其相对于包根目录的路径。
        /// </summary>
        /// <param name="fileName">包含文件的名称和相对路径的字符串。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetName([MarshalAs(UnmanagedType.LPWStr)] out string fileName);

        /// <summary>
        /// 检索文件的未压缩大小。
        /// </summary>
        /// <param name="size">未压缩的大小（以字节为单位）。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetSize(out ulong size);

        /// <summary>
        /// 获取包含文件未压缩内容的只读流。
        /// </summary>
        /// <param name="stream">一个只读流，其中包含文件的未压缩内容。</param>
        /// <returns>如果方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetStream([MarshalAs(UnmanagedType.Interface)] out IStream stream);
    }
}
