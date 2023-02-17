using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 枚举名字对象或名字对象表中名字对象的组成部分。
    /// </summary>
    [Guid("00000102-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IEnumMoniker
    {
        /// <summary>
        /// 检索枚举序列中的指定项数。
        /// </summary>
        /// <param name="celt">要检索的项数。 如果序列中剩余的项数少于请求的项数，此方法将检索剩余的元素。</param>
        /// <param name="rgelt">枚举项的数组。 枚举器负责调用 AddRef，调用方负责通过枚举的每个指针调用 Release 。 如果 <param name="celt"> 大于 1，调用方还必须传递传递给 <param name="pceltFetched"> 的非 NULL 指针，以了解要释放的指针数。</param>
        /// <param name="pceltFetched">检索到的项数。 此参数始终小于或等于请求的项数。 如果 <param name="pceltFetched"> 为 1，则此参数可以为 NULL。</param>
        /// <returns></returns>
        [PreserveSig]
        int Next(int celt, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0), Out] IMoniker[] rgelt, IntPtr pceltFetched);

        /// <summary>
        /// 跳过枚举序列中指定数量的项。
        /// </summary>
        /// <param name="celt">要跳过的项数。</param>
        /// <returns>如果该方法跳过请求的项数，则返回值S_OK。 否则，它将S_FALSE。</returns>
        [PreserveSig]
        int Skip(int celt);

        /// <summary>
        /// 将枚举序列重置到开头。
        /// </summary>
        void Reset();

        /// <summary>
        /// 创建一个新的枚举器，其中包含与当前枚举状态相同的枚举状态。此方法可以记录枚举序列中的特定点，然后在以后返回到该点。调用方必须独立于第一个枚举器释放此新枚举器。
        /// </summary>
        /// <param name="ppenum">接收指向枚举对象的接口指针的 <see cref="IEnumMoniker"> 指针变量的地址。如果该方法不成功，则未定义此输出变量的值。</param>
        void Clone(out IEnumMoniker ppenum);
    }
}
