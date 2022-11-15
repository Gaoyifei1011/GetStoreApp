using System.Runtime.InteropServices;

namespace GetStoreAppWindowsAPI.PInvoke.NTdll
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_BASIC_INFORMATION
    {
        /// <summary>
        /// 可以强制转换为 DWORD ，并包含 GetProcessAffinityMask 为 lpProcessAffinityMask 参数返回的相同值。
        /// </summary>
        public int AffinityMask;

        /// <summary>
        /// 包含计划优先级中所述的进程优先级。
        /// </summary>
        public int BasePriority;

        /// <summary>
        /// 包含 GetExitCodeProcess 返回的相同值。
        /// </summary>
        public int ExitStatus;

        /// <summary>
        /// 可以强制转换为 DWORD ，并包含父进程的唯一标识符。
        /// </summary>
        public int InheritedFromUniqueProcessId;

        /// <summary>
        /// 指向 PEB 结构。
        /// </summary>
        public int PebBaseAddress;

        /// <summary>
        /// 可以强制转换为 DWORD ，并包含此过程的唯一标识符。 建议使用 GetProcessId 函数检索此信息。
        /// </summary>
        public int UniqueProcessId;

        public int Size
        {
            get
            {
                return (6 * 4);
            }
        }
    };
}
