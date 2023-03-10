using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 进程创建标志
    /// <see cref="Kernel32Library.CreateProcess">、CreateProcessAsUser、CreateProcessWithLogonW 和 CreateProcessWithTokenW
    /// 函数使用以下进程创建标志。 可以在任意组合中指定它们，
    /// </summary>
    [Flags]
    public enum CreateProcessFlags
    {
        /// <summary>
        /// 无任何标志
        /// </summary>
        None = 0x0,

        /// <summary>
        /// 与作业关联的进程的子进程不与作业相关联。
        /// 如果调用进程未与作业关联，则此常量不起作用。 如果调用进程与作业相关联，则作业必须设置 JOB_OBJECT_LIMIT_BREAKAWAY_OK 限制。
        /// </summary>
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,

        /// <summary>
        /// 新进程不会继承调用进程的错误模式。 相反，新进程会获取默认错误模式。
        /// 此功能对于禁用硬错误的多线程 shell 应用程序尤其有用。
        /// 默认行为是让新进程继承调用方的错误模式。 设置此标志会更改默认行为。
        /// </summary>
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,

        /// <summary>
        /// 新进程具有新控制台，而不是继承其父控制台， (默认) 。 有关详细信息，请参阅 “创建控制台”。
        /// 此标志不能与 DETACHED_PROCESS一起使用。
        /// </summary>
        CREATE_NEW_CONSOLE = 0x00000010,

        /// <summary>
        /// 新进程是新进程组的根进程。 进程组包括此根进程的子代的所有进程。 新进程组的进程标识符与进程标识符相同，该标识符在 lpProcessInformation 参数中返回。 GenerateConsoleCtrlEvent 函数使用进程组，以便向一组控制台进程发送 CTRL+BREAK 信号。
        /// 如果指定此标志，将为新进程组中的所有进程禁用 CTRL+C 信号。
        /// 如果使用 <see cref="CREATE_NEW_CONSOLE"/> 指定，则忽略此标志。
        /// </summary>
        CREATE_NEW_PROCESS_GROUP = 0x00000200,

        /// <summary>
        /// 此过程是一个在没有控制台窗口的情况下运行的控制台应用程序。 因此，未设置应用程序的控制台句柄。
        /// T如果应用程序不是控制台应用程序，或者将其与 <see cref="CREATE_NEW_CONSOLE"/> 或 <see cref="DETACHED_PROCESS"/> 一起使用，则忽略此标志。
        /// </summary>
        CREATE_NO_WINDOW = 0x08000000,

        /// <summary>
        /// 进程将作为受保护的进程运行。 系统限制对受保护进程和受保护进程的线程的访问。 有关如何与受保护进程交互的详细信息，请参阅 进程安全性和访问权限。
        /// 若要激活受保护的进程，二进制文件必须具有特殊的签名。 此签名由 Microsoft 提供，但目前不适用于非 Microsoft 二进制文件。 目前有四个受保护的进程：媒体基础、音频引擎、Windows错误报告和系统。 还必须对加载到这些二进制文件的组件进行签名。 多媒体公司可以利用前两个受保护的流程。 有关详细信息，请参阅 受保护的媒体路径概述。
        /// Windows Server 2003 和 Windows XP：不支持此值。
        /// </summary>
        CREATE_PROTECTED_PROCESS = 0x00040000,

        /// <summary>
        /// 允许调用方执行绕过通常自动应用于进程的进程限制的子进程。
        /// </summary>
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,

        /// <summary>
        /// 此标志允许在Virtualization-Based安全环境中运行的安全进程启动。
        /// </summary>
        CREATE_SECURE_PROCESS = 0x00400000,

        /// <summary>
        /// 仅当启动基于 16 位Windows的应用程序时，此标志才有效。 如果已设置，新进程将在专用虚拟 DOS 计算机中运行 (VDM) 。 默认情况下，所有基于 16 位Windows的应用程序在单个共享 VDM 中作为线程运行。 单独运行的优点是，崩溃只会终止单个 VDM;在不同 VM 中运行的任何其他程序将继续正常运行。 此外，在单独的 VM 中运行的基于 16 位Windows的应用程序具有单独的输入队列。 这意味着，如果一个应用程序暂时停止响应，则单独的 VM 中的应用程序将继续接收输入。 单独运行的缺点是，需要更多内存才能执行此操作。 仅当用户请求 16 位应用程序应在自己的 VDM 中运行时，才应使用此标志。
        /// </summary>
        CREATE_SEPARATE_WOW_VDM = 0x00000800,

        /// <summary>
        /// 仅当启动基于 16 位Windows的应用程序时，标志才有效。 如果 WIN.INI Windows 部分中的 DefaultSeparateVDM 开关为 TRUE，则此标志将替代该开关。 新进程在共享虚拟 DOS 计算机中运行。
        /// </summary>
        CREATE_SHARED_WOW_VDM = 0x00001000,

        /// <summary>
        /// 新进程的主线程处于挂起状态创建，在调用 ResumeThread 函数之前不会运行。
        /// </summary>
        CREATE_SUSPENDED = 0x00000004,

        /// <summary>
        /// 如果设置了此标志， 则 lpEnvironment 指向的环境块使用 Unicode 字符。 否则，环境块使用 ANSI 字符。
        /// </summary>
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,

        /// <summary>
        /// 调用线程启动并调试新进程。 它可以使用 WaitForDebugEvent 函数接收所有相关的调试事件。
        /// </summary>
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,

        /// <summary>
        /// 调用线程启动并调试由新进程创建的新进程和所有子进程。 它可以使用 WaitForDebugEvent 函数接收所有相关的调试事件。
        /// 使用 <see cref="DEBUG_PROCESS"/> 的进程将成为调试链的根目录。 这一点一直持续到链中的另一个进程通过 <see cref="DEBUG_PROCESS"/> 创建。
        /// 如果此标志与 <see cref="DEBUG_ONLY_THIS_PROCESS"/>  结合使用，则调用方仅调试新进程，而不调试任何子进程。
        /// </summary>
        DEBUG_PROCESS = 0x00000001,

        /// <summary>
        /// 对于控制台进程，新进程不会 (默认) 继承其父级的控制台。 新进程稍后可以调用 <see cref="Kernel32Library.AllocConsole"> 函数来创建控制台。 有关详细信息，请参阅 “创建控制台”。
        /// 此值不能与 <see cref="CREATE_NEW_CONSOLE"/> 一起使用。
        /// </summary>
        DETACHED_PROCESS = 0x00000008,

        /// <summary>
        /// 此过程是使用扩展的启动信息创建的; lpStartupInfo 参数指定 STARTUPINFOEX 结构。
        /// Windows Server 2003 和 Windows XP：不支持此值。
        /// </summary>
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,

        /// <summary>
        /// 进程继承其父级的相关性。 如果父进程具有多个 处理器组中的线程，新进程将继承父组使用的任意组的组相对相关性。
        /// Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP：不支持此值。
        /// </summary>
        INHERIT_PARENT_AFFINITY = 0x00010000,
    }
}
