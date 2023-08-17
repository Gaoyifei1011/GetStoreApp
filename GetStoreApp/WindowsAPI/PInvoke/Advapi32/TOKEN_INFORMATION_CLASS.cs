namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// TOKEN_INFORMATION_CLASS枚举包含的值指定要分配给访问令牌或从访问令牌检索的信息类型。
    /// </summary>
    public enum TOKEN_INFORMATION_CLASS
    {
        /// <summary>
        /// TOKEN_USER 包含令牌用户帐户的结构。
        /// </summary>
        TokenUser = 1,

        /// <summary>
        /// TOKEN_GROUPS 结构，其中包含与令牌关联的组帐户。
        /// </summary>
        TokenGroups = 2,

        /// <summary>
        /// TOKEN_PRIVILEGES 包含令牌特权的结构。
        /// </summary>
        TokenPrivileges = 3,

        /// <summary>
        /// TOKEN_OWNER 结构，其中包含新创建对象的默认所有者 安全标识符 (SID) 。
        /// </summary>
        TokenOwner = 4,

        /// <summary>
        /// TOKEN_PRIMARY_GROUP 结构，其中包含新创建的对象的默认主组 SID。
        /// </summary>
        TokenPrimaryGroup = 5,

        /// <summary>
        /// TOKEN_DEFAULT_DACL 结构，其中包含新创建对象的默认 DACL。
        /// </summary>
        TokenDefaultDacl = 6,

        /// <summary>
        /// TOKEN_SOURCE 包含令牌源的结构。 检索 此信息需要TOKEN_QUERY_SOURCE访问权限。
        /// </summary>
        TokenSource = 7,

        /// <summary>
        /// TOKEN_TYPE值，该值指示令牌是主令牌还是模拟令牌。
        /// </summary>
        TokenType = 8,

        /// <summary>
        /// SECURITY_IMPERSONATION_LEVEL 指示令牌的模拟级别的值。 如果访问令牌不是 模拟令牌，则函数将失败。
        /// </summary>
        TokenImpersonationLevel = 9,

        /// <summary>
        /// TOKEN_STATISTICS 包含各种令牌统计信息的结构。
        /// </summary>
        TokenStatistics = 10,

        /// <summary>
        /// TOKEN_GROUPS 结构，其中包含在 中限制 SID 的列表受限令牌。
        /// </summary>
        TokenRestrictedSids = 11,

        /// <summary>
        /// 缓冲区接收一个 DWORD 值，该值指示与令牌关联的终端服务会话标识符。
        /// 如果令牌与终端服务器客户端会话相关联，则会话标识符为非零。
        /// Windows Server 2003 和 Windows XP： 如果令牌与终端服务器控制台会话相关联，则会话标识符为零。
        /// 在非终端服务环境中，会话标识符为零。
        /// 如果使用 SetTokenInformation 设置 TokenSessionId，则应用程序必须具有“充当操作系统的一部分”权限，并且必须启用该应用程序才能在令牌中设置会话 ID。
        /// </summary>
        TokenSessionId = 12,

        /// <summary>
        /// 缓冲区接收 TOKEN_GROUPS_AND_PRIVILEGES 结构，其中包含用户 SID、组帐户、受限 SID 以及与令牌关联的身份验证 ID。
        /// </summary>
        TokenGroupsAndPrivileges = 13,

        /// <summary>
        /// 保留。
        /// </summary>
        TokenSessionReference = 14,

        /// <summary>
        /// 如果令牌包含 SANDBOX_INERT 标志，则缓冲区将收到非零的DWORD 值。
        /// </summary>
        TokenSandBoxInert = 15,

        /// <summary>
        /// 保留。
        /// </summary>
        TokenAuditPolicy = 16,

        /// <summary>
        /// 缓冲区接收 TOKEN_ORIGIN 值。
        /// 如果令牌是使用显式凭据（例如将名称、域和密码传递给 LogonUser 函数）生成的，则 TOKEN_ORIGIN 结构将包含创建它的 登录会话 的 ID。
        /// 如果令牌来自网络身份验证，例如调用 AcceptSecurityContext 或调用 LogonUser ，并将 dwLogonType 设置为 LOGON32_LOGON_NETWORK 或 LOGON32_LOGON_NETWORK_CLEARTEXT，则此值将为零。
        /// </summary>
        TokenOrigin = 17,

        /// <summary>
        /// 缓冲区接收 一个TOKEN_ELEVATION_TYPE 值，该值指定令牌的提升级别。
        /// </summary>
        TokenElevationType = 18,

        /// <summary>
        /// 缓冲区接收 TOKEN_LINKED_TOKEN 结构，该结构包含链接到此令牌的另一个令牌的句柄。
        /// </summary>
        TokenLinkedToken = 19,

        /// <summary>
        /// 缓冲区接收 一个TOKEN_ELEVATION 结构，该结构指定是否提升令牌。
        /// </summary>
        TokenElevation = 20,

        /// <summary>
        /// 如果已筛选令牌，缓冲区将收到非零的 DWORD 值。
        /// </summary>
        TokenHasRestrictions = 21,

        /// <summary>
        /// 缓冲区接收 TOKEN_ACCESS_INFORMATION 结构，该结构指定令牌中包含的安全信息。
        /// </summary>
        TokenAccessInformation = 22,

        /// <summary>
        /// 如果令牌允许虚拟化，则缓冲区会收到一个非零的 DWORD 值。
        /// </summary>
        TokenVirtualizationAllowed = 23,

        /// <summary>
        /// 如果为令牌启用了虚拟化，则缓冲区会收到一个非零的 DWORD 值。
        /// </summary>
        TokenVirtualizationEnabled = 24,

        /// <summary>
        /// 缓冲区接收指定令牌完整性级别的 TOKEN_MANDATORY_LABEL 结构。
        /// </summary>
        TokenIntegrityLevel = 25,

        /// <summary>
        /// 如果令牌设置了 UIAccess 标志，缓冲区将收到非零的 DWORD 值。
        /// </summary>
        TokenUIAccess = 26,

        /// <summary>
        /// 缓冲区接收 TOKEN_MANDATORY_POLICY 结构，该结构指定令牌的强制完整性策略。
        /// </summary>
        TokenMandatoryPolicy = 27,

        /// <summary>
        /// 缓冲区接收 TOKEN_GROUPS 结构，该结构指定令牌的登录 SID。
        /// </summary>
        TokenLogonSid = 28,

        /// <summary>
        /// 如果令牌是应用容器令牌，则缓冲区接收非零 的 DWORD 值。 检查 TokenIsAppContainer 并使其返回 0 的任何调用方还应验证调用方令牌是否不是标识级别模拟令牌。 如果当前令牌不是应用容器，而是标识级令牌，则应返回 AccessDenied。
        /// </summary>
        TokenIsAppContainer = 29,

        /// <summary>
        /// 缓冲区接收包含与令牌关联的功能的 TOKEN_GROUPS 结构。
        /// </summary>
        TokenCapabilities = 30,

        /// <summary>
        /// 缓冲区接收包含与令牌关联的 AppContainerSid 的 TOKEN_APPCONTAINER_INFORMATION 结构。 如果令牌未与应用容器关联，则TOKEN_APPCONTAINER_INFORMATION结构的 TokenAppContainer 成员指向 NULL。
        /// </summary>
        TokenAppContainerSid = 31,

        /// <summary>
        /// 缓冲区接收包含令牌的应用容器号的 DWORD 值。 对于不是应用容器令牌的令牌，此值为零。
        /// </summary>
        TokenAppContainerNumber = 32,

        /// <summary>
        /// 缓冲区接收 CLAIM_SECURITY_ATTRIBUTES_INFORMATION 结构，其中包含与令牌关联的用户声明。
        /// </summary>
        TokenUserClaimAttributes = 33,

        /// <summary>
        /// 缓冲区接收包含与令牌关联的设备声明 的 CLAIM_SECURITY_ATTRIBUTES_INFORMATION 结构。
        /// </summary>
        TokenDeviceClaimAttributes = 34,

        /// <summary>
        /// 此值是保留的。
        /// </summary>
        TokenRestrictedUserClaimAttributes = 35,

        /// <summary>
        /// 此值是保留的。
        /// </summary>
        TokenRestrictedDeviceClaimAttributes = 36,

        /// <summary>
        /// 缓冲区接收包含与令牌关联的设备组 的 TOKEN_GROUPS 结构。
        /// </summary>
        TokenDeviceGroups = 37,

        /// <summary>
        /// 缓冲区接收 TOKEN_GROUPS 结构，其中包含与令牌关联的受限设备组。
        /// </summary>
        TokenRestrictedDeviceGroups = 38,

        /// <summary>
        /// 此值是保留的。
        /// </summary>
        TokenSecurityAttributes = 39,

        /// <summary>
        /// 此值是保留的。
        /// </summary>
        TokenIsRestricted = 40,

        TokenProcessTrustLevel = 41,

        TokenPrivateNameSpace = 42,

        TokenSingletonAttributes = 43,

        TokenBnoIsolation = 44,

        TokenChildProcessFlags = 45,

        /// <summary>
        /// 指最低特权 AppContainer (LPAC) 。 LPAC 实际上是被 ALL_APPLICATION_PACKAGES SID 忽略的 AppContainer。 有关什么是 AppContainer 的信息，请参阅 适用于旧版应用的 AppContainer。
        /// </summary>
        TokenIsLessPrivilegedAppContainer = 46,

        TokenIsSandboxed = 47,

        TokenIsAppSilo = 48,

        MaxTokenInfoClass,
    }
}
