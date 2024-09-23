using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppShellExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开获取命令外观、枚举子命令或调用命令的方法。
    /// </summary>
    [GeneratedComInterface, Guid("A08CE4D0-FA25-44AB-B57C-C7B1C323E0B9")]
    public partial interface IExplorerCommand
    {
        /// <summary>
        /// 获取启动指定 Windows 资源管理器命令项的按钮或菜单项的标题文本。
        /// </summary>
        /// <param name="psiItemArray">指向 IShellItemArray 的指针。</param>
        /// <param name="ppszName">指向缓冲区的指针，当此方法成功返回时，该缓冲区接收标题字符串。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetTitle([MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        /// <summary>
        /// 获取与指定的 Windows 资源管理器命令项关联的图标的图标资源字符串。
        /// </summary>
        /// <param name="psiItemArray">指向 IShellItemArray 的指针。</param>
        /// <param name="ppszIcon">指向缓冲区的指针，此方法成功返回时，该缓冲区接收标识图标源的资源字符串。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetIcon([MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszIcon);

        /// <summary>
        /// 获取与指定的 Windows 资源管理器命令项关联的工具提示字符串。
        /// </summary>
        /// <param name="psiItemArray">指向 IShellItemArray 的指针。</param>
        /// <param name="ppszInfotip">指向缓冲区的指针，当此方法成功返回时，该缓冲区接收工具提示字符串。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetToolTip([MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszInfotip);

        /// <summary>
        /// 获取 Windows 资源管理器命令的 GUID。
        /// </summary>
        /// <param name="pguidCommandName">指向一个值的指针，当此方法成功返回时，将接收命令的 GUID，该 GUID 在注册表中声明它。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetCanonicalName(out Guid pguidCommandName);

        /// <summary>
        /// 获取与指定的 Windows 资源管理器命令项关联的状态信息。
        /// </summary>
        /// <param name="psiItemArray">指向 IShellItemArray 的指针。</param>
        /// <param name="fOkToBeSlow">如果 谓词对象不应执行任何可能导致 UI 线程停止响应的内存密集型计算，则为 FALSE。 在这种情况下，谓词对象应返回E_PENDING。 如果 为 TRUE，则可以完成这些计算。</param>
        /// <param name="pCmdState">指向一个值的指针，当此方法成功返回时，该值接收 EXPCMDSTATE 常量指示的一个或多个 Windows 资源管理器命令状态。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetState([MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow, out EXPCMDSTATE pCmdState);

        /// <summary>
        /// 调用 Windows 资源管理器命令。
        /// </summary>
        /// <param name="psiItemArray">指向 IShellItemArray 的指针。</param>
        /// <param name="pbc">指向 IBindCtx 接口的指针，该接口提供对绑定上下文的访问。 如果不需要绑定上下文，此值可以为 NULL 。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Invoke([MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray, IntPtr pbc);

        /// <summary>
        /// 获取与 Windows 资源管理器命令关联的标志。
        /// </summary>
        /// <param name="pFlags">此方法返回时，此值指向当前命令标志。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetFlags(out EXPCMDFLAGS pFlags);

        /// <summary>
        /// 检索命令的子命令的枚举器。
        /// </summary>
        /// <param name="ppEnum">此方法成功返回时，包含可用于遍历子命令集的 IEnumExplorerCommand 接口指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int EnumSubCommands([MarshalAs(UnmanagedType.Interface)] out IEnumExplorerCommand ppEnum);
    }
}
