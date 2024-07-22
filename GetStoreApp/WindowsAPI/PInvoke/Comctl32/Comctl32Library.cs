using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Comctl32
{
    /// <summary>
    /// Comctl32.dll 函数库
    /// </summary>
    public static partial class Comctl32Library
    {
        private const string Comctl32 = "comctl32.dll";

        /// <summary>
        /// 安装或更新窗口子类回调。
        /// </summary>
        /// <param name="hWnd">正在子类化的窗口的句柄。</param>
        /// <param name="pfnSubclass">指向窗口过程的指针。 此指针和子类 ID 唯一标识此子类回调。 有关回调函数原型，请参阅 SUBCLASSPROC。</param>
        /// <param name="uIdSubclass">子类 ID。 此 ID 与子类过程一起唯一标识子类。 若要删除子类，请将子类过程和此值传递给 RemoveWindowSubclass 函数。 此值将传递给 uIdSubclass 参数中的子类过程。</param>
        /// <param name="dwRefData">用于 引用数据的DWORD_PTR。 此值的含义由调用应用程序确定。 此值将传递给 dwRefData 参数中的子类过程。 不同的 dwRefData 与窗口句柄、子类过程和 uIdSubclass 的每个组合相关联。</param>
        /// <returns>如果成功安装子类回调，则为 TRUE;否则为 FALSE。</returns>
        [LibraryImport(Comctl32, EntryPoint = "SetWindowSubclass", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetWindowSubclass(IntPtr hWnd, IntPtr pfnSubclass, uint uIdSubclass, IntPtr dwRefData);

        /// <summary>
        /// 在窗口的子类链中调用下一个处理程序。 子类链中的最后一个处理程序调用窗口的原始窗口过程。
        /// </summary>
        /// <param name="hWnd">正在子类化的窗口的句柄。</param>
        /// <param name="uMsg">一个 unsigned int 类型的值，该值指定窗口消息。</param>
        /// <param name="wParam">指定附加消息信息。 此参数的内容取决于窗口消息的值。</param>
        /// <param name="lParam">指定附加消息信息。 此参数的内容取决于窗口消息的值。 注意：在 64 位版本的 Windows LPARAM 上是一个 64 位值。</param>
        /// <returns>返回的值特定于发送的消息。 应忽略此值。</returns>
        [LibraryImport(Comctl32, EntryPoint = "DefSubclassProc", SetLastError = false)]
        public static partial IntPtr DefSubclassProc(IntPtr hWnd, WindowMessage uMsg, UIntPtr wParam, IntPtr lParam);
    }
}
