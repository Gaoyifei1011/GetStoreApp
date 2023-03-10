using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 包含有关新创建的进程及其主线程的信息。 它与 <see cref="Kernel32Library.CreateProcess">、CreateProcessAsUser、CreateProcessWithLogonW 或 CreateProcessWithTokenW 函数一起使用。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PROCESS_INFORMATION
    {
        /// <summary>
        /// 新创建的进程的句柄。 句柄用于在对进程对象执行操作的所有函数中指定进程。
        /// </summary>
        public IntPtr hProcess;

        /// <summary>
        /// 新创建的进程的主线程的句柄。 句柄用于在线程对象上执行操作的所有函数中指定线程。
        /// </summary>
        public IntPtr hThread;

        /// <summary>
        /// 可用于标识进程的值。 从创建进程到进程的所有句柄关闭并释放进程对象为止，该值有效;此时，可以重复使用标识符。
        /// </summary>
        public int dwProcessId;

        /// <summary>
        /// 可用于标识线程的值。 在线程创建到线程的所有句柄关闭且线程对象释放之前，该值有效;此时，可以重复使用标识符。
        /// </summary>
        public int dwThreadId;
    }
}
