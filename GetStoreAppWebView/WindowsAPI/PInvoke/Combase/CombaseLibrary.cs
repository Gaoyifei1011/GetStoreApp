using System;
using System.Runtime.InteropServices;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.Combase
{
    /// <summary>
    /// Combase.dll 函数库
    /// </summary>
    public static class CombaseLibrary
    {
        private const string Combase = "combase.dll";

        /// <summary>
        /// 获取指定运行时类的激活工厂。
        /// </summary>
        /// <param name="activatableClassId">可激活类的 ID。</param>
        /// <param name="iid">接口的引用 ID。</param>
        /// <param name="factory">激活工厂。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [DllImport(Combase, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "RoGetActivationFactory", SetLastError = false), PreserveSig]
        public static extern int RoGetActivationFactory(IntPtr activatableClassId, Guid iid, out IntPtr factory);

        /// <summary>
        /// 基于指定的源字符串创建新的 HSTRING 。
        /// </summary>
        /// <param name="sourceString">以 null 结尾的字符串，用作新 HSTRING 的源。 若要创建新的、空或 NULL 字符串，请为 sourceString 传递 NULL，为长度传递 0。</param>
        /// <param name="length">sourceString 的长度，以 Unicode 字符为单位。 如果 sourceString 为 NULL，则必须为 0。</param>
        /// <param name="hstring">指向新创建的 HSTRING 的指针;如果发生错误，则为 NULL 。 字符串中的任何现有内容将被覆盖。 HSTRING 是标准句柄类型。</param>
        /// <returns>如果该函数成功，则返回值为零值。如果函数失败，则返回值为非零值。</returns>
        [DllImport(Combase, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "WindowsCreateString", SetLastError = false), PreserveSig]
        public static extern int WindowsCreateString([MarshalAs(UnmanagedType.LPWStr)] string sourceString, int length, out IntPtr hstring);
    }
}
