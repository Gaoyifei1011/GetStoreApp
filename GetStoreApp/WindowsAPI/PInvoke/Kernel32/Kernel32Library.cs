﻿using System;
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

        public const int APPMODEL_ERROR_NO_PACKAGE = 15700;

        public const int MAX_PATH = 260;

        public static IntPtr INVALID_HANDLE_VALUE { get; } = new IntPtr(-1);

        /// <summary>
        /// 为调用进程分配一个新的控制台。
        /// </summary>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "AllocConsole", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool AllocConsole();

        /// <summary>
        /// 将调用进程附加到指定进程的控制台作为客户端应用程序
        /// </summary>
        /// <param name="dwProcessId">
        /// 将调用进程附加到指定进程的控制台作为客户端应用程序，默认值为ATTACH_PARENT_PROCESS = -1
        /// </param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "AttachConsole", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool AttachConsole(int dwProcessId = -1);

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
        [LibraryImport(Kernel32, EntryPoint = "CreateToolhelp32Snapshot", SetLastError = true)]
        public static partial IntPtr CreateToolhelp32Snapshot(CreateToolhelp32SnapshotFlags dwFlags, uint th32ProcessID);

        /// <summary>
        /// 枚举由两个字母组成的国际标准化组织 (ISO) 3166-1 代码或数字联合国 (联合国) 系列 M，编号 49 (M.49) 操作系统上可用的地理位置代码。
        /// </summary>
        /// <param name="geoClass">要枚举其可用的双字母 ISO 3166-1 或数字 UN M.49 代码的地理位置类。</param>
        /// <param name="geoEnumProc">指向应用程序定义的回调函数 Geo_EnumNameProc的指针。 EnumSystemGeoNames 函数针对操作系统上可用的地理位置的每个双字母 ISO 3166-1 或数字 UN M.49 代码调用此回调函数，直到回调函数返回 FALSE。</param>
        /// <param name="data">要传递给 genEnumProc 参数指定的回调函数的应用程序特定信息。</param>
        /// <returns>如果成功，则返回非零值，否则返回 0。</returns>
        [LibraryImport(Kernel32, EntryPoint = "EnumSystemGeoNames", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool EnumSystemGeoNames(SYSGEOCLASS geoClass, GEO_ENUMNAMEPROC geoEnumProc, IntPtr data);

        /// <summary>
        /// 从其控制台中分离调用进程。
        /// </summary>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "FreeConsole", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool FreeConsole();

        /// <summary>
        /// 检索当前进程的伪句柄。
        /// </summary>
        /// <returns>返回值是当前进程的伪句柄。</returns>
        [LibraryImport(Kernel32, EntryPoint = "GetCurrentProcess", SetLastError = false), PreserveSig]
        public static partial IntPtr GetCurrentProcess();

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
        public static partial bool GetProcessTimes(IntPtr hProcess, out System.Runtime.InteropServices.ComTypes.FILETIME lpCreationTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpExitTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpKernelTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpUserTime);

        /// <summary>
        /// 检索指定标准设备的句柄（标准输入、标准输出或标准错误）。
        /// </summary>
        /// <param name="nStdHandle">标准设备。</param>
        /// <returns>
        /// 如果该函数成功，则返回值为指定设备的句柄，或为由先前对 SetStdHandle 的调用设置的重定向句柄。
        /// 除非应用程序已使用 SetStdHandle 来设置具有较少访问权限的标准句柄，否则该句柄具有 GENERIC_READ 和 GENERIC_WRITE 访问权限。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "GetStdHandle", SetLastError = false), PreserveSig]
        public static partial IntPtr GetStdHandle(STD_HANDLE nStdHandle);

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
        [LibraryImport(Kernel32, EntryPoint = "OpenProcess", SetLastError = false)]
        public static partial IntPtr OpenProcess(EDesiredAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        /// <summary>
        /// 检索有关系统快照中遇到的第一个进程的信息。
        /// </summary>
        /// <param name="snapshot">从对 CreateToolhelp32Snapshot 函数的上一次调用返回的快照的句柄。</param>
        /// <param name="lppe">指向 PROCESSENTRY32 结构的指针。 它包含进程信息，例如可执行文件的名称、进程标识符和父进程的进程标识符。</param>
        /// <returns>
        /// 如果进程列表的第一个条目已复制到缓冲区或 FALSE，则返回 TRUE。 如果不存在进程或快照不包含进程信息，则 GetLastError 函数将返回ERROR_NO_MORE_FILES错误值。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "Process32FirstW", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool Process32First(IntPtr snapshot, ref PROCESSENTRY32 lppe);

        /// <summary>
        /// 检索有关系统快照中记录的下一个进程的信息。
        /// </summary>
        /// <param name="snapshot">从对 CreateToolhelp32Snapshot 函数的上一次调用返回的快照的句柄。</param>
        /// <param name="lppe">指向 PROCESSENTRY32 结构的指针。</param>
        /// <returns>
        /// 如果进程列表的下一个条目已复制到缓冲区，则返回 TRUE，否则返回 FALSE。如果不存在任何进程或快照不包含进程信息，则 GetLastError 函数将返回ERROR_NO_MORE_FILES错误值。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "Process32NextW", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool Process32Next(IntPtr snapshot, ref PROCESSENTRY32 lppe);

        /// <summary>
        /// 从控制台输入缓冲区读取字符输入，并将其从缓冲区中删除。
        /// </summary>
        /// <param name="hConsoleInput">控制台输入缓冲区的句柄。 该句柄必须具有 GENERIC_READ 访问权限。 有关详细信息，请参阅控制台缓冲区安全性和访问权限。</param>
        /// <param name="lpBuffer">指向接收从控制台输入缓冲区读取数据的缓冲区的指针。</param>
        /// <param name="nNumberOfCharsToRead">要读取的字符数。 lpBuffer 参数指向的缓冲区的大小应至少nNumberOfCharsToRead * sizeof(TCHAR)为字节。</param>
        /// <param name="lpNumberOfCharsRead">指向接收实际读取字符数的变量的指针。</param>
        /// <param name="pInputControl">指向 CONSOLE_READCONSOLE_CONTROL 结构的指针，该结构指定要向读取操作末尾发出信号的控制字符。 此参数可以为 NULL。此参数默认需要 Unicode 输入。 对于 ANSI 模式，请将此参数设置为 NULL。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Kernel32, EntryPoint = "ReadConsoleW", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ReadConsole(IntPtr hConsoleInput, [Out, MarshalAs(UnmanagedType.LPArray)] char[] lpBuffer, int nNumberOfCharsToRead, out int lpNumberOfCharsRead, IntPtr pInputControl);

        /// <summary>
        /// 设置由 WriteFile 或 WriteConsole 函数写入控制台屏幕缓冲区或由 ReadFile 或 ReadConsole 函数回显的字符的属性。 此函数会影响函数调用后写入的文本。
        /// </summary>
        /// <param name="hConsoleOutput">控制台屏幕缓冲区的句柄。 该句柄必须具有 GENERIC_READ 访问权限。 有关详细信息，请参阅控制台缓冲区安全性和访问权限。</param>
        /// <param name="wAttributes">字符属性。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Kernel32, EntryPoint = "SetConsoleTextAttribute", SetLastError = false), PreserveSig]
        public static partial int SetConsoleTextAttribute(IntPtr hConsoleOutput, ushort wAttributes);

        /// <summary>
        /// 设置当前控制台窗口的标题。
        /// </summary>
        /// <param name="title">要在控制台窗口的标题栏中显示的字符串。 总大小必须小于64K。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Kernel32, EntryPoint = "SetConsoleTitleW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetConsoleTitle([MarshalAs(UnmanagedType.LPWStr)] string lpConsoleTitle);

        /// <summary>
        /// 从调用进程的处理程序函数列表中添加或删除应用程序定义的 HandlerRoutine 函数。如果未指定处理程序函数，则该函数将设置可继承的属性，该属性确定调用进程是否忽略 Ctrl+C 信号。
        /// </summary>
        /// <param name="HandlerRoutine">指向应用程序定义的要添加或删除的 HandlerRoutine 函数的指针。 此参数可以为 NULL。</param>
        /// <param name="add">如果此参数为 TRUE，则添加处理程序；如果 FALSE，则删除处理程序 。如果 HandlerRoutine 参数为 NULL，TRUE 值会使调用进程忽略 Ctrl+C 输入，而 FALSE 值会还原 Ctrl+C 输入的正常处理过程 。 此属性（忽略或处理 Ctrl+C）由子进程继承。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Kernel32, EntryPoint = "SetConsoleCtrlHandler", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetConsoleCtrlHandler(ConsoleEventDelegate handlerRoutine, [MarshalAs(UnmanagedType.Bool)] bool add);

        /// <summary>
        /// 从当前光标位置开始，将字符串写入控制台屏幕缓冲区。
        /// </summary>
        /// <param name="hConsoleOutput">控制台屏幕缓冲区的句柄。 此句柄必须具有 GENERIC_WRITE 访问权限。 有关详细信息，请参阅控制台缓冲区安全性和访问权限。</param>
        /// <param name="lpBuffer">指向缓冲区的指针，该缓冲区包含要写入控制台屏幕缓冲区的字符。 这应为 WriteConsoleA 的 char 数组，或 WriteConsoleW 的 wchar_t 数组。</param>
        /// <param name="nNumberOfCharsToWrite">将要写入的字符数。 如果指定数量的字符的总大小超过可用堆，则函数将失败并出现 ERROR_NOT_ENOUGH_MEMORY。</param>
        /// <param name="lpNumberOfCharsWritten">指向某个变量的指针，该变量接收实际写入的字符数。</param>
        /// <param name="lpReservedMustBeNull">lpReserved 保留；必须为 NULL。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Kernel32, EntryPoint = "WriteConsoleW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool WriteConsole(IntPtr hConsoleOutput, [MarshalAs(UnmanagedType.LPWStr)] string lpBuffer, int nNumberOfCharsToWrite, out int lpNumberOfCharsWritten, IntPtr lpReservedMustBeNull);
    }
}
