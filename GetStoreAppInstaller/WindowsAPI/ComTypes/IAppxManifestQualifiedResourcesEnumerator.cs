using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace WindowsTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// 枚举程序包清单中定义的限定资源。
    /// </summary>
    [GeneratedComInterface, Guid("8EF6ADFE-3762-4A8F-9373-2FC5D444C8D2")]
    public partial interface IAppxManifestQualifiedResourcesEnumerator
    {
        /// <summary>
        /// 获取枚举器当前位置的资源。
        /// </summary>
        /// <param name="packageInfo">当前 <Resource> 元素。</param>
        /// <returns>如果方法成功，则返回 S_OK。 否则，它将返回一个错误代码。</returns>
        [PreserveSig]
        int GetCurrent([MarshalAs(UnmanagedType.Interface)] out IAppxManifestQualifiedResource packageInfo);

        /// <summary>
        /// 确定枚举器的当前位置是否有资源。
        /// </summary>
        /// <param name="hasCurrent">如果枚举器的当前位置引用项，则为 TRUE;如果枚举器已传递集合中的最后一项，则为 FALSE。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetHasCurrent([MarshalAs(UnmanagedType.Bool)] out bool hasCurrent);

        /// <summary>
        /// 将枚举器的位置推进到下一个资源。
        /// </summary>
        /// <param name="hasNext">如果枚举器成功前进，则为 TRUE,如果枚举器已通过集合的末尾，则为 FALSE。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，它将返回错误代码。</returns>
        [PreserveSig] int MoveNext([MarshalAs(UnmanagedType.Bool)] out bool hasNext);
    }
}
