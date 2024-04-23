using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Ole32
{
    /// <summary>
    /// Ole32.dll 函数库
    /// </summary>
    public static partial class Ole32Library
    {
        private const string Ole32 = "ole32.dll";

        /// <summary>
        /// 创建和默认初始化与指定 CLSID 关联的类的单个对象。
        /// 如果只想在本地系统上创建一个对象，请调用 CoCreateInstance 。 若要在远程系统上创建单个对象，请调用 CoCreateInstanceEx 函数。 若要基于单个 CLSID 创建多个对象，请调用 CoGetClassObject 函数。
        /// </summary>
        /// <param name="rclsid">与要用于创建对象的数据和代码关联的 CLSID。</param>
        /// <param name="pUnkOuter">
        /// 如果 为 NULL，则指示对象未作为聚合的一部分创建。 如果为非 NULL，则指向聚合对象的 IUnknown 接口的指针 (控制 IUnknown) 。
        /// </param>
        /// <param name="dwClsContext">管理新创建对象的代码将在其中运行的上下文。 这些值取自枚举 CLSCTX。</param>
        /// <param name="riid">对要用于与对象通信的接口标识符的引用。</param>
        /// <param name="ppv">接收 riid 中请求的接口指针的指针变量的地址。 成功返回后，*ppv 包含请求的接口指针。 失败后，*ppv 包含 NULL。</param>
        [LibraryImport(Ole32, EntryPoint = "CoCreateInstance", SetLastError = false)]
        public static partial int CoCreateInstance(ref Guid rclsid, IntPtr pUnkOuter, CLSCTX dwClsContext, ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// 初始化 COM 库以供调用线程使用，设置线程的并发模型，并根据需要为线程创建一个新单元。
        /// </summary>
        /// <param name="pvReserved">此参数是保留的，必须为 NULL。</param>
        /// <param name="dwCoInit">线程的并发模型和初始化选项。 此参数的值取自 COINIT 枚举。 可以使用 COINIT 中值的任意组合，但不能同时设置COINIT_APARTMENTTHREADED和COINIT_MULTITHREADED标志。 默认值为 COINIT_MULTITHREADED。</param>
        /// <returns></returns>
        [LibraryImport(Ole32, EntryPoint = "CoInitializeEx", SetLastError = false)]
        public static partial int CoInitializeEx(IntPtr pvReserved, COINIT dwCoInit);

        /// <summary>
        /// 设置将用于对指定代理进行调用的身份验证信息。 这是 IClientSecurity：：SetBlanket 的帮助程序函数。
        /// </summary>
        /// <param name="punk">要设置的代理。</param>
        /// <param name="dwAuthnSvc">要使用的身份验证服务。 有关可能值的列表，请参阅 身份验证服务常量。 如果不需要身份验证，请使用 RPC_C_AUTHN_NONE。 如果指定了RPC_C_AUTHN_DEFAULT，DCOM 将按照其常规安全一揽子协商算法选择身份验证服务。</param>
        /// <param name="dwAuthzSvc">要使用的授权服务。 有关可能值的列表，请参阅 授权常量。 如果指定了RPC_C_AUTHZ_DEFAULT，DCOM 将按照其正常的安全一揽子协商算法选择授权服务。 如果使用 NTLMSSP、Kerberos 或 Schannel 作为身份验证服务，则应将RPC_C_AUTHZ_NONE用作授权服务。</param>
        /// <param name="pServerPrincName">
        /// 要与身份验证服务一起使用的服务器主体名称。 如果指定了COLE_DEFAULT_PRINCIPAL，DCOM 将使用其安全总协商算法选取主体名称。 如果使用 Kerberos 作为身份验证服务，则此值不得为 NULL。 它必须是服务器的正确主体名称，否则调用将失败。
        /// 如果使用 Schannel 作为身份验证服务，则此值必须是 主体名称中所述的 msstd 或 fullsic 形式之一; 如果不需要相互身份验证，此值必须为 NULL 。
        /// 通常，指定 NULL 不会重置代理上的服务器主体名称;相反，将保留以前的设置。 在为代理选择其他身份验证服务时，使用 NULL 作为 pServerPrincName 时必须小心，因为无法保证以前设置的主体名称对新选择的身份验证服务有效。
        /// </param>
        /// <param name="dwAuthLevel">要使用的身份验证级别。 有关可能值的列表，请参阅 身份验证级别常量。 如果指定了RPC_C_AUTHN_LEVEL_DEFAULT，DCOM 将按照其常规安全一揽子协商算法选择身份验证级别。 如果此值为 none，则身份验证服务也必须为 none。</param>
        /// <param name="dwImpLevel">要使用的模拟级别。 有关可能值的列表，请参阅 模拟级别常量。 如果指定了RPC_C_IMP_LEVEL_DEFAULT，DCOM 将按照其正常的安全一揽子协商算法选择模拟级别。 如果 NTLMSSP 是身份验证服务，则必须RPC_C_IMP_LEVEL_IMPERSONATE或RPC_C_IMP_LEVEL_IDENTIFY此值。 NTLMSSP 还支持同一计算机上的委托级模拟 (RPC_C_IMP_LEVEL_DELEGATE) 。 如果 Schannel 是身份验证服务，则必须RPC_C_IMP_LEVEL_IMPERSONATE此参数。</param>
        /// <param name="pAuthInfo">
        /// 指向建立客户端标识 的RPC_AUTH_IDENTITY_HANDLE 值的指针。 句柄引用的结构的格式取决于身份验证服务的提供程序。
        /// 对于同一计算机上的调用，RPC 使用提供的凭据记录用户，并使用生成的令牌进行方法调用。
        /// 对于 NTLMSSP 或 Kerberos，结构是 SEC_WINNT_AUTH_IDENTITY 或 SEC_WINNT_AUTH_IDENTITY_EX 结构。 客户端可以在调用 API 后放弃 pAuthInfo 。 RPC 不保留 pAuthInfo 指针的副本，并且客户端稍后无法在 CoQueryProxyBlanket 方法中检索它。
        /// 如果此参数为 NULL，DCOM 将使用当前代理标识(该标识是进程令牌或模拟令牌) 。 如果句柄引用结构，则使用该标识。
        /// 对于 Schannel，此参数必须是指向包含客户端 X.509 证书的 CERT_CONTEXT 结构的指针，如果客户端希望与服务器建立匿名连接，则此参数必须为 NULL 。 如果指定了证书，则只要当前单元中存在对象的任何代理，调用方就不得释放该证书。
        /// 对于 Snego，此成员为 NULL，指向 SEC_WINNT_AUTH_IDENTITY 结构，或指向 SEC_WINNT_AUTH_IDENTITY_EX 结构。 如果为 NULL，Snego 将根据客户端计算机上可用的身份验证服务选择一个列表。 如果它指向 SEC_WINNT_AUTH_IDENTITY_EX 结构，则结构的 PackageList 成员必须指向包含以逗号分隔的身份验证服务名称列表的字符串， 而 PackageListLength 成员必须提供 PackageList 字符串中的字节数。 如果 PackageList 为 NULL，则使用 Snego 的所有调用都将失败。
        /// 如果为此参数指定了COLE_DEFAULT_AUTHINFO，则 DCOM 将按照其常规安全一揽子协商算法选择身份验证信息。
        /// 如果设置了 pAuthInfo 并在 dwCapabilities 参数中设置了其中一个隐藏标志，CoSetProxyBlanket 将失败。
        /// </param>
        /// <param name="dwCapabilities">此代理的功能。 有关可能值的列表，请参阅 EOLE_AUTHENTICATION_CAPABILITIES 枚举。 唯一可以通过此函数设置的标志是EOAC_MUTUAL_AUTH、EOAC_STATIC_CLOAKING、EOAC_DYNAMIC_CLOAKING，EOAC_ANY_AUTHORITY (此标志已弃用) 、EOAC_MAKE_FULLSIC和EOAC_DEFAULT。 如果未设置 pAuthInfo 且 Schannel 不是身份验证服务，则可以设置 EOAC_STATIC_CLOAKING 或 EOAC_DYNAMIC_CLOAKING。 (有关详细信息，请参阅 隐藏 。) 如果设置了此处提及的功能以外的任何功能标志， CoSetProxyBlanket 将失败。</param>
        [LibraryImport(Ole32, EntryPoint = "CoSetProxyBlanket", SetLastError = false)]
        public static partial int CoSetProxyBlanket(IntPtr punk, uint dwAuthnSvc, uint dwAuthzSvc, IntPtr pServerPrincName, uint dwAuthLevel, uint dwImpLevel, IntPtr pAuthInfo, uint dwCapabilities);

        /// <summary>
        /// 关闭当前线程上的 COM 库，卸载线程加载的所有 DLL，释放线程维护的任何其他资源，并强制关闭线程上的所有 RPC 连接。
        /// </summary>
        [LibraryImport(Ole32, EntryPoint = "CoUninitialize", SetLastError = false)]
        public static partial void CoUninitialize();
    }
}
