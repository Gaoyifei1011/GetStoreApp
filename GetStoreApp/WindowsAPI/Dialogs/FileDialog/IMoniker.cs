using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    [Guid("0000000f-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IMoniker
    {
        /// <summary>
        /// 绑定到指定的对象。 绑定过程涉及查找对象、根据需要将其置于运行状态，并向调用方提供指向已标识对象的指定接口的指针。
        /// </summary>
        /// <param name="pbc">指向绑定上下文对象上的 <see cref="IBindCtx"> 接口的指针，该对象用于此绑定操作。 绑定上下文缓存绑定过程中绑定的对象，包含使用绑定上下文应用于所有操作的参数，并提供名字对象实现应检索其环境的相关信息的方法。</param>
        /// <param name="pmkToLeft">如果名字对象是复合名字对象的一部分，则指向此名字对象左侧的名字对象指针。 此参数主要由名字对象实现者用来在复合名字对象的各个组件之间进行合作。 名字对象客户端应使用 NULL。</param>
        /// <param name="riidResult">客户端希望用来与名字对象标识的对象通信的接口 IID。</param>
        /// <param name="ppvResult">接收 riid 中请求的接口指针的指针变量的地址。 成功返回后，*ppvResult 包含指向名字对象标识的对象的请求接口指针。 成功后，实现必须在名字对象上调用 AddRef 。 调用方负责调用 Release。 如果发生错误，*ppvResult 应为 NULL。</param>
        void BindToObject(IBindCtx pbc, IMoniker pmkToLeft, [In()] ref Guid riidResult, [MarshalAs(UnmanagedType.Interface)] out object ppvResult);

        /// <summary>
        /// 绑定到指定对象的存储。 与 <see cref="BindToObject"> 方法不同，此方法不会激活名字对象标识的对象。
        /// </summary>
        /// <param name="pbc">指向绑定上下文对象上的 <see cref="IBindCtx"> 接口的指针，该对象用于此绑定操作。 绑定上下文缓存绑定过程中绑定的对象，包含使用绑定上下文应用于所有操作的参数，并提供名字对象实现应检索其环境的相关信息的方法。</param>
        /// <param name="pmkToLeft">如果名字对象是复合名字对象的一部分，则指向此名字对象左侧的名字对象指针。 此参数主要由名字对象实现者用来在复合名字对象的各个组件之间进行合作。 名字对象客户端应使用 NULL。</param>
        /// <param name="riid">对请求的存储接口的标识符的引用，其指针将在 ppvObj 中返回。 通常请求的存储接口包括 IStorage、 IStream 和 ILockBytes。</param>
        /// <param name="ppvObj">接收 riid 中请求的接口指针的指针变量的地址。 成功返回后，*ppvObj 包含指向名字对象所标识对象的存储的请求接口指针。
        /// 成功后，实现必须在存储上调用 AddRef 。 调用方负责调用 Release。 如果发生错误，*ppvObj 应为 NULL。</param>
        void BindToStorage(IBindCtx pbc, IMoniker pmkToLeft, [In()] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObj);

        /// <summary>
        /// 基于此名字对象与指定名字对象通用的前缀创建新的名字对象。
        /// </summary>
        /// <param name="pmkOther">指向另一个名字对象上的 <see cref="IMoniker"> 接口的指针，用于确定是否存在通用前缀。</param>
        /// <param name="ppmkPrefix"><see cref="IMoniker"> 指针变量的地址，该变量接收指向此名字对象和 <param name="pmkOther"> 的常见前缀的接口指针。
        /// 成功时，实现必须在生成的名字对象上调用 AddRef ;调用方负责调用 Release。
        /// 如果发生错误或没有常见前缀，则实现应将 <param name="ppmkPrefix"> 设置为 NULL。</param>
        void CommonPrefixWith(IMoniker pmkOther, out IMoniker ppmkPrefix);

        // IPersist portion
        void GetClassID(out Guid pClassID);

        // IPersistStream portion
        [PreserveSig]
        int IsDirty();

        void Load(IStream pStm);

        void Save(IStream pStm, [MarshalAs(UnmanagedType.Bool)] bool fClearDirty);

        void GetSizeMax(out long pcbSize);

        void Reduce(IBindCtx pbc, int dwReduceHowFar, ref IMoniker ppmkToLeft, out IMoniker ppmkReduced);

        void ComposeWith(IMoniker pmkRight, [MarshalAs(UnmanagedType.Bool)] bool fOnlyIfNotGeneric, out IMoniker ppmkComposite);

        void Enum([MarshalAs(UnmanagedType.Bool)] bool fForward, out IEnumMoniker ppenumMoniker);

        [PreserveSig]
        int IsEqual(IMoniker pmkOtherMoniker);

        void Hash(out int pdwHash);

        [PreserveSig]
        int IsRunning(IBindCtx pbc, IMoniker pmkToLeft, IMoniker pmkNewlyRunning);

        void GetTimeOfLastChange(IBindCtx pbc, IMoniker pmkToLeft, out FILETIME pFileTime);

        void Inverse(out IMoniker ppmk);

        void RelativePathTo(IMoniker pmkOther, out IMoniker ppmkRelPath);

        void GetDisplayName(IBindCtx pbc, IMoniker pmkToLeft, [MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplayName);

        void ParseDisplayName(IBindCtx pbc, IMoniker pmkToLeft, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out int pchEaten, out IMoniker ppmkOut);

        [PreserveSig]
        int IsSystemMoniker(out int pdwMksys);
    }
}
