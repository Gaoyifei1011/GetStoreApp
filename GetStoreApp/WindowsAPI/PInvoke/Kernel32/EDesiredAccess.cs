namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 进程对象的访问权限
    /// </summary>
    public enum EDesiredAccess : uint
    {
        /// <summary>
        /// 删除对象所必需的。
        /// </summary>
        DELETE = 0x00010000,

        /// <summary>
        /// 需要读取对象的安全描述符中的信息，而不包括 SACL 中的信息。 若要读取或写入 SACL，必须请求 ACCESS_SYSTEM_SECURITY 访问权限。
        /// </summary>
        READ_CONTROL = 0x00020000,

        /// <summary>
        /// 在对象的安全描述符中修改 DACL 是必需的。
        /// </summary>
        WRITE_DAC = 0x00040000,

        /// <summary>
        /// 在对象的安全描述符中更改所有者所必需的。
        /// </summary>
        WRITE_OWNER = 0x00080000,

        /// <summary>
        /// 将对象用于同步的权限。 这使线程可以等待对象处于信号状态。
        /// </summary>
        SYNCHRONIZE = 0x00100000,

        /// <summary>
        /// 需要使用 TerminateProcess 终止进程。
        /// </summary>
        PROCESS_TERMINATE = 0x0001,

        /// <summary>
        /// 在进程中创建线程所必需的。
        /// </summary>
        PROCESS_CREATE_THREAD = 0x0002,

        /// <summary>
        /// 在进程中设置进程会话
        /// </summary>
        PROCESS_SET_SESSIONID = 0x0004,

        /// <summary>
        /// 在进程地址空间上执行操作
        /// </summary>
        PROCESS_VM_OPERATION = 0x0008,

        /// <summary>
        /// 需要使用 ReadProcessMemory 读取进程中的内存。
        /// </summary>
        PROCESS_VM_READ = 0x0010,

        /// <summary>
        /// 需要使用 WriteProcessMemory 写入进程中的内存。
        /// </summary>
        PROCESS_VM_WRITE = 0x0020,

        /// <summary>
        /// 需要使用 DuplicateHandle 复制句柄。
        /// </summary>
        PROCESS_DUP_HANDLE = 0x0040,

        /// <summary>
        /// 需要使用此过程作为父进程和 PROC_THREAD_ATTRIBUTE_PARENT_PROCESS。
        /// </summary>
        PROCESS_CREATE_PROCESS = 0x0080,

        /// <summary>
        /// 需要使用 SetProcessWorkingSetSize 设置内存限制。
        /// </summary>
        PROCESS_SET_QUOTA = 0x0100,

        /// <summary>
        /// 设置有关进程的某些信息
        /// </summary>
        PROCESS_SET_INFORMATION = 0x0200,

        /// <summary>
        /// 检索有关进程的某些信息
        /// </summary>
        PROCESS_QUERY_INFORMATION = 0x0400,

        /// <summary>
        /// 暂停或恢复进程所必需的。
        /// </summary>
        PROCESS_SUSPEND_RESUME = 0x0800,

        /// <summary>
        /// 检索有关进程的某些信息需要。 <see cref="PROCESS_QUERY_LIMITED_INFORMATION"> 自动授予具有 <see cref="PROCESS_QUERY_INFORMATION"> 访问权限的句柄。
        /// </summary>
        PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,

        /// <summary>
        /// 进程对象的所有可能访问权限。
        /// </summary>
        PROCESS_ALL_ACCESS = SYNCHRONIZE | 0xFFF
    }
}
