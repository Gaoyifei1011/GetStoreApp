using System;
using System.Runtime.InteropServices;

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
        [LibraryImport(Kernel32, EntryPoint = "AllocConsole", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool AllocConsole();

        /// <summary>
        /// 将调用进程附加到指定进程的控制台作为客户端应用程序
        /// </summary>
        /// <param name="dwProcessId">
        /// 将调用进程附加到指定进程的控制台作为客户端应用程序，默认值为ATTACH_PARENT_PROCESS = -1
        /// </param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "AttachConsole", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool AttachConsole(int dwProcessId = -1);

        /// <summary>
        /// 关闭打开的对象句柄。
        /// </summary>
        /// <param name="hObject">打开对象的有效句柄。</param>
        /// <returns>
        /// 如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。
        /// 如果应用程序在调试器下运行，则如果函数收到无效的句柄值或伪句柄值，该函数将引发异常。
        /// 如果两次关闭句柄，或者对 FindFirstFile 函数返回的句柄调用 CloseHandle，而不是调用 FindClose 函数，则可能会出现这种情况。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "CloseHandle", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// 创建匿名管道，并将句柄返回到管道的读取和写入端。
        /// </summary>
        /// <param name="hReadPipe">指向接收管道读取句柄的变量的指针。</param>
        /// <param name="hWritePipe">指向接收管道写入句柄的变量的指针。</param>
        /// <param name="lpPipeAttributes">
        /// 指向 <see cref="SECURITY_ATTRIBUTES"> 结构的指针，该结构确定返回的句柄是否可以由子进程继承。 如果 <param name="lpPipeAttributes"> 为 NULL，则无法继承句柄。
        /// 结构的 lpSecurityDescriptor 成员为新管道指定安全描述符。 如果 <param name="lpPipeAttributes"> 为 NULL，管道将获取默认的安全描述符。 管道的默认安全描述符中的 ACL 来自创建者的主要或模拟令牌。
        /// </param>
        /// <param name="nSize">管道缓冲区的大小（以字节为单位）。 大小只是建议;系统使用该值计算适当的缓冲机制。 如果此参数为零，则系统使用默认缓冲区大小。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "CreatePipe", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe partial bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, SECURITY_ATTRIBUTES* lpPipeAttributes, uint nSize);

        /// <summary>
        /// 获取指定进程的快照，以及这些进程使用的堆、模块和线程。
        /// </summary>
        /// <param name="dwFlags">要包含在快照中的系统部分。 此参数可使用以下一个或多个值。</param>
        /// <param name="th32ProcessID">
        /// 要包含在快照中的进程的进程标识符。 此参数可以为零以指示当前进程。 指定 <see cref="CreateToolhelp32SnapshotFlags.TH32CS_SNAPHEAPLIST">、 <see cref="CreateToolhelp32SnapshotFlags.TH32CS_SNAPMODULE">、 <see cref="CreateToolhelp32SnapshotFlags.TH32CS_SNAPMODULE32"> 或 <see cref="CreateToolhelp32SnapshotFlags.TH32CS_SNAPALL"> 值时，将使用此参数。 否则，它将被忽略，所有进程都包含在快照中。
        /// 如果指定的进程是空闲进程或 CSRSS 进程之一，则此函数将失败，并且最后一个错误代码 ERROR_ACCESS_DENIED ，因为它们的访问限制会阻止用户级代码打开它们。
        /// 如果指定的进程是 64 位进程，并且调用方是 32 位进程，则此函数将失败，最后一个错误代码 ERROR_PARTIAL_COPY( 299) 。
        /// </param>
        /// <returns>
        /// 如果函数成功，它将返回指定快照的打开句柄。如果函数失败，它将返回 INVALID_HANDLE_VALUE。 要获得更多的错误信息，请调用 GetLastError。 可能的错误代码包括 ERROR_BAD_LENGTH。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "CreateToolhelp32Snapshot", SetLastError = true)]
        public static partial IntPtr CreateToolhelp32Snapshot(CreateToolhelp32SnapshotFlags dwFlags, uint th32ProcessID);

        /// <summary>
        /// 创建新进程及其主线程。 新进程在调用进程的安全上下文中运行。
        /// 如果调用进程正在模拟其他用户，则新进程将令牌用于调用进程，而不是模拟令牌。 若要在模拟令牌表示的用户的安全上下文中运行新进程，请使用 CreateProcessAsUser 或 CreateProcessWithLogonW 函数。
        /// </summary>
        /// <param name="lpApplicationName">
        /// 要执行的模块的名称。 此模块可以是基于 Windows 的应用程序。 它可以是某种其他类型的模块 (例如 MS-DOS 或 OS/2) （如果本地计算机上提供了相应的子系统）。
        /// 字符串可以指定要执行的模块的完整路径和文件名，也可以指定部分名称。 对于部分名称，函数使用当前驱动器和当前目录来完成规范。 函数不会使用搜索路径。 此参数必须包含文件扩展名;不采用默认扩展名。
        /// <param name="lpApplicationName"> 参数可以为 NULL。 在这种情况下，模块名称必须是 <param name="lpCommandLine"> 字符串中第一个空格分隔的标记。 如果使用包含空格的长文件名，请使用带引号的字符串来指示文件名结束和参数开始的位置;否则，文件名不明确。
        /// 如果可执行模块是 16 位应用程序， <param name="lpApplicationName"> 应为 NULL， <param name="lpCommandLine"> 指向的字符串应指定可执行模块及其参数。
        /// 若要运行批处理文件，必须启动命令解释器;将 <param name="lpApplicationName"> 设置为 cmd.exe 并将 <param name="lpCommandLine"> 设置为以下参数：/c 加上批处理文件的名称。
        /// </param>
        /// <param name="lpCommandLine">
        /// 要执行的命令行。
        /// 此字符串的最大长度为 32，767 个字符，包括 Unicode 终止 null 字符。 如果 lpApplicationName 为 NULL，则 lpCommandLine 的模块名称部分限制为 MAX_PATH 个字符。
        /// 此函数的 Unicode 版本 CreateProcessW 可以修改此字符串的内容。 因此，此参数不能是指向只读内存(的指针，例如 const 变量或文本字符串) 。 如果此参数是常量字符串，该函数可能会导致访问冲突。
        /// <param name="lpCommandLine"> 参数可以为 NULL。 在这种情况下，函数使用 <param name="lpApplicationName"> 指向的字符串作为命令行。
        /// 如果 <param name="lpApplicationName"> 和 <param name="lpCommandLine"> 均为非 NULL，则 <param name="lpApplicationName"> 指向的以 null 结尾的字符串将指定要执行的模块， 而 <param name="lpCommandLine"> 指向的以 null 结尾的字符串将指定命令行。 新进程可以使用 GetCommandLine 检索整个命令行。 用 C 编写的控制台进程可以使用 argc 和 argv 参数来分析命令行。 由于 argv[0] 是模块名称，因此 C 程序员通常将模块名称重复为命令行中的第一个标记。
        /// 如果 <param name="lpApplicationName"> 为 NULL，则命令行的第一个空格分隔标记将指定模块名称。 如果使用包含空格的长文件名，请使用带引号的字符串来指示文件名结束和参数开始的位置 (请参阅 <param name="lpApplicationName"> 参数) 的说明。 如果文件名不包含扩展名，则追加.exe。 因此，如果文件扩展名为 .com，则此参数必须包含 .com 扩展名。 如果文件名以不带扩展名的句点 (.) 结尾，或者文件名包含路径，则不会追加.exe。 如果文件名不包含目录路径，系统会按以下顺序搜索可执行文件：
        /// 1.从中加载应用程序的目录。
        /// 2.父进程的当前目录。
        /// 3.32 位 Windows 系统目录。 使用 GetSystemDirectory 函数获取此目录的路径。
        /// 4.16 位 Windows 系统目录。 没有获取此目录的路径的函数，但会对其进行搜索。 此目录的名称为 System。
        /// 5.Windows 目录。 使用 GetWindowsDirectory 函数获取此目录的路径。
        /// 6.PATH 环境变量中列出的目录。 请注意，此函数不会搜索应用程序路径注册表项指定的每个 应用程序 路径。 若要在搜索序列中包含此每个应用程序的路径，请使用 ShellExecute 函数。
        /// 系统向命令行字符串添加一个终止 null 字符，以将文件名与参数分开。 这会将原始字符串划分为两个字符串以供内部处理。
        /// </param>
        /// <param name="lpProcessAttributes">
        /// 指向 SECURITY_ATTRIBUTES 结构的指针，该结构确定返回的新进程对象的句柄是否可以由子进程继承。 如果 <param name="lpProcessAttributes"> 为 NULL，则不能继承句柄。
        /// 结构的 lpSecurityDescriptor 成员为新进程指定安全描述符。 如果 <param name="lpProcessAttributes"> 为 NULL 或 lpSecurityDescriptor 为 NULL，则进程将获取默认安全描述符。 进程的默认安全描述符中的 ACL 来自创建者的主令牌。Windowsxp： 进程的默认安全描述符中的 ACL 来自创建者的主要令牌或模拟令牌。 此行为随 Windows XP SP2 和 Windows Server 2003 更改。
        /// </param>
        /// <param name="lpThreadAttributes">
        /// 指向 SECURITY_ATTRIBUTES 结构的指针，该结构确定返回的新线程对象的句柄是否可以由子进程继承。 如果 <param name="lpThreadAttributes"> 为 NULL，则不能继承句柄。
        /// 结构的 lpSecurityDescriptor 成员指定主线程的安全描述符。 如果 <param name="lpThreadAttributes"> 为 NULL 或 lpSecurityDescriptor 为 NULL，则线程获取默认安全描述符。 线程的默认安全描述符中的 ACL 来自进程令牌。Windowsxp： 线程的默认安全描述符中的 ACL 来自创建者的主令牌或模拟令牌。 此行为随 Windows XP SP2 和 Windows Server 2003 更改。
        /// </param>
        /// <param name="bInheritHandles">
        /// 如果此参数为 TRUE，则调用进程中的每个可继承句柄都由新进程继承。 如果参数为 FALSE，则不继承句柄。 请注意，继承的句柄与原始句柄具有相同的值和访问权限。 有关可继承句柄的其他讨论，请参阅备注。
        /// 终端服务： 不能跨会话继承句柄。 此外，如果此参数为 TRUE，则必须在与调用方相同的会话中创建进程。
        /// 受保护的流程灯(PPL) 进程： 当 PPL 进程创建非 PPL 进程时，将阻止泛型句柄继承，因为不允许将PROCESS_DUP_HANDLE从非 PPL 进程转换为 PPL 进程。 请参阅 进程安全性和访问权限
        /// </param>
        /// <param name="dwCreationFlags">
        /// 控制优先级类和进程的创建的标志。 有关值的列表，请参阅 进程创建标志。
        /// 此参数还控制新进程的优先级类，该类用于确定进程线程的计划优先级。 有关值的列表，请参阅 GetPriorityClass。 如果未指定任何优先级类标志，则优先级类默认为 NORMAL_PRIORITY_CLASS ，除非创建过程的优先级类 IDLE_PRIORITY_CLASS 或 BELOW_NORMAL_PRIORITY_CLASS。 在这种情况下，子进程接收调用进程的默认优先级类。
        /// 如果 dwCreationFlags 参数的值为 0：
        /// 1.进程同时继承调用方和父级控制台的错误模式。
        /// 2.假定新进程的环境块包含 ANSI 字符(请参阅 lpEnvironment 参数以获取) 的其他信息。
        /// 3.基于 16 位 Windows 的应用程序在共享的虚拟 DOS 计算机中运行， (VDM) 。
        /// </param>
        /// <param name="lpEnvironment">
        /// 指向新进程的环境块的指针。 如果此参数为 NULL，则新进程使用调用进程的 环境。环境块由以 null 结尾的字符串的以 null 结尾的块组成。
        /// 每个字符串采用以下格式：名字=value\0
        /// 由于等号用作分隔符，因此不得在环境变量的名称中使用。环境块可以包含 Unicode 或 ANSI 字符。 如果 <param name="lpEnvironment"> 指向的环境块包含 Unicode 字符，请确保 dwCreationFlags 包含 CREATE_UNICODE_ENVIRONMENT。如果进程的环境块的总大小超过 32，767 个字符，则此函数的 ANSI 版本 CreateProcessA 将失败。
        /// 请注意，ANSI 环境块以两个零字节结尾：一个字节用于最后一个字符串，另一个用于终止该块。 Unicode 环境块以四个零字节结尾：两个作为最后一个字符串，另外两个用于终止该块。
        /// </param>
        /// <param name="lpCurrentDirectory">
        /// 进程当前目录的完整路径。 字符串还可以指定 UNC 路径。如果此参数为 NULL，则新进程将具有与调用进程相同的当前驱动器和目录。 (此功能主要用于需要启动应用程序并指定其初始驱动器和工作目录的 shell。)
        /// </param>
        /// <param name="lpStartupInfo">
        /// 指向 <see cref="STARTUPINFO"> 或 STARTUPINFOEX 结构的指针。若要设置扩展属性，请使用 STARTUPINFOEX 结构并在 <param name="dwCreationFlags"> 参数中指定 <see cref="CreateProcessFlags.EXTENDED_STARTUPINFO_PRESENT">。当不再需要 <see cref="STARTUPINFO"> 或 STARTUPINFOEX 中的句柄时，必须使用 <see cref="CloseHandle"> 关闭它们。
        /// 调用方负责确保 <see cref="STARTUPINFO"> 中的标准句柄字段包含有效的句柄值。 即使 dwFlags 成员指定了 STARTF_USESTDHANDLES，这些字段也保持不变地复制到子进程，而无需验证。 不正确的值可能导致子进程行为不端或崩溃。 使用应用程序验证程序运行时验证工具检测无效句柄。
        /// </param>
        /// <param name="lpProcessInformation">
        /// 指向 <see cref="PROCESS_INFORMATION"> 结构的指针，该结构接收有关新进程的标识信息。当不再需要 <see cref="PROCESS_INFORMATION"> 中的句柄时，必须使用 <see cref="CloseHandle"> 将其关闭。
        /// </param>
        /// <returns>
        /// 如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。
        /// 请注意，函数在进程完成初始化之前返回 。 如果找不到所需的 DLL 或无法初始化，则进程将终止。 若要获取进程的终止状态，请调用 GetExitCodeProcess。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "CreateProcessW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
            CreateProcessFlags dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        /// <summary>
        /// 从其控制台中分离调用进程。
        /// </summary>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "FreeConsole", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool FreeConsole();

        /// <summary>
        /// 检索与调用进程关联的控制台所使用的窗口句柄。
        /// </summary>
        /// <returns>返回值是与调用进程关联的控制台所使用的窗口的句柄; 如果没有此类关联的控制台，则为 NULL 。</returns>
        [LibraryImport(Kernel32, EntryPoint = "GetConsoleWindow", SetLastError = false)]
        public static partial IntPtr GetConsoleWindow();

        /// <summary>
        /// 获取调用进程的包全名。
        /// </summary>
        /// <param name="packageFullNameLength">
        /// 输入时， packageFullName 缓冲区的大小（以字符为单位）。 在输出中，返回的包全名的大小（以字符为单位，包括 null 终止符）。
        /// </param>
        /// <param name="packageFullName">包全名。</param>
        /// <returns>
        /// 如果函数成功，它将返回 ERROR_SUCCESS。 否则，该函数将返回错误代码。 可能的错误代码包括以下内容。
        /// APPMODEL_ERROR_NO_PACKAGE：此过程没有包标识。
        /// ERROR_INSUFFICIENT_BUFFER：缓冲区不够大，无法保存数据。 所需大小由 packageFullNameLength 指定。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "GetCurrentPackageFullName", SetLastError = true)]
        public static unsafe partial int GetCurrentPackageFullName(ref int packageFullNameLength, char* packageFullName);

        /// <summary>
        /// 检索创建调用进程时指定的 <see cref="STARTUPINFO"> 结构的内容。
        /// </summary>
        /// <param name="lpStartupInfo">指向接收启动信息的 <see cref="STARTUPINFO"> 结构的指针。</param>
        [LibraryImport(Kernel32, EntryPoint = "GetStartupInfoW", SetLastError = false)]
        public static partial void GetStartupInfo(out STARTUPINFO lpStartupInfo);

        /// <summary>
        /// 检索指定标准设备的句柄（标准输入、标准输出或标准错误）。
        /// </summary>
        /// <param name="nStdHandle">标准设备。</param>
        /// <returns>
        /// 如果该函数成功，则返回值为指定设备的句柄，或为由先前对 <see cref="SetStdHandle"> 的调用设置的重定向句柄。
        /// 除非应用程序已使用 <see cref="SetStdHandle"> 来设置具有较少访问权限的标准句柄，否则该句柄具有 GENERIC_READ 和 GENERIC_WRITE 访问权限。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "GetStdHandle", SetLastError = true)]
        public static partial IntPtr GetStdHandle(StdHandle nStdHandle);

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
        /// <param name="snapshot">从对 <see cref="CreateToolhelp32Snapshot"> 函数的上一次调用返回的快照的句柄。</param>
        /// <param name="lppe">指向 <see cref="PROCESSENTRY32"> 结构的指针。 它包含进程信息，例如可执行文件的名称、进程标识符和父进程的进程标识符。</param>
        /// <returns>
        /// 如果进程列表的第一个条目已复制到缓冲区或 FALSE，则返回 TRUE。 如果不存在进程或快照不包含进程信息，则 GetLastError 函数将返回ERROR_NO_MORE_FILES错误值。
        /// </returns>
        [LibraryImport(Kernel32, EntryPoint = "Process32FirstW", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool Process32First(IntPtr snapshot, ref PROCESSENTRY32 lppe);

        /// <summary>
        /// 检索有关系统快照中记录的下一个进程的信息。
        /// </summary>
        /// <param name="snapshot">从对 <see cref="CreateToolhelp32Snapshot"> 函数的上一次调用返回的快照的句柄。</param>
        /// <param name="lppe">指向 <see cref="PROCESSENTRY32"> 结构的指针。</param>
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
        [LibraryImport(Kernel32, EntryPoint = "ReadConsoleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe partial bool ReadConsole(
            IntPtr hConsoleInput,
            byte[] lpBuffer,
            int nNumberOfCharsToRead,
            out int lpNumberOfCharsRead,
            IntPtr pInputControl);

        /// <summary>
        /// 从指定的文件或输入/输出 (I/O) 设备读取数据。 如果设备支持，则读取发生在文件指针指定的位置。
        /// 此函数适用于同步操作和异步操作。 有关专为异步操作设计的类似函数，请参阅 ReadFileEx。
        /// </summary>
        /// <param name="hFile">
        /// 设备句柄 (例如文件、文件流、物理磁盘、卷、控制台缓冲区、磁带驱动器、套接字、通信资源、mailslot 或管道) 。必须使用读取访问权限创建 <param name="hFile"> 参数。
        /// 有关详细信息，请参阅通用访问权限和文件安全性和访问权限。对于异步读取操作，<param name="hFile"> 可以是使用 CreateFile 函数的 FILE_FLAG_OVERLAPPED 标志打开的任何句柄，
        /// 也可以是套接字或 accept 函数返回的套接字句柄。
        /// </param>
        /// <param name="lpBuffer">
        /// 指向接收从文件或设备读取数据的缓冲区的指针。此缓冲区必须在读取操作期间保持有效。 在读取操作完成之前，调用方不得使用此缓冲区。
        /// </param>
        /// <param name="nNumberOfBytesToRead">要读取的最多字节数。</param>
        /// <param name="lpNumberOfBytesRead">
        /// 指向使用同步 <param name="hFile"> 参数时接收读取的字节数的变量的指针。 <see cref="ReadFile"> 将此值设置为零，然后再执行任何工作或错误检查。
        /// 如果这是一个异步操作，请对此参数使用 NULL ，以避免潜在的错误结果。仅当 <param name="lpOverlapped"> 参数不为 NULL 时，此参数才可为 NULL。
        /// Windows 7： 此参数不能为 NULL。
        /// </param>
        /// <param name="lpOverlapped">
        /// 如果使用 FILE_FLAG_OVERLAPPED 打开 <param name="hFile"> 参数，则需要指向 OVERLAPPED 结构的指针，否则可为 NULL。
        /// 如果使用 FILE_FLAG_OVERLAPPED打开 <param name="hFile">，则 <param name="lpOverlapped"> 参数必须指向有效且唯一的 OVERLAPPED 结构，否则该函数无法错误地报告读取操作已完成。
        /// 对于支持字节偏移量的 <param name="hFile"> ，如果使用此参数，则必须指定从文件或设备开始读取的字节偏移量。 通过设置 OVERLAPPED 结构的 Offset 和 OffsetHigh 成员来指定此偏移量。
        /// 对于不支持字节偏移量的 <param name="hFile">，将忽略 Offset 和 OffsetHigh。
        /// 有关 lpOverlapped 和 FILE_FLAG_OVERLAPPED的不同组合的详细信息，请参阅“备注”部分和 “同步和文件位置 ”部分。
        /// </param>
        /// <returns>如果函数成功，则返回值为非零 (TRUE) 。如果函数失败或异步完成，则返回值为零，(FALSE) </returns>
        [LibraryImport(Kernel32, EntryPoint = "ReadFile", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ReadFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        /// <summary>
        /// 设置由 WriteFile 或 WriteConsole 函数写入控制台屏幕缓冲区或由 ReadFile 或 ReadConsole 函数回显的字符的属性。 此函数会影响函数调用后写入的文本。
        /// </summary>
        /// <param name="hConsoleOutput">控制台屏幕缓冲区的句柄。 该句柄必须具有 GENERIC_READ 访问权限。 有关详细信息，请参阅控制台缓冲区安全性和访问权限。</param>
        /// <param name="wAttributes">字符属性。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Kernel32, EntryPoint = "SetConsoleTextAttribute", SetLastError = true)]
        public static partial int SetConsoleTextAttribute(IntPtr hConsoleOutput, ushort wAttributes);

        /// <summary>
        /// 设置当前控制台窗口的标题。
        /// </summary>
        /// <param name="title">要在控制台窗口的标题栏中显示的字符串。 总大小必须小于64K。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Kernel32, EntryPoint = "SetConsoleTitleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetConsoleTitle(string lpConsoleTitle);

        /// <summary>
        /// 为指定的标准设备 (标准输入、标准输出或标准错误) 设置句柄。
        /// </summary>
        /// <param name="nStdHandle">要为其设置句柄的标准设备。</param>
        /// <param name="handle">标准设备的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "SetStdHandle", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetStdHandle(StdHandle nStdHandle, IntPtr handle);

        /// <summary>
        /// 从调用进程的处理程序函数列表中添加或删除应用程序定义的 HandlerRoutine 函数。如果未指定处理程序函数，则该函数将设置可继承的属性，该属性确定调用进程是否忽略 Ctrl+C 信号。
        /// </summary>
        /// <param name="HandlerRoutine">指向应用程序定义的要添加或删除的 HandlerRoutine 函数的指针。 此参数可以为 NULL。</param>
        /// <param name="add">如果此参数为 TRUE，则添加处理程序；如果 FALSE，则删除处理程序 。如果 HandlerRoutine 参数为 NULL，TRUE 值会使调用进程忽略 Ctrl+C 输入，而 FALSE 值会还原 Ctrl+C 输入的正常处理过程 。 此属性（忽略或处理 Ctrl+C）由子进程继承。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Kernel32, EntryPoint = "SetConsoleCtrlHandler", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetConsoleCtrlHandler(ConsoleEventDelegate HandlerRoutine, [MarshalAs(UnmanagedType.Bool)] bool add);

        /// <summary>
        /// 终止指定的进程及其所有线程。
        /// </summary>
        /// <param name="hProcess">句柄必须具有 <see cref="EDesiredAccess.PROCESS_TERMINATE"> 访问权限。 有关详细信息，请参阅 进程安全性和访问权限。</param>
        /// <param name="uExitCode">
        /// 进程和线程因此调用而终止的退出代码。 使用 GetExitCodeProcess 函数检索进程的退出值。 使用 GetExitCodeThread 函数检索线程的退出值。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Kernel32, EntryPoint = "TerminateProcess", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        /// <summary>
        /// 从当前光标位置开始，将字符串写入控制台屏幕缓冲区。
        /// </summary>
        /// <param name="hConsoleOutput">控制台屏幕缓冲区的句柄。 此句柄必须具有 GENERIC_WRITE 访问权限。 有关详细信息，请参阅控制台缓冲区安全性和访问权限。</param>
        /// <param name="lpBuffer">指向缓冲区的指针，该缓冲区包含要写入控制台屏幕缓冲区的字符。 这应为 WriteConsoleA 的 char 数组，或 WriteConsoleW 的 wchar_t 数组。</param>
        /// <param name="nNumberOfCharsToWrite">将要写入的字符数。 如果指定数量的字符的总大小超过可用堆，则函数将失败并出现 ERROR_NOT_ENOUGH_MEMORY。</param>
        /// <param name="lpNumberOfCharsWritten">指向某个变量的指针，该变量接收实际写入的字符数。</param>
        /// <param name="lpReservedMustBeNull">lpReserved 保留；必须为 NULL。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Kernel32, EntryPoint = "WriteConsoleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe partial bool WriteConsole(
             IntPtr hConsoleOutput,
             [MarshalAs(UnmanagedType.LPWStr)] string lpBuffer,
             int nNumberOfCharsToWrite,
             out int lpNumberOfCharsWritten,
             IntPtr lpReservedMustBeNull);
    }
}
