using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 使你能够使用名字对象，该对象包含唯一标识 COM 对象的信息。 具有指向名字对象 <see cref="IMoniker"> 接口的指针的对象可以查找、激活和获取对已标识对象的访问，而无需获取有关该对象实际位于分布式系统中的任何其他特定信息。
    /// 名字对象用作在 COM 中链接的基础。 链接对象包含标识其源的名字对象。 当用户激活链接的对象以对其进行编辑时，将绑定名字对象;这会将链接源加载到内存中。
    /// </summary>
    [Guid("0000000f-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IMoniker
    {
        /// <summary>
        /// 检索对象的 CLSID (类标识符) 。
        /// </summary>
        /// <param name="pClassID">指向返回时接收 CLSID 的位置的指针。 CLSID 是一个全局唯一标识符 (GUID) ，它唯一表示一个对象类，该类定义可以操作对象的数据的代码。</param>
        void GetClassID(out Guid pClassID);

        /// <summary>
        /// 确定对象自上次保存到其流以来是否已更改。
        /// </summary>
        /// <returns>此方法返回 S_OK 以指示对象已更改。否则，它将返回S_FALSE。</returns>
        [PreserveSig]
        int IsDirty();

        /// <summary>
        /// 从以前保存的流中初始化对象。
        /// </summary>
        /// <param name="pStm">一个 <see cref="IStream"> 指针，指向应从中加载对象的流。</param>
        void Load(IStream pStm);

        /// <summary>
        /// 将对象保存到指定流。
        /// </summary>
        /// <param name="pStm">一个 <see cref="IStream"> 指针，指向应将对象保存到其中的流。</param>
        /// <param name="fClearDirty">指示在保存完成后是否清除已更新标志。 如果 为 TRUE，应清除该标志。 如果 为 FALSE，则标志应保持不变。</param>
        void Save(IStream pStm, [MarshalAs(UnmanagedType.Bool)] bool fClearDirty);

        /// <summary>
        /// 检索保存对象所需的流的大小。
        /// </summary>
        /// <param name="pcbSize">保存此对象所需的流的大小（以字节为单位）。</param>
        void GetSizeMax(out long pcbSize);

        /// <summary>
        /// 绑定到指定的对象。 绑定过程涉及查找对象、根据需要将其置于运行状态，并向调用方提供指向已标识对象的指定接口的指针。
        /// </summary>
        /// <param name="pbc">指向绑定上下文对象上的 <see cref="IBindCtx"> 接口的指针，该对象用于此绑定操作。 绑定上下文缓存绑定过程中绑定的对象，包含使用绑定上下文应用于所有操作的参数，并提供名字对象实现应检索其环境的相关信息的方法。</param>
        /// <param name="pmkToLeft">如果名字对象是复合名字对象的一部分，则指向此名字对象左侧的名字对象指针。 此参数主要由名字对象实现者用来在复合名字对象的各个组件之间进行合作。 名字对象客户端应使用 NULL。</param>
        /// <param name="riidResult">客户端希望用来与名字对象标识的对象通信的接口 IID。</param>
        /// <param name="ppvResult">接收 riid 中请求的接口指针的指针变量的地址。 成功返回后，<param name="ppvResult"> 包含指向名字对象标识的对象的请求接口指针。 成功后，实现必须在名字对象上调用 AddRef 。 调用方负责调用 Release。 如果发生错误，<param name="ppvResult"> 应为 NULL。</param>
        void BindToObject(IBindCtx pbc, IMoniker pmkToLeft, [In()] ref Guid riidResult, [MarshalAs(UnmanagedType.Interface)] out object ppvResult);

        /// <summary>
        /// 绑定到指定对象的存储。 与 <see cref="BindToObject"> 方法不同，此方法不会激活名字对象标识的对象。
        /// </summary>
        /// <param name="pbc">指向绑定上下文对象上的 <see cref="IBindCtx"> 接口的指针，该对象用于此绑定操作。 绑定上下文缓存绑定过程中绑定的对象，包含使用绑定上下文应用于所有操作的参数，并提供名字对象实现应检索其环境的相关信息的方法。</param>
        /// <param name="pmkToLeft">如果名字对象是复合名字对象的一部分，则指向此名字对象左侧的名字对象指针。 此参数主要由名字对象实现者用来在复合名字对象的各个组件之间进行合作。 名字对象客户端应使用 NULL。</param>
        /// <param name="riid">对请求的存储接口的标识符的引用，其指针将在 ppvObj 中返回。 通常请求的存储接口包括 IStorage、 <see cref="IStream"> 和 ILockBytes。</param>
        /// <param name="ppvObj">接收 riid 中请求的接口指针的指针变量的地址。 成功返回后，<param name="ppvObj"> 包含指向名字对象所标识对象的存储的请求接口指针。
        /// 成功后，实现必须在存储上调用 AddRef 。 调用方负责调用 Release。 如果发生错误，<param name="ppvObj"> 应为 NULL。</param>
        void BindToStorage(IBindCtx pbc, IMoniker pmkToLeft, [In()] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObj);

        /// <summary>
        /// 将名字对象减少为最简单的形式。
        /// </summary>
        /// <param name="pbc">指向绑定上下文上要用于此绑定操作的 <see cref="IBindCtx"> 接口的指针。 绑定上下文缓存绑定过程中绑定的对象，包含使用绑定上下文应用于所有操作的参数，并提供名字对象实现应检索其环境的相关信息的方法。</param>
        /// <param name="dwReduceHowFar">指定该名字对象应该简化的程度。 此参数必须是 MKRREDUCE 枚举中的值之一。</param>
        /// <param name="ppmkToLeft">
        /// 在条目中，指向 <see cref="IMoniker"> 指针变量的指针，该变量包含指向此名字对象左侧的接口指针。 此参数主要由名字对象实现者用于启用复合名字对象的各个组件之间的合作：名字对象客户端通常可以传递 NULL。
        ///返回时，<param name="ppmkToLeft"> 通常设置为 NULL，指示原始名字对象在左侧没有更改。 在极少数情况下，<param name="ppmkToLeft"> 表示名字对象，指示应忽略左侧的上一个名字对象，并且通过 <param name="ppmkToLeft"> 返回的名字对象是替换项。 在这种情况下，实现必须调用此名字对象左侧的“ 发布 ”，并且必须在新的返回名字对象上调用 AddRef ：调用方以后必须释放它。 如果发生错误，则实现可以保留接口指针不变或将其设置为 NULL。
        /// </param>
        /// <param name="ppmkReduced">指向 <see cref="IMoniker"> 指针变量的指针，该变量接收指向此名字对象形式减少的接口指针，如果发生错误，或者此名字对象已减少为无，则为 NULL 。 如果无法减少此名字对象，<param name="ppmkReduced"> 只是设置为此名字对象，并且返回值MK_S_REDUCED_TO_SELF。 如果 <param name="ppmkReduced"> 为非 NULL，则实现必须在新的名字对象上调用 AddRef ;调用方负责调用 发布。 (即使 <param name="ppmkReduced"> 设置为此名字对象.) 也是如此</param>
        void Reduce(IBindCtx pbc, int dwReduceHowFar, ref IMoniker ppmkToLeft, out IMoniker ppmkReduced);

        /// <summary>
        /// 通过将当前名字对象与指定的名字对象组合在一起，创建新的复合名字对象。
        /// </summary>
        /// <param name="pmkRight">指向名字对象上的 <see cref="IMoniker"> 接口的指针，用于撰写到此名字对象末尾。</param>
        /// <param name="fOnlyIfNotGeneric">如果 为 TRUE，则调用方需要非泛型组合，因此只有在 <param name="pmkRight"> 是一个名字对象类，此名字对象可以采用某种方式组合，而不是形成泛型复合时，该操作才应该继续。 如果 为 FALSE，该方法可以在必要时创建泛型复合。 大多数调用方应将此参数设置为 FALSE。</param>
        /// <param name="ppmkComposite">指向接收复合名字对象指针的 <see cref="IMoniker"> 指针变量的指针。 成功时，实现必须在生成的名字对象上调用 AddRef ;调用方负责调用 Release。 如果发生错误，或者如果名字对象撰写为无 (，则使用项名字对象或文件名字对象) 组合反名字对象，<param name="ppmkComposite"> 应设置为 NULL。</param>
        void ComposeWith(IMoniker pmkRight, [MarshalAs(UnmanagedType.Bool)] bool fOnlyIfNotGeneric, out IMoniker ppmkComposite);

        /// <summary>
        /// 检索指向复合名字对象组件的枚举器的指针。
        /// </summary>
        /// <param name="fForward">如果为 TRUE，则从左到右枚举名字对象。如果为 FALSE，则从右到左枚举。</param>
        /// <param name="ppenumMoniker">指向 <see cref="IEnumMoniker"> 指针变量的指针，该指针变量接收指向名字对象的枚举器对象的接口指针。成功后，实现必须在枚举器对象上调用 AddRef。调用方有责任调用“发布”。如果发生错误或名字对象没有可枚举的组件，则实现会将 <param name="ppenumMoniker"> 设置为 NULL。</param>
        void Enum([MarshalAs(UnmanagedType.Bool)] bool fForward, out IEnumMoniker ppenumMoniker);

        /// <summary>
        /// 确定此名字对象是否与指定的名字对象相同。
        /// </summary>
        /// <param name="pmkOtherMoniker">指向名字对象上的 <see cref="IMoniker"> 接口的指针，用于与此接口进行比较 (此方法) 。</param>
        /// <returns>此方法返回S_OK，指示两个名字对象是相同的，否则S_FALSE。</returns>
        [PreserveSig]
        int IsEqual(IMoniker pmkOtherMoniker);

        /// <summary>
        /// 使用名字对象的内部状态创建哈希值。
        /// </summary>
        /// <param name="pdwHash">指向接收哈希值的变量的指针。</param>
        void Hash(out int pdwHash);

        /// <summary>
        /// 确定此名字对象标识的对象当前是否已加载并运行。
        /// </summary>
        /// <param name="pbc">指向绑定上下文上要用于此绑定操作的 <see cref="IBindCtx"> 接口的指针。 绑定上下文缓存绑定过程中绑定的对象，包含使用绑定上下文应用于所有操作的参数，并提供名字对象实现应检索其环境的相关信息的方法。</param>
        /// <param name="pmkToLeft">指向此名字对象左侧的 <see cref="IBindCtx"> 接口的指针（如果此名字对象是复合的一部分）。 此参数主要由名字对象实现者用于启用复合名字对象的各个组件之间的合作：名字对象客户端通常可以传递 NULL。</param>
        /// <param name="pmkNewlyRunning">指向最近添加到正在运行的对象表的 <see cref="IBindCtx"> 接口的指针， (ROT) 。 这可以是 NULL。 如果为非 NULL，则实现可以返回在 <param name="pmkNewlyRunning"> 参数上调用 <see cref="IsEqual"> 的结果，并传递当前名字对象。 此参数旨在启用比搜索 ROT 更高效的 <see cref="IsRunning"> 实现，但实现可以选择忽略 <param name="pmkNewlyRunning"> 而不造成任何伤害。</param>
        /// <returns>
        /// 此方法可以返回标准返回值E_UNEXPECTED，以及以下值。
        /// 此方法返回S_OK，指示名字对象正在运行，否则S_FALSE。
        /// </returns>
        [PreserveSig]
        int IsRunning(IBindCtx pbc, IMoniker pmkToLeft, IMoniker pmkNewlyRunning);

        /// <summary>
        /// 检索上次更改此名字对象标识的对象的时间。
        /// </summary>
        /// <param name="pbc">指向此绑定操作中使用的绑定上下文的指针。 绑定上下文缓存绑定过程中绑定的对象，包含使用绑定上下文应用于所有操作的参数，并提供名字对象实现应检索其环境的相关信息的方法。 有关详细信息，请参阅 <see cref="IBindCtx">。</param>
        /// <param name="pmkToLeft">如果名字对象是复合名字对象的一部分，则指向此名字对象左侧的名字对象指针。 此参数主要由名字对象实现者用来在复合名字对象的各个组件之间进行合作。 名字对象客户端应传递 NULL。</param>
        /// <param name="pFileTime">指向接收上次更改时间的 <see cref="FILETIME"> 结构的指针。 {0xFFFFFFFF，0x7FFFFFFF} 的值指示错误 (例如超出时间限制、信息不可用) 。</param>
        void GetTimeOfLastChange(IBindCtx pbc, IMoniker pmkToLeft, out FILETIME pFileTime);

        /// <summary>
        /// 创建一个名字对象，该名字对象是此名字的反函数。 当组合到此名字对象或类似结构之一的右侧时，名字对象将不构成任何内容。
        /// </summary>
        /// <param name="ppmk"><see cref="IMoniker"> 指针变量的地址，该变量接收接口指针到此名字对象反义的名字对象。 成功后，实现必须在新的反向名字对象上调用 AddRef 。 调用方负责呼叫 发布。 如果发生错误，则实现应将 <param name="ppmk"> 设置为 NULL。</param>
        void Inverse(out IMoniker ppmk);

        /// <summary>
        /// 基于此名字对象与指定名字对象通用的前缀创建新的名字对象。
        /// </summary>
        /// <param name="pmkOther">指向另一个名字对象上的 <see cref="IMoniker"> 接口的指针，用于确定是否存在通用前缀。</param>
        /// <param name="ppmkPrefix"><see cref="IMoniker"> 指针变量的地址，该变量接收指向此名字对象和 <param name="pmkOther"> 的常见前缀的接口指针。
        /// 成功时，实现必须在生成的名字对象上调用 AddRef ;调用方负责调用 Release。
        /// 如果发生错误或没有常见前缀，则实现应将 <param name="ppmkPrefix"> 设置为 NULL。</param>
        void CommonPrefixWith(IMoniker pmkOther, out IMoniker ppmkPrefix);

        /// <summary>
        /// 在此名字对象与指定的名字对象之间创建相对名字对象。
        /// </summary>
        /// <param name="pmkOther">指向应将其作为相对路径的名字对象上的 <see cref="IMoniker"> 接口的指针。</param>
        /// <param name="ppmkRelPath">指向 <see cref="IMoniker"> 指针变量的指针，该变量接收指向相对名字对象的接口指针。 成功后，实现必须在新的名字对象上调用 AddRef ;调用方负责调用 Release。 如果发生错误，则实现会将 <param name="ppmkRelPath"> 设置为 NULL。</param>
        void RelativePathTo(IMoniker pmkOther, out IMoniker ppmkRelPath);

        /// <summary>
        /// 检索名字对象的显示名称。
        /// </summary>
        /// <param name="pbc">指向绑定上下文上要用于此操作的 <see cref="IBindCtx"> 接口的指针。 绑定上下文缓存绑定过程中绑定的对象，包含使用绑定上下文应用于所有操作的参数，并提供名字对象实现应检索其环境的相关信息的方法。</param>
        /// <param name="pmkToLeft">如果名字对象是复合名字对象的一部分，则指向此名字对象左侧的名字对象指针。 此参数主要由名字对象实现者用来在复合名字对象的各个组件之间进行合作。 名字对象客户端应传递 NULL。</param>
        /// <param name="ppszDisplayName">指针变量的地址，该变量接收指向名字对象的显示名称字符串的指针。 实现必须使用 IMalloc::Alloc 分配 在 <param name="ppszDisplayName"> 中返回的字符串，调用方负责调用 IMalloc::Free 来释放它。 调用方和此方法的实现都使用 CoGetMalloc 返回的 COM 任务分配器。 如果发生错误，则实现必须将 <param name="ppszDisplayName"> 设置为 NULL。</param>
        void GetDisplayName(IBindCtx pbc, IMoniker pmkToLeft, [MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplayName);

        /// <summary>
        /// 将显示名称转换为名字对象。
        /// </summary>
        /// <param name="pbc">指向绑定上下文中要用于此绑定操作的 <see cref="IBindCtx"> 接口的指针。 绑定上下文缓存绑定过程中绑定的对象，包含使用绑定上下文应用于所有操作的参数，并提供名字对象实现应检索其环境的相关信息的方法。</param>
        /// <param name="pmkToLeft">指向名字对象上 <see cref="IMoniker"> 接口的指针，该接口已从显示名称生成到此点。</param>
        /// <param name="pszDisplayName">要分析的剩余显示名称。</param>
        /// <param name="pchEaten">指向一个变量的指针，该变量接收此步骤中消耗的 <param name="pszDisplayName"> 中的字符数。</param>
        /// <param name="ppmkOut">指向 <see cref="IMoniker"> 指针变量的指针，该变量接收从 <param name="pszDisplayName"> 生成的名字对象接口指针。 成功后，实现必须在新的名字对象上调用 AddRef ;调用方负责调用 Release。 如果发生错误，则实现会将 <param name="ppmkOut"> 设置为 NULL。</param>
        void ParseDisplayName(IBindCtx pbc, IMoniker pmkToLeft, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out int pchEaten, out IMoniker ppmkOut);

        /// <summary>
        /// 确定此名字对象是否为系统提供的名字对象类之一。
        /// </summary>
        /// <param name="pdwMksys">指向从 MKSYS 枚举接收其中一个值的变量的指针，并引用 COM 名字对象类之一。 此参数不能为 NULL。</param>
        /// <returns>此方法返回S_OK，以指示名字对象是系统名字对象，否则S_FALSE。</returns>
        [PreserveSig]
        int IsSystemMoniker(out int pdwMksys);
    }
}
