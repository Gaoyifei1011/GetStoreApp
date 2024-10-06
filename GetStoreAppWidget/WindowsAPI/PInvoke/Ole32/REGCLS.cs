using System;

namespace GetStoreAppWidget.WindowsAPI.PInvoke.Ole32
{
    /// <summary>
    /// 控制与类对象的连接类型。
    /// </summary>
    [Flags]
    public enum REGCLS : uint
    {
        /// <summary>
        /// 使用 CoGetClassObject 将应用程序连接到类对象后，将从公共视图中删除该类对象，以便其他应用程序无法连接到该类对象。 此值通常用于单文档界面 (SDI) 应用程序。 指定此值不会影响对象应用程序调用 CoRevokeClassObject 的责任;使用对象类完成时，它必须始终调用 CoRevokeClassObject 。
        /// </summary>
        REGCLS_SINGLEUSE = 0x000000,

        /// <summary>
        /// 多个应用程序可以通过调用 CoGetClassObject 连接到类对象。 如果在对 CoRegisterClassObject 的调用中同时设置了REGCLS_MULTIPLEUSE和CLSCTX_LOCAL_SERVER，则无论是否显式设置CLSCTX_INPROC_SERVER，类对象也会自动注册为进程内服务器。
        /// </summary>
        REGCLS_MULTIPLEUSE = 0x000001,

        /// <summary>
        /// 用于通过调用 CoGetClassObject 注册单独的CLSCTX_LOCAL_SERVER和CLSCTX_INPROC_SERVER类工厂。 如果设置了REGCLS_MULTI_SEPARATE，则必须单独设置每个执行上下文; CoRegisterClassObject 不会自动注册进程外服务器 (CLSCTX_LOCAL_SERVER设置为进程内服务器) 。 这允许 EXE 根据进程内需求（例如自嵌入）创建对象的多个实例，而不会干扰其CLSCTX_LOCAL_SERVER注册。 如果 EXE 注册REGCLS_MULTI_SEPARATE类工厂和CLSCTX_INPROC_SERVER类工厂，则实例创建调用在 EXE 执行的 CLSCTX 参数中指定CLSCTX_INPROC_SERVER将在本地得到满足，而无需接近 SCM。 当 EXE 使用 OleCreate 和 OleLoad 等函数创建嵌入，但同时不希望为自嵌入情况启动自身的新实例时，此机制非常有用。 这一区别对于嵌入非常重要，因为默认处理程序默认聚合代理管理器，应用程序应通过调用 OleCreateEmbeddingHelper 来替代此默认行为，以处理自嵌入情况。
        /// 如果应用程序不需要区分本地和过程案例，则无需使用 REGCLS_MULTI_SEPARATE 注册类工厂。 事实上，当应用程序将其 MULTIPLEUSE 类工厂注册为MULTI_SEPARATE而不将另一个类工厂注册为INPROC_SERVER时，应用程序将产生额外的网络往返到 SCM。
        /// </summary>
        REGCLS_MULTI_SEPARATE = 0x000002,

        /// <summary>
        /// 暂停指定 CLSID 的注册和激活请求，直到调用 CoResumeClassObjects 为止。 这通常用于为可以注册多个类对象的服务器注册 CLSID，以便通过对 SCM 进行单个调用来减少总体注册时间，从而缩短服务器应用程序启动时间，无论为服务器注册了多少 CLSID。
        /// </summary>
        REGCLS_SUSPENDED = 0x000004,

        /// <summary>
        /// 类对象是用来运行 DLL 服务器的代理项进程。 代理进程注册的类工厂不是 DLL 服务器实现的实际类工厂，而是代理项实现的泛型类工厂。 此泛型类工厂将实例创建和封送委托给代理项中运行的 DLL 服务器的类工厂。 有关 DLL 代理项的详细信息，请参阅 DllSurrogate 注册表值。
        /// </summary>
        REGCLS_SURROGATE = 0x000008,

        /// <summary>
        /// 类对象聚合自由线程封送处理器和将对所有项目内公寓可见。 可以与其他标志一起使用。
        /// </summary>
        REGCLS_AGILE = 0x000010
    }
}
