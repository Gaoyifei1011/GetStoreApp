using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 公开检索有关 Shell 项的信息的方法。 <see cref="IShellItem"> 和 IShellItem2 是任何新代码中项的首选表示形式。
    /// <see cref="IShellItem"> 接口继承自 IUnknown 接口。
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    public interface IShellItem
    {
        /// <summary>
        /// 绑定到由处理程序 ID 值 (BHID) 指定的项的处理程序。
        /// </summary>
        /// <param name="pbc">
        /// 指向绑定上下文对象上的 <see cref="IBindCtx"> 接口的指针。 用于将可选参数传递给处理程序。 绑定上下文的内容特定于处理程序。
        /// 例如，绑定到 BHID_Stream时，绑定上下文中的 STGM 标志指示 (读取或读/写) 所需的访问模式。
        /// </param>
        /// <param name="bhid">对 GUID 的引用，指定将创建哪个处理程序。</param>
        /// <param name="riid">要检索的对象类型的 IID。</param>
        /// <param name="ppv">此方法返回时，包含由 riid 指定的处理程序返回的类型 riid 的指针。</param>
        /// <returns>如果项相同，则返回S_OK项，如果项不同，则返回S_FALSE，否则返回错误值。</returns>
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object BindToHandler(IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, ref IntPtr ppv);

        /// <summary>
        /// 比较两个 <see cref="IShellItem"> 对象。
        /// </summary>
        /// <param name="psi">指向要与现有 <see cref="IShellItem"> 对象进行比较的 <see cref="IShellItem"> 对象的指针。</param>
        /// <param name="hint">确定如何执行比较的 <see cref="SICHINTF"> 值之一。有关此参数的可能值列表，请参阅 <see cref="SICHINTF">。</param>
        /// <returns>如果项相同，则返回S_OK项，如果项不同，则返回S_FALSE，否则返回错误值。</returns>
        int Compare(IShellItem psi, SICHINTF hint);

        /// <summary>
        /// 获取 <see cref="IShellItem"> 对象的请求属性集。
        /// </summary>
        /// <param name="sfgaoMask">指定要检索的属性。 一个或多个 <see cref="SFGAO"> 值。 使用按位 OR 运算符确定要检索的属性。</param>
        /// <returns>如果返回的属性与 sfgaoMask 中请求的属性完全匹配，则返回S_OK。如果属性不完全匹配，或者返回标准 COM 错误值，则返回S_FALSE。</returns>
        SFGAO GetAttributes(SFGAO sfgaoMask, out SFGAO psfgaoAttribs);

        /// <summary>
        /// 获取 <see cref="IShellItem"> 对象的显示名称。
        /// </summary>
        /// <param name="sigdnName">指示名称外观的 <see cref="SIGDN"> 值之一。</param>
        /// <param name="ppszName">一个值，当此函数成功返回时，接收指向检索的显示名称的指针的地址。</param>
        void GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        /// <summary>
        /// 获取 <see cref="IShellItem"> 对象的父对象。
        /// </summary>
        /// <param name="ppsi">指向 <see cref="IShellItem"> 接口父级的指针的地址。</param>
        /// <returns>如果成功，则返回 S_OK，否则返回错误值。</returns>
        IShellItem GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
    }
}
