using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 要包含在快照中的系统部分 CreateToolhelp32Snapshot。
    /// </summary>
    [Flags]
    public enum CREATE_TOOLHELP32_SNAPSHOT_FLAGS : uint
    {
        /// <summary>
        /// 指示快照句柄是可继承的。
        /// </summary>
        TH32CS_INHERIT = 0x80000000,

        /// <summary>
        /// 包括快照中 th32ProcessID 中指定的进程的所有堆。
        ///  若要枚举堆，请参阅 Heap32ListFirst。
        /// </summary>
        TH32CS_SNAPHEAPLIST = 0x00000001,

        /// <summary>
        /// 在快照中包含系统中的所有进程。 若要枚举进程，请参阅 Process32First。
        /// </summary>
        TH32CS_SNAPPROCESS = 0x00000002,

        /// <summary>
        /// 在快照中包含系统中的所有线程。 若要枚举线程，请参阅 Thread32First。
        /// <para>
        /// 若要标识属于特定进程的线程，请在枚举线程时将其进程标识符与 THREADENTRY32 结构的 th32OwnerProcessID 成员进行比较。
        /// </para>
        /// </summary>
        TH32CS_SNAPTHREAD = 0x00000004,

        /// <summary>
        /// 包括快照中 th32ProcessID 中指定的进程的所有模块。
        /// 若要枚举模块，请参阅 Module32First。 如果函数失败 且ERROR_BAD_LENGTH，请重试该函数，直到函数成功。
        /// <para>
        /// 64 位 Windows： 在 32 位进程中使用此标志包括 第 32 个ProcessID 中指定的进程的 32 位模块，而在 64 位进程中使用它包括 64 位模块。 若要从 64 位进程包含 在 th32ProcessID 中指定的进程的 32 位模块，请使用 TH32CS_SNAPMODULE32 标志。
        /// </para>
        /// </summary>
        TH32CS_SNAPMODULE = 0x00000008,

        /// <summary>
        /// 从 64 位进程调用时，在快照中包含 在 th32ProcessID 中指定的进程的所有 32 位模块。
        /// 此标志可以与 TH32CS_SNAPMODULE or TH32CS_SNAPALL 结合使用。
        ///  如果函数失败 且ERROR_BAD_LENGTH，请重试该函数，直到函数成功。
        /// succeeds.
        /// </summary>
        TH32CS_SNAPMODULE32 = 0x00000010,

        /// <summary>
        /// 包括系统中的所有进程和线程，以及 th32ProcessID 中指定的进程的堆和模块。等效于使用 OR 操作 (“|”组合的 TH32CS_SNAPHEAPLIST、TH32CS_SNAPMODULE、TH32CS_SNAPPROCESS 和 TH32CS_SNAPTHREAD 值) 。
        /// </summary>
        TH32CS_SNAPALL = TH32CS_SNAPHEAPLIST | TH32CS_SNAPMODULE | TH32CS_SNAPPROCESS | TH32CS_SNAPTHREAD,
    }
}
