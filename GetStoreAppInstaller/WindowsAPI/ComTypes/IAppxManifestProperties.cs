using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供对包清单的属性部分的只读访问权限。
    /// </summary>
    [GeneratedComInterface, Guid("03FAF64D-F26F-4B2C-AAF7-8FE7789B8BCA")]
    public partial interface IAppxManifestProperties
    {
        /// <summary>
        /// 获取 properties 节中指定布尔元素的值。
        /// </summary>
        /// <param name="name">布尔元素的名称。</param>
        /// <param name="value">指定布尔元素的值。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetBoolValue([MarshalAs(UnmanagedType.LPWStr)] string name, [MarshalAs(UnmanagedType.Bool)] out bool value);

        /// <summary>
        /// 获取 properties 节中指定字符串元素的值。
        /// </summary>
        /// <param name="name">字符串元素的名称。</param>
        /// <param name="value">指定元素的值。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetStringValue([MarshalAs(UnmanagedType.LPWStr)] string name, [MarshalAs(UnmanagedType.LPWStr)] out string value);
    }
}
