using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 获取一个枚举器，该枚举器循环访问清单中定义的应用程序。
    /// </summary>
    [GeneratedComInterface, Guid("9EB8A55A-F04B-4D0D-808D-686185D4847A")]
    public partial interface IAppxManifestApplicationsEnumerator
    {
        /// <summary>
        /// 获取枚举器当前位置处的应用程序。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetCurrent([MarshalAs(UnmanagedType.Interface)] out IAppxManifestApplication application);

        /// <summary>
        /// 确定枚举器的当前位置是否有应用程序。
        /// </summary>
        /// <param name="hasCurrent">如果枚举器的当前位置引用项，则为 TRUE;如果枚举器已传递集合中的最后一项，则为 FALSE。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetHasCurrent([MarshalAs(UnmanagedType.Bool)] out bool hasCurrent);

        /// <summary>
        /// 将枚举器的位置推进到下一个应用程序。
        /// </summary>
        /// <param name="hasNext">如果枚举器成功前进，则为 TRUE，如果枚举器已通过集合的末尾，则为 FALSE。</param>
        /// <returns>
        /// 如果该方法成功，则返回 S_OK。 否则，它将返回错误代码。
        /// 请注意，当枚举器首次通过集合末尾时， hasNext = FALSE，但该方法成功并返回 S_OK。 但是，如果随后在集合结束之后调用另一个 MoveNext，并且之前收到 hasNext = FALSE，则该方法将返回E_BOUNDS。
        /// </returns>
        [PreserveSig]
        int MoveNext([MarshalAs(UnmanagedType.Bool)] out bool hasNext);
    }
}
