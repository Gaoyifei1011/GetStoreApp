using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 枚举字符串。 LPWSTR 是一种类型，指示指向宽字符串或 Unicode 字符的零终止字符串的指针。
    /// </summary>
    [Guid("00000101-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IEnumString
    {
        /// <summary>
        /// 检索枚举序列中的指定项数。
        /// </summary>
        /// <param name="celt">要检索的项数。 如果序列中剩余的项数少于所请求的项数，此方法将检索剩余的元素。</param>
        /// <param name="rgelt">枚举项的数组。 枚举器负责分配任何内存，调用方负责释放它。 如果 celt 大于 1，则调用方还必须传递传递给 <param name="pceltFetched"> 的非 NULL 指针，以了解要释放的指针数。</param>
        /// <param name="pceltFetched">检索到的项数。 此参数始终小于或等于所请求的项数。</param>
        /// <returns>如果该方法检索请求的项数，则返回值S_OK。 否则，它S_FALSE。</returns>
        [PreserveSig]
        int Next(int celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0), Out] string[] rgelt, IntPtr pceltFetched);

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
        /// 创建与当前枚举数包含相同枚举状态的一个新枚举数。使用此方法可记录枚举序列中的点，以便稍后返回该点。 调用方必须将这个新枚举器与第一个枚举器分开发布。
        /// </summary>
        /// <param name="ppenum"></param>
        void Clone(out IEnumString ppenum);
    }
}
