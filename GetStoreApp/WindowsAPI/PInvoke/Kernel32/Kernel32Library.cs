using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// Kernel32.dll 函数库
    /// </summary>
    public static partial class Kernel32Library
    {
        private const string Kernel32 = "kernel32.dll";

        public const int MAX_PATH = 260;

        public static nint INVALID_HANDLE_VALUE { get; } = new nint(-1);

        /// <summary>
        /// 获取指定进程的快照，以及这些进程使用的堆、模块和线程。
        /// </summary>
        /// <param name="dwFlags">要包含在快照中的系统部分。 此参数可使用以下一个或多个值。</param>
        /// <param name="th32ProcessID">
        /// 要包含在快照中的进程的进程标识符。 此参数可以为零以指示当前进程。 指定 TH32CS_SNAPHEAPLIST、 TH32CS_SNAPMODULE、 TH32CS_SNAPMODULE32 或 TH32CS_SNAPALL 值时，将使用此参数。 否则，它将被忽略，所有进程都包含在快照中。
        /// 如果指定的进程是空闲进程或 CSRSS 进程之一，则此函数将失败，并且最后一个错误代码 ERROR_ACCESS_DENIED ，因为它们的访问限制会阻止用户级代码打开它们。
        /// 如果指定的进程是 64 位进程，并且调用方是 32 位进程，则此函数将失败，最后一个错误代码 ERROR_PARTIAL_COPY( 299) 。
        /// </param>
        /// <returns>
        /// 如果函数成功，它将返回指定快照的打开句柄。如果函数失败，它将返回 INVALID_HANDLE_VALUE。 要获得更多的错误信息，请调用 GetLastError。 可能的错误代码包括 ERROR_BAD_LENGTH。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "CreateToolhelp32Snapshot", SetLastError = true), PreserveSig]
        public static partial nint CreateToolhelp32Snapshot(CreateToolhelp32SnapshotFlags dwFlags, uint th32ProcessID);

        /// <summary>
        /// 枚举由两个字母组成的国际标准化组织 (ISO) 3166-1 代码或数字联合国 (联合国) 系列 M，编号 49 (M.49) 操作系统上可用的地理位置代码。
        /// </summary>
        /// <param name="geoClass">要枚举其可用的双字母 ISO 3166-1 或数字 UN M.49 代码的地理位置类。</param>
        /// <param name="geoEnumProc">指向应用程序定义的回调函数 Geo_EnumNameProc的指针。 EnumSystemGeoNames 函数针对操作系统上可用的地理位置的每个双字母 ISO 3166-1 或数字 UN M.49 代码调用此回调函数，直到回调函数返回 FALSE。</param>
        /// <param name="data">要传递给 genEnumProc 参数指定的回调函数的应用程序特定信息。</param>
        /// <returns>如果成功，则返回非零值，否则返回 0。</returns>
        [LibraryImport(Kernel32, EntryPoint = "EnumSystemGeoNames", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool EnumSystemGeoNames(SYSGEOCLASS geoClass, GEO_ENUMNAMEPROC geoEnumProc, nint data);

        /// <summary>
        /// 检索当前进程的伪句柄。
        /// </summary>
        /// <returns>返回值是当前进程的伪句柄。</returns>
        [LibraryImport(Kernel32, EntryPoint = "GetCurrentProcess", SetLastError = false), PreserveSig]
        public static partial nint GetCurrentProcess();

        /// <summary>
        /// 检索指定进程的计时信息。
        /// </summary>
        /// <param name="hProcess">要获取其计时信息的进程的句柄。 句柄必须具有 PROCESS_QUERY_INFORMATION 或 PROCESS_QUERY_LIMITED_INFORMATION 访问权限。</param>
        /// <param name="lpCreationTime">指向 FILETIME 结构的指针，该结构接收进程的创建时间。</param>
        /// <param name="lpExitTime">指向 FILETIME 结构的指针，该结构接收进程的退出时间。 如果进程尚未退出，则此结构的内容未定义。</param>
        /// <param name="lpKernelTime">指向 FILETIME 结构的指针，该结构接收进程在内核模式下执行的时间。 确定进程的每个线程在内核模式下执行的时间，然后将所有这些时间相加以获取此值。</param>
        /// <param name="lpUserTime">指向 FILETIME 结构的指针，该结构接收进程在用户模式下执行的时间。 确定进程的每个线程在用户模式下执行的时间，然后将所有这些时间相加以获取此值。 请注意，如果进程跨多个 CPU 核心执行，则此值可能会超过 ( lpCreationTime 和 lpExitTime) 之间经过的实时量。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "GetProcessTimes", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetProcessTimes(nint hProcess, out System.Runtime.InteropServices.ComTypes.FILETIME lpCreationTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpExitTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpKernelTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpUserTime);

        /// <summary>
        /// 打开现有的本地进程对象。
        /// </summary>
        /// <param name="dwDesiredAccess">
        /// 对进程对象的访问。根据进程的安全描述符检查此访问权限。此参数可以是一个或多个进程访问权限。
        /// 如果调用方已启用SeDebugPrivilege 特权，则无论安全描述符的内容如何，都会授予请求的访问权限。</param>
        /// <param name="bInheritHandle">如果此值为 TRUE，则此进程创建的进程将继承句柄。否则，进程不会继承此句柄。</param>
        /// <param name="dwProcessId">
        /// 要打开的本地进程的标识符。
        /// 如果指定的进程是系统空闲进程 （0x00000000），则该函数将失败，最后一个错误代码为。
        /// 如果指定的进程是系统进程或客户端服务器运行时子系统 （CSRSS） 进程之一，则此函数将失败，
        /// 最后一个错误代码是因为它们的访问限制阻止用户级代码打开它们。ERROR_INVALID_PARAMETERERROR_ACCESS_DENIED
        /// </param>
        /// <returns>如果函数成功，则返回值是指定进程的打开句柄。如果函数失败，则返回值为 NULL。</returns>
        [LibraryImport(Kernel32, EntryPoint = "OpenProcess", SetLastError = false), PreserveSig]
        public static partial nint OpenProcess(EDesiredAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        /// <summary>
        /// 检索有关系统快照中遇到的第一个进程的信息。
        /// </summary>
        /// <param name="snapshot">从对 CreateToolhelp32Snapshot 函数的上一次调用返回的快照的句柄。</param>
        /// <param name="lppe">指向 PROCESSENTRY32 结构的指针。 它包含进程信息，例如可执行文件的名称、进程标识符和父进程的进程标识符。</param>
        /// <returns>
        /// 如果进程列表的第一个条目已复制到缓冲区或 FALSE，则返回 TRUE。 如果不存在进程或快照不包含进程信息，则 GetLastError 函数将返回ERROR_NO_MORE_FILES错误值。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "Process32FirstW", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool Process32First(nint snapshot, ref PROCESSENTRY32 lppe);

        /// <summary>
        /// 检索有关系统快照中记录的下一个进程的信息。
        /// </summary>
        /// <param name="snapshot">从对 CreateToolhelp32Snapshot 函数的上一次调用返回的快照的句柄。</param>
        /// <param name="lppe">指向 PROCESSENTRY32 结构的指针。</param>
        /// <returns>
        /// 如果进程列表的下一个条目已复制到缓冲区，则返回 TRUE，否则返回 FALSE。如果不存在任何进程或快照不包含进程信息，则 GetLastError 函数将返回ERROR_NO_MORE_FILES错误值。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "Process32NextW", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool Process32Next(nint snapshot, ref PROCESSENTRY32 lppe);

        /// <summary>
        /// 终止指定的进程及其所有线程。
        /// </summary>
        /// <param name="hProcess">句柄必须具有 PROCESS_TERMINATE 访问权限。 有关详细信息，请参阅 进程安全性和访问权限。</param>
        /// <param name="uExitCode">进程和线程将使用的退出代码，由于此调用而终止。 使用 GetExitCodeProcess 函数检索进程的退出值。 使用 GetExitCodeThread 函数检索线程的退出值。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "TerminateProcess", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool TerminateProcess(nint hProcess, uint uExitCode);
    }
}
