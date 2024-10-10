using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDODownload 接口用于启动和管理下载。
    /// </summary>
    [GeneratedComInterface, Guid("FBBD7FC0-C147-4727-A38D-827EF071EE77")]
    public partial interface IDODownload
    {
        /// <summary>
        /// 启动或恢复下载，将可选范围作为指向 DO_DOWNLOAD_RANGES_INFO 结构的指针传递。
        /// </summary>
        /// <param name="ranges">
        /// 可选。 指向 DO_DOWNLOAD_RANGES_INFO 结构的指针 (仅下载文件) 的特定范围。 此处传递的值指示下载是针对整个文件或文件中的部分范围，还是只是为了在不更改范围信息的情况下继续下载。 通过传递指向空DO_DOWNLOAD_RANGES_INFO结构的指针来指示下载整个文件的请求，即，其 RangeCount 成员设置为零。 通过传递指向非空DO_DOWNLOAD_RANGES_INFO结构的指针来指示下载部分文件的请求。 nullptr仅当之前已使用空/非空范围开始下载一次时，才允许传递，并且指示必须恢复下载而不对请求的范围进行任何更改。
        /// </param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int Start(IntPtr ranges);

        /// <summary>
        /// 暂停下载。
        /// </summary>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int Pause();

        /// <summary>
        /// 中止下载。
        /// </summary>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int Abort();

        /// <summary>
        /// 完成下载。 完成后，无法通过调用 “开始”来恢复下载。
        /// </summary>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int Finalize();

        /// <summary>
        /// 指向 DO_DOWNLOAD_STATUS 结构的指针。
        /// </summary>
        /// <param name="status">指向 DO_DOWNLOAD_STATUS 结构的指针。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int GetStatus(out DO_DOWNLOAD_STATUS status);

        /// <summary>
        /// 检索指向包含特定下载属性的 VARIANT 的指针。
        /// </summary>
        /// <param name="propId">获取 (DODownloadProperty) 类型所需的属性 ID。</param>
        /// <param name="propVal">生成的属性值，存储在 VARIANT 中。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int GetProperty(DODownloadProperty propId, out ComVariant propVal);

        /// <summary>
        /// 检索指向包含特定下载属性的 VARIANT 的指针。
        /// </summary>
        /// <param name="propId">要设置 (DODownloadProperty) 类型的所需的属性 ID。</param>
        /// <param name="propVal">要设置的属性值，存储在 VARIANT 中。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int SetProperty(DODownloadProperty propId, in ComVariant propVal);
    }
}
