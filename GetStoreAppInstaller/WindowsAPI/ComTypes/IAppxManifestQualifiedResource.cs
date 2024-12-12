using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace WindowsTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// 为捆绑包清单中的 <Resource> 元素提供只读对象模型。
    /// </summary>
    [GeneratedComInterface, Guid("3B53A497-3C5C-48D1-9EA3-BB7EAC8CD7D4")]
    public partial interface IAppxManifestQualifiedResource
    {
        /// <summary>
        /// 检索资源的语言。
        /// </summary>
        /// <param name="language">包含资源语言的字符串。</param>
        /// <returns>如果此方法成功，则返回 S_OK。否则，它将返回 HRESULT 错误代码。</returns>
        int GetLanguage([MarshalAs(UnmanagedType.LPWStr)] out string language);

        /// <summary>
        /// 检索资源的大小。
        /// </summary>
        /// <param name="scale">资源的规模（以字节为单位）。</param>
        /// <returns>如果此方法成功，则返回 S_OK。否则，它将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetScale(out uint scale);

        /// <summary>
        /// 从清单的 Resources\Resource 字段中检索资源的 DirectX 功能级别。
        /// </summary>
        /// <param name="dxFeatureLevel">一个指向变量的指针，该变量接收指定资源的 DirectX 功能级别的 DX_FEATURE_LEVEL 类型值。</param>
        /// <returns>如果此方法成功，则返回 S_OK。否则，它将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetDXFeatureLevel(out uint dxFeatureLevel);
    }
}
