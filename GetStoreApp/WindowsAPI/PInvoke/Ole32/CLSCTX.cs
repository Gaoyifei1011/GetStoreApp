namespace GetStoreApp.WindowsAPI.PInvoke.Ole32
{
    public enum CLSCTX : uint
    {
        /// <summary>
        /// 创建和管理此类的对象的代码是一个 DLL，它与指定类上下文的函数的调用方在同一个进程中运行。
        /// </summary>
        CLSCTX_INPROC_SERVER = 0x1,

        /// <summary>
        /// 创建和管理此类的对象的代码是一个 DLL，它与指定类上下文的函数的调用方在同一个进程中运行。
        /// </summary>
        CLSCTX_INPROC_HANDLER = 0x2,

        /// <summary>
        /// 创建和管理此类的对象的 EXE 代码在同一台计算机上运行，但在一个单独的进程空间中加载。
        /// </summary>
        CLSCTX_LOCAL_SERVER = 0x4,

        /// <summary>
        /// 已过时。
        /// </summary>
        CLSCTX_INPROC_SERVER16 = 0x8,

        /// <summary>
        /// 远程上下文。 创建和管理此类对象的 LocalServer32 或 LocalService 代码在不同的计算机上运行。
        /// </summary>
        CLSCTX_REMOTE_SERVER = 0x10,

        /// <summary>
        /// 已过时。
        /// </summary>
        CLSCTX_INPROC_HANDLER16 = 0x20,

        /// <summary>
        /// 保留。
        /// </summary>
        CLSCTX_RESERVED1 = 0x40,

        /// <summary>
        /// 保留。
        /// </summary>
        CLSCTX_RESERVED2 = 0x80,

        /// <summary>
        /// 保留。
        /// </summary>
        CLSCTX_RESERVED3 = 0x100,

        /// <summary>
        /// 保留。
        /// </summary>
        CLSCTX_RESERVED4 = 0x200,

        /// <summary>
        /// 禁用从目录服务或 Internet 下载代码。 此标志不能与CLSCTX_ENABLE_CODE_DOWNLOAD同时设置。
        /// </summary>
        CLSCTX_NO_CODE_DOWNLOAD = 0x400,

        /// <summary>
        /// 保留。
        /// </summary>
        CLSCTX_RESERVED5 = 0x800,

        /// <summary>
        /// 指定如果激活使用自定义封送，则指定激活是否失败。
        /// </summary>
        CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,

        /// <summary>
        /// 启用从目录服务或 Internet 下载代码。 此标志不能与 CLSCTX_NO_CODE_DOWNLOAD 同时设置。
        /// </summary>
        CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,

        /// <summary>
        /// CLSCTX_NO_FAILURE_LOG可用于替代 CoCreateInstanceEx 中失败的日志记录。
        /// 如果创建了 ActivationFailureLoggingLevel，以下值可以确定事件日志记录的状态：
        /// 0 = 自由日志记录。 默认情况下，日志，但客户端可以通过在 CoCreateInstanceEx 中指定CLSCTX_NO_FAILURE_LOG来替代。
        /// 1 = 无论客户端指定什么，始终记录所有故障。
        /// 2 = 从不记录任何故障，无论客户端指定什么。 如果缺少注册表项，则默认值为 0。 如果需要控制客户应用程序，建议将此值设置为 0，并编写客户端代码以替代失败。 强烈建议不要将值设置为 2。 如果禁用事件日志记录，则更难诊断问题。
        /// </summary>
        CLSCTX_NO_FAILURE_LOG = 0x4000,

        /// <summary>
        /// 仅对此激活禁用“作为激活者激活”(AAA)。 此标志替代EOLE_AUTHENTICATION_CAPABILITIES枚举中的EOAC_DISABLE_AAA标志的设置。 此标志不能与CLSCTX_ENABLE_AAA同时设置。 在调用方标识下启动服务器进程的任何激活称为激活即激活器 (AAA) 激活。 禁用 AAA 激活允许在特权帐户 (（如 LocalSystem) ）下运行的应用程序，以帮助防止其标识用于启动不受信任的组件。 使用激活调用的库应用程序应在这些调用期间始终设置此标志。 这有助于防止库应用程序在特权升级安全攻击中使用。 这是在库应用程序中禁用 AAA 激活的唯一方法，因为EOLE_AUTHENTICATION_CAPABILITIES枚举中的EOAC_DISABLE_AAA标志仅应用于服务器进程，而不是应用于库应用程序。
        /// </summary>
        CLSCTX_DISABLE_AAA = 0x8000,

        /// <summary>
        /// 仅对此激活禁用“作为激活者激活”(AAA)。 此标志替代EOLE_AUTHENTICATION_CAPABILITIES枚举中的EOAC_DISABLE_AAA标志的设置。 此标志不能与CLSCTX_ENABLE_AAA同时设置。 在调用方标识下启动服务器进程的任何激活称为激活即激活器 (AAA) 激活。 禁用 AAA 激活允许在特权帐户 (（如 LocalSystem) ）下运行的应用程序，以帮助防止其标识用于启动不受信任的组件。 使用激活调用的库应用程序应在这些调用期间始终设置此标志。 这有助于防止库应用程序在特权升级安全攻击中使用。 这是在库应用程序中禁用 AAA 激活的唯一方法，因为EOLE_AUTHENTICATION_CAPABILITIES枚举中的EOAC_DISABLE_AAA标志仅应用于服务器进程，而不是应用于库应用程序。
        /// </summary>
        CLSCTX_ENABLE_AAA = 0x10000,

        /// <summary>
        /// 从当前单元的默认上下文开始此激活。
        /// </summary>
        CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,

        /// <summary>
        /// 激活或连接到服务器的 32 位版本;如果未注册一个，则失败。
        /// </summary>
        CLSCTX_ACTIVATE_X86_SERVER = 0x40000,

        /// <summary>
        /// 激活或连接到服务器的 32 位版本;如果未注册一个，则失败。
        /// </summary>
        CLSCTX_ACTIVATE_32_BIT_SERVER = CLSCTX_ACTIVATE_X86_SERVER,

        /// <summary>
        /// 激活或连接到服务器的 64 位版本;如果未注册一个，则失败。
        /// </summary>
        CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,

        /// <summary>
        /// 指定此标志时，COM 使用线程的模拟令牌（如果存在）来获取线程发出的激活请求。 如果未指定此标志或线程没有模拟令牌，COM 将使用线程进程的进程令牌作为线程发出的激活请求。
        /// </summary>
        CLSCTX_ENABLE_CLOAKING = 0x100000,

        /// <summary>
        /// 指示激活适用于应用容器。此标志保留供内部使用，不打算直接从代码使用。
        /// </summary>
        CLSCTX_APPCONTAINER = 0x400000,

        /// <summary>
        /// 为As-Activator服务器指定交互式用户激活行为的此标志。 强名称的中型 IL Windows 应用商店应用可以使用此标志启动没有强名称的“激活器”COM 服务器。 此外，可以使用此标志绑定到由桌面应用程序启动的 COM 服务器的正在运行实例。
        /// 客户端必须是中等 IL，它必须强命名，这意味着它在客户端令牌中具有 SysAppID，不能位于会话 0 中，并且它必须与会话 ID 的用户位于客户端令牌中。
        /// 如果服务器进程外且“作为激活器”，它将使用客户端令牌会话用户的令牌启动服务器。 此令牌不会强命名。
        /// 如果服务器进程外且 RunAs 为“交互式用户”，则此标志不起作用。
        /// 如果服务器进程外且是任何其他 RunAs 类型，则激活会失败。
        /// 此标志对进程内服务器不起作用。
        /// 使用此标志时，计算机外激活会失败。
        /// </summary>
        CLSCTX_ACTIVATE_AAA_AS_IU = 0x800000,

        CLSCTX_RESERVED6 = 0x1000000,

        CLSCTX_ACTIVATE_ARM32_SERVER = 0x2000000,

        /// <summary>
        /// 用于加载代理/存根 DLL。此标志保留供内部使用，不打算直接从代码使用。
        /// </summary>
        CLSCTX_PS_DLL = 0x80000000,

        CLSCTX_ALL = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER
    }
}
