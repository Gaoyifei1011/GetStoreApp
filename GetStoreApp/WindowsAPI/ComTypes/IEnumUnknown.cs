using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 枚举具有 IUnknown 接口的对象。 它可用于枚举包含多个对象的组件中的对象。
    /// </summary>
    [GeneratedComInterface, Guid("00000100-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IEnumUnknown
    {
        /// <summary>
        /// 检索枚举序列中指定数量的项。
        /// </summary>
        /// <param name="celt">要检索的项数。 如果序列中剩余的项数少于请求的项数，此方法将检索剩余的元素。</param>
        /// <param name="rgelt">
        /// 枚举项的数组。
        /// 枚举器负责调用 AddRef，调用方负责通过枚举的每个指针调用 Release 。 如果 celt 大于 1，则调用方还必须传递传递给 pceltFetched 的非 NULL 指针，以了解要释放的指针数。
        /// </param>
        /// <param name="celtFetched">检索到的项数。 此参数始终小于或等于请求的项数。</param>
        /// <returns>如果 方法检索请求的项数，则返回值S_OK。 否则，它将S_FALSE。</returns>
        [PreserveSig]
        int Next(int celt, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface)] object[] rgelt, ref int celtFetched);

        /// <summary>
        /// 跳过枚举序列中指定数量的项。
        /// </summary>
        /// <param name="celt">要跳过的项数。</param>
        /// <returns>如果方法跳过请求的项数，则返回值S_OK。 否则，S_FALSE。</returns>
        [PreserveSig]
        int Skip(int celt);

        /// <summary>
        /// 将枚举序列重置到开头。
        /// </summary>
        /// <returns>返回值为S_OK。</returns>
        [PreserveSig]
        int Reset();

        /// <summary>
        /// 创建与当前枚举数包含相同枚举状态的一个新枚举数。
        /// 使用此方法可记录枚举序列中的点，以便稍后返回该点。 调用方必须将这个新枚举器与第一个枚举器分开发布。
        /// </summary>
        /// <param name="ppenum">指向克隆的枚举器对象的指针。</param>
        /// <returns>此方法可以返回标准返回值E_INVALIDARG、E_OUTOFMEMORY、E_UNEXPECTED和S_OK。</returns>
        [PreserveSig]
        int Clone(out IEnumUnknown ppenum);
    }
}
