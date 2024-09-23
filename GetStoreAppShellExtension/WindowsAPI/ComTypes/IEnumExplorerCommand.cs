using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppShellExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// 由 IExplorerCommandProvider 提供。 此接口包含要放入命令栏的命令枚举。
    /// </summary>
    [GeneratedComInterface, Guid("A88826F8-186F-4987-AADE-EA0CEF8FBFE8")]
    public partial interface IEnumExplorerCommand
    {
        /// <summary>
        /// 检索指定数量的直接跟随当前元素的元素。
        /// </summary>
        /// <param name="celt">指定要提取的元素数。</param>
        /// <param name="pUICommand">celt 元素的 IExplorerCommand 接口指针数组的地址，此方法返回时，这些元素是指向检索到的元素的指针数组。</param>
        /// <param name="pceltFetched">此方法返回时，包含指向实际检索的元素数的指针。 如果不需要此信息，则此指针可以为 NULL 。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Next(uint celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0), Out] IExplorerCommand[] pUICommand, out uint pceltFetched);

        /// <summary>
        /// 目前尚未实现。
        /// </summary>
        /// <param name="celt">当前未使用。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Skip(uint celt);

        /// <summary>
        /// 将枚举重置为 0。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Reset();

        /// <summary>
        /// 目前尚未实现。
        /// </summary>
        /// <param name="ppenum">当前未使用。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Clone([MarshalAs(UnmanagedType.Interface)] out IEnumExplorerCommand ppenum);
    }
}
