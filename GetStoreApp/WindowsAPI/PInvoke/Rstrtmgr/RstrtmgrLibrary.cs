using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace WindowsTools.WindowsAPI.PInvoke.Rstrtmgr
{
    public static partial class RstrtmgrLibrary
    {
        private const string Rstrtmgr = "rstrtmgr.dll";

        /// <summary>
        /// 将资源注册到重启管理器会话。 重启管理器使用向会话注册的资源列表来确定必须关闭和重启哪些应用程序和服务。 可以通过文件名、服务短名称或描述正在运行的应用程序 RM_UNIQUE_PROCESS 结构来标识资源。 RmRegisterResources 函数可由主安装程序或辅助安装程序使用。
        /// </summary>
        /// <param name="pSessionHandle">现有重启管理器会话的句柄。</param>
        /// <param name="nFiles">正在注册的文件数。</param>
        /// <param name="rgsFilenames">完整文件名路径的 以 null 结尾的字符串数组。 如果 nFiles 为 0，此参数可以为 NULL。</param>
        /// <param name="nApplications">正在注册的进程数。</param>
        /// <param name="rgApplications">RM_UNIQUE_PROCESS 结构的数组。 如果 nApplications 为 0，此参数可以为 NULL。</param>
        /// <param name="nServices">要注册的服务数。</param>
        /// <param name="rgsServiceNames">以 null 结尾的服务短名称字符串的数组。 如果 nServices 为 0，此参数可以为 NULL。</param>
        /// <returns>这是收到的最新错误。 函数可以返回 Winerror.h 中定义的系统错误代码之一。</returns>
        [LibraryImport(Rstrtmgr, EntryPoint = "RmRegisterResources", SetLastError = false), PreserveSig]
        public static partial int RmRegisterResources(uint pSessionHandle, uint nFiles, [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0)] string[] rgsFilenames, uint nApplications, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RM_UNIQUE_PROCESS[] rgApplications, uint nServices, [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0)] string[] rgsServiceNames);

        /// <summary>
        /// 重启已由 RmShutdown 函数关闭且已注册为使用 RegisterApplicationRestart 函数重启的应用程序和服务。 此函数只能由调用 RmStartSession 函数的主安装程序调用，以启动重启管理器会话。
        /// </summary>
        /// <param name="dwSessionHandle">现有重启管理器会话的句柄。</param>
        /// <param name="dwRestartFlags">保留。 此参数应为 0。</param>
        /// <param name="fnStatus">指向状态消息回调函数的指针，该函数用于在 RmRestart 函数运行时传达状态。 如果 为 NULL，则不提供状态。</param>
        /// <returns>这是收到的最新错误。 函数可以返回 Winerror.h 中定义的 系统错误代码 之一。</returns>
        [LibraryImport(Rstrtmgr, EntryPoint = "RmRestart", SetLastError = false), PreserveSig]
        public static partial int RmRestart(uint dwSessionHandle, RM_SHUTDOWN_TYPE dwRestartFlags, RM_WRITE_STATUS_CALLBACK fnStatus);

        /// <summary>
        /// 启动应用程序的关闭。 只能从使用 RmStartSession 函数启动重启管理器会话的安装程序调用此函数。
        /// </summary>
        /// <param name="dwSessionHandle">现有 Restart Manager 会话的句柄。</param>
        /// <param name="lActionFlags">配置组件的关闭 的 一个或多个RM_SHUTDOWN_TYPE选项。 OR 运算符可以组合以下值，以指定当且仅当所有应用程序都已注册重启时，才会强制关闭无响应的应用程序和服务。</param>
        /// <param name="fnStatus">指向 RM_WRITE_STATUS_CALLBACK 函数的指针，该函数在执行时用于传达详细状态。 如果 为 NULL，则不提供任何状态。</param>
        /// <returns>这是收到的最新错误。 函数可以返回 Winerror.h 中定义的 系统错误代码 之一。</returns>
        [LibraryImport(Rstrtmgr, EntryPoint = "RmShutdown", SetLastError = false), PreserveSig]
        public static partial int RmShutdown(uint dwSessionHandle, RM_SHUTDOWN_TYPE lActionFlags, RM_WRITE_STATUS_CALLBACK fnStatus);

        /// <summary>
        /// 启动新的重启管理器会话。 每个用户会话最多可以同时在系统上打开 64 个重启管理器会话。 当此函数启动会话时，它将返回会话句柄和会话密钥，这些句柄和会话密钥可用于对重启管理器 API 的后续调用。
        /// </summary>
        /// <param name="pSessionHandle">指向重启管理器会话句柄的指针。 会话句柄可以在后续调用中传递给重启管理器 API。</param>
        /// <param name="dwSessionFlags">保留。 此参数应为 0。</param>
        /// <param name="strSessionKey">一个 以 null 结尾的字符串，其中包含新会话的会话密钥。 在调用 RmStartSession 函数之前，必须分配字符串。</param>
        /// <returns>这是收到的最新错误。 函数可以返回 Winerror.h 中定义的系统错误代码之一。</returns>
        [LibraryImport(Rstrtmgr, EntryPoint = "RmStartSession", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        public static partial int RmStartSession(out uint pSessionHandle, int dwSessionFlags, [MarshalAs(UnmanagedType.LPWStr)] string strSessionKey);

        /// <summary>
        /// 结束重启管理器会话。 此函数应由之前通过调用 RmStartSession 函数启动会话的主安装程序调用。 RmEndSession 函数可由加入会话的辅助安装程序调用，辅助安装程序无需再注册更多资源。
        /// </summary>
        /// <param name="pSessionHandle">现有 Restart Manager 会话的句柄。</param>
        /// <returns>这是收到的最新错误。 函数可以返回 Winerror.h 中定义的系统错误代码之一。</returns>
        [LibraryImport(Rstrtmgr, EntryPoint = "RmEndSession", SetLastError = false), PreserveSig]
        public static partial int RmEndSession(uint pSessionHandle);
    }
}
