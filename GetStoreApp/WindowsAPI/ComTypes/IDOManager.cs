using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("400E2D4A-1431-4C1A-A748-39CA472CFDB1")]
    public partial interface IDOManager
    {
        /// <summary>
        /// 创建新的下载。
        /// </summary>
        /// <param name="download">IDODownload 接口指针的地址。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int CreateDownload([MarshalAs(UnmanagedType.Interface)] out IDODownload download);

        /// <summary>
        /// 检索指向枚举器对象的接口指针，该对象用于枚举现有下载。
        /// </summary>
        /// <param name="category">
        /// 可选。 要用作枚举的类别的属性名称。 nullptr传递将检索所有现有下载。 支持将以下属性作为一个类别。
        /// DODownloadProperty_Id
        /// DODownloadProperty_Uri
        /// DODownloadProperty_ContentId
        /// DODownloadProperty_DisplayName
        /// DODownloadProperty_LocalPath
        /// </param>
        /// <returns>指向 IEnumUnknown 的接口指针的地址，用于枚举现有下载。 枚举器的内容取决于 类别的值。 枚举接口中包含的下载是以前由此函数的同一调用方创建的下载。</returns>
        [PreserveSig]
        int EnumDownloads(DODownloadProperty category, out IntPtr ppEnum);
    }
}
