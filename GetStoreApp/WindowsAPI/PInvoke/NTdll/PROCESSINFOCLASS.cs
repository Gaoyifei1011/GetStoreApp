namespace GetStoreApp.WindowsAPI.PInvoke.NTdll
{
    public enum PROCESSINFOCLASS : int
    {
        /// <summary>
        /// 检索指向 PEB 结构的指针，该结构可用于确定是否正在调试指定的进程，以及系统用于标识指定进程的唯一值。
        /// </summary>
        ProcessBasicInformation = 0,

        /// <summary>
        /// 检索一个 DWORD_PTR 值，该值是进程的调试器的端口号。 非零值指示进程在环 3 调试器的控制下运行。
        /// </summary>
        ProcessDebugPort = 7,

        /// <summary>
        /// 确定进程是否在 WOW64 环境中运行， (WOW64 是 x86 模拟器，它允许基于 Win32 的应用程序在 64 位 Windows) 上运行。
        /// </summary>
        ProcessWow64Information = 26,

        /// <summary>
        /// 检索包含进程映像文件名称的 UNICODE_STRING 值。
        /// </summary>
        ProcessImageFileName = 27,

        /// <summary>
        /// 检索一个 ULONG 值，该值指示进程是否被视为关键。
        /// </summary>
        ProcessBreakOnTermination = 29,

        /// <summary>
        /// 检索指示进程的子系统类型的 SUBSYSTEM_INFORMATION_TYPE 值。ProcessInformation 参数指向的缓冲区应足够大，可以容纳单个SUBSYSTEM_INFORMATION_TYPE枚举。
        /// </summary>
        ProcessSubsystemInformation = 75,
    };
}
