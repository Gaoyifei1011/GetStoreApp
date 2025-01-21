using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供对应用程序的属性值的访问。
    /// </summary>
    [GeneratedComInterface, Guid("5DA89BF4-3773-46BE-B650-7E744863B7E8")]
    public partial interface IAppxManifestApplication
    {
        /// <summary>
        /// 获取清单的应用程序元数据部分中元素或属性的字符串值。
        /// </summary>
        /// <param name="name">要从应用程序元数据获取的元素或属性值的名称。</param>
        /// <param name="value">请求的元素或特性的值。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetStringValue([MarshalAs(UnmanagedType.LPWStr)] string name, [MarshalAs(UnmanagedType.LPWStr)] out string value);

        /// <summary>
        /// 获取应用程序用户模型标识符。
        /// </summary>
        /// <param name="appUserModelId">用户模型标识符。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetAppUserModelId([MarshalAs(UnmanagedType.LPWStr)] out string appUserModelId);
    }
}
