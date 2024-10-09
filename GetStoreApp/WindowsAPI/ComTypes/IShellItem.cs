using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开检索有关 Shell 项的信息的方法。 IShellItem 和 IShellItem2 是任何新代码中项的首选表示形式。
    /// IShellItem 接口继承自 IUnknown 接口。
    /// </summary>
    [GeneratedComInterface, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
    public partial interface IShellItem
    {
        /// <summary>
        /// 绑定到由处理程序 ID 值 (BHID) 指定的项的处理程序。
        /// </summary>
        /// <param name="pbc">
        /// 指向绑定上下文对象上的 IBindCtx 接口的指针。 用于将可选参数传递给处理程序。 绑定上下文的内容特定于处理程序。
        /// 例如，绑定到 BHID_Stream时，绑定上下文中的 STGM 标志指示 (读取或读/写) 所需的访问模式。
        /// </param>
        /// <param name="bhid">对 GUID 的引用，指定将创建哪个处理程序。</param>
        /// <param name="riid">要检索的对象类型的 IID。</param>
        /// <param name="ppv">此方法返回时，包含由 riid 指定的处理程序返回的类型 riid 的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int BindToHandler(IntPtr pbc, Guid bhid, Guid riid, out IntPtr ppv);

        /// <summary>
        /// 获取 IShellItem 对象的父对象。
        /// </summary>
        /// <param name="ppsi">指向 IShellItem 接口父级的指针的地址。</param>
        /// <returns>如果成功，则返回 S_OK，否则返回错误值。</returns>
        [PreserveSig]
        int GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// 获取 IShellItem 对象的显示名称。
        /// </summary>
        /// <param name="sigdnName">指示名称外观的 SIGDN 值之一。</param>
        /// <param name="ppszName">一个值，当此函数成功返回时，接收指向检索的显示名称的指针的地址。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        /// <summary>
        /// 获取 IShellItem 对象的请求属性集。
        /// </summary>
        /// <param name="sfgaoMask">指定要检索的属性。 一个或多个 SFGAO 值。 使用按位 OR 运算符确定要检索的属性。</param>
        /// <returns>如果返回的属性与 sfgaoMask 中请求的属性完全匹配，则返回S_OK。如果属性不完全匹配，或者返回标准 COM 错误值，则返回S_FALSE。</returns>
        [PreserveSig]
        int GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

        /// <summary>
        /// 比较两个 IShellItem 对象。
        /// </summary>
        /// <param name="psi">指向要与现有 IShellItem 对象进行比较的 IShellItem 对象的指针。</param>
        /// <param name="hint">确定如何执行比较的 SICHINTF 值之一。有关此参数的可能值列表，请参阅 SICHINTF。</param>
        /// <param name="piOrder">此参数接收比较结果。如果两个项目相同，则此参数等于零;如果它们不同，则参数为非零。</param>
        /// <returns>如果项相同，则返回S_OK项，如果项不同，则返回S_FALSE，否则返回错误值。</returns>
        [PreserveSig]
        int Compare([MarshalAs(UnmanagedType.Interface)] IShellItem psi, uint hint, out int piOrder);
    }
}
