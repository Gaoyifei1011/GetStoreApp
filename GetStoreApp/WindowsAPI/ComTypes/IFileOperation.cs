using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开用于复制、移动、重命名、创建和删除 Shell 项的方法，以及提供进度和错误对话框的方法。
    /// </summary>
    [GeneratedComInterface, Guid("947AAB5F-0A5C-4C13-B4D6-4BF7836FC9F8")]
    public partial interface IFileOperation
    {
        /// <summary>
        /// 使处理程序能够为所有操作提供状态和错误信息。
        /// </summary>
        /// <param name="pfops">指向要用于进度状态和错误通知的 IFileOperationProgressSink 对象的指针。</param>
        /// <param name="pdwCookie">此方法返回时，此参数指向唯一标识此连接的返回标记。 调用应用程序稍后使用此令牌通过将其传递给 IFileOperation：：Unadvise 来删除连接。 如果调用 建议 失败，此值毫无意义。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Advise(nint pfops, out uint pdwCookie);

        /// <summary>
        /// 终止以前通过 IFileOperation：：Advise 建立的咨询连接。
        /// </summary>
        /// <param name="dwCookie">标识要删除的连接的连接令牌。 此值最初是在建立连接时由 建议 检索的。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Unadvise(uint dwCookie);

        /// <summary>
        /// 设置当前操作的参数。
        /// </summary>
        /// <param name="dwOperationFlags">控制文件操作的标志。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetOperationFlags(FileOperationFlags dwOperationFlags);

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="pszMessage">指向窗口标题的指针。 这是以 null 结尾的 Unicode 字符串。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetProgressMessage([MarshalAs(UnmanagedType.LPWStr)] string pszMessage);

        /// <summary>
        /// 指定用于显示操作进度的对话框。
        /// </summary>
        /// <param name="popd">指向表示对话框的 IOperationsProgressDialog 对象的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetProgressDialog(nint popd);

        /// <summary>
        /// 声明一组要对一个或多个项设置的属性和值。
        /// </summary>
        /// <param name="pproparray">指向 IPropertyChangeArray 的指针，该指针访问 IPropertyChange 对象的集合，这些对象指定要设置的属性及其新值。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetProperties(nint pproparray);

        /// <summary>
        /// 设置进度窗口和对话框窗口的父窗口或所有者窗口。
        /// </summary>
        /// <param name="hwndOwner">操作的所有者窗口的句柄。 此窗口将收到错误消息。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetOwnerWindow(nint hwndOwner);

        /// <summary>
        /// 声明要设置其属性值的单个项。
        /// </summary>
        /// <param name="psiItem">指向要接收新属性值的项的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int ApplyPropertiesToItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem);

        /// <summary>
        /// 声明一组要为其应用一组通用属性值的项。
        /// </summary>
        /// <param name="punkItems">指向表示项组的 IShellItemArray、IDataObject 或 IEnumShellItems 对象的 IUnknown 的指针。 还可以指向 IPersistIDList 对象来表示单个项，从而有效地完成与 IFileOperation：：ApplyPropertiesToItem 相同的函数。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int ApplyPropertiesToItems(nint punkItems);

        /// <summary>
        /// 声明一个项，该项将被赋予新的显示名称。
        /// </summary>
        /// <param name="psiItem">指向指定源项的 IShellItem 的指针。</param>
        /// <param name="pszNewName">指向项的新 显示名称 的指针。 这是以 null 结尾的 Unicode 字符串。</param>
        /// <param name="pfopsItem">指向要用于状态和失败通知的 IFileOperationProgressSink 对象的指针。 如果为整体操作调用 IFileOperation：：Advise ，则会包含重命名操作的进度状态和错误通知，因此请将此参数设置为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int RenameItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, nint pfopsItem);

        /// <summary>
        /// 声明一组要赋予新显示名称的项。 所有项的名称都相同。
        /// </summary>
        /// <param name="pUnkItems">指向 IShellItemArray、IDataObject 或 IEnumShellItems 对象的 IUnknown 的指针，该对象表示要重命名的项组。 还可以指向 IPersistIDList 对象来表示单个项，从而有效地完成与 IFileOperation：：RenameItem 相同的功能。</param>
        /// <param name="pszNewName">指向项的新显示名称的指针。 这是以 null 结尾的 Unicode 字符串。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int RenameItems(nint pUnkItems, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

        /// <summary>
        /// 声明要移动到指定目标的单个项。
        /// </summary>
        /// <param name="psiItem">指向指定源项的 IShellItem 的指针。</param>
        /// <param name="psiDestinationFolder">指向 IShellItem 的指针，该 IShellItem 指定要包含移动项的目标文件夹。</param>
        /// <param name="pszNewName">指向项目在新位置的新名称的指针。 这是以 null 结尾的 Unicode 字符串，可以为 NULL。 如果 为 NULL，则目标项的名称与源项的名称相同。</param>
        /// <param name="pfopsItem">指向 IFileOperationProgressSink 对象的指针，该对象用于此特定移动操作的进度状态和错误通知。 如果为整体操作调用 IFileOperation：：Advise ，则会包含移动操作的进度状态和错误通知，因此请将此参数设置为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int MoveItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem, [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, nint pfopsItem);

        /// <summary>
        /// 声明一组要移动到指定目标的项。
        /// </summary>
        /// <param name="punkItems">指向 IShellItemArray、IDataObject 或 IEnumShellItems 对象的 IUnknown 的指针，该对象表示要移动的项组。 还可以指向 IPersistIDList 对象来表示单个项，从而有效地完成与 IFileOperation：：MoveItem 相同的函数。</param>
        /// <param name="psiDestinationFolder">指向 IShellItem 的指针，该 IShellItem 指定要包含移动项目的目标文件夹。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int MoveItems(nint punkItems, [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder);

        /// <summary>
        /// 声明要复制到指定目标的单个项。
        /// </summary>
        /// <param name="psiItem">指向指定源项的 IShellItem 的指针。</param>
        /// <param name="psiDestinationFolder">指向 IShellItem 的指针，该 IShellItem 指定要包含项目副本的目标文件夹。</param>
        /// <param name="pszCopyName">指向复制后项的新名称的指针。 这是以 null 结尾的 Unicode 字符串，可以为 NULL。 如果 为 NULL，则目标项的名称与源相同。</param>
        /// <param name="pfopsItem">指向 IFileOperationProgressSink 对象的指针，该对象用于此特定复制操作的进度状态和错误通知。 如果为整个操作调用 IFileOperation：：Advise ，则会包含复制操作的进度状态和错误通知，因此请将此参数设置为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int CopyItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem, [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, nint pfopsItem);

        /// <summary>
        /// 声明一组要复制到指定目标的项。
        /// </summary>
        /// <param name="punkItems">指向表示要复制的项组的 IShellItemArray、IDataObject 或 IEnumShellItems 对象的 IUnknown 的指针。 还可以指向 IPersistIDList 对象来表示单个项，从而有效地完成与 IFileOperation：：CopyItem 相同的函数。</param>
        /// <param name="psiDestinationFolder">指向 IShellItem 的指针，该 IShellItem 指定要包含项目副本的目标文件夹。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int CopyItems(nint punkItems, [MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder);

        /// <summary>
        /// 声明要删除的单个项。
        /// </summary>
        /// <param name="psiItem">指向指定要删除的项的 IShellItem 的指针。</param>
        /// <param name="pfopsItem">指向 IFileOperationProgressSink 对象的指针，该对象用于此特定删除操作的进度状态和错误通知。 如果为整体操作调用 IFileOperation：：Advise ，则会包含删除操作的进度状态和错误通知，因此请将此参数设置为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int DeleteItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiItem, nint pfopsItem);

        /// <summary>
        /// 声明一组要删除的项。
        /// </summary>
        /// <param name="punkItems">指向 IShellItemArray、IDataObject 或 IEnumShellItems 对象的 IUnknown 的指针，该对象表示要删除的项组。 还可以指向 IPersistIDList 对象来表示单个项，从而有效地完成与 IFileOperation：:D eleteItem 相同的功能。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int DeleteItems(nint punkItems);

        /// <summary>
        /// 指向 IShellItem 的指针，该 IShellItem 指定将包含新项的目标文件夹。
        /// </summary>
        /// <param name="psiDestinationFolder">一个按位值，指定文件或文件夹的文件系统属性。 有关可能的值 ，请参阅 GetFileAttributes 。</param>
        /// <param name="dwFileAttributes"></param>
        /// <param name="pszName">指向新项的文件名的指针，例如 Newfile.txt。 这是以 null 结尾的 Unicode 字符串。</param>
        /// <param name="pszTemplateName">
        /// 指向模板文件名称的指针，存储在以下位置之一：
        /// CSIDL_COMMON_TEMPLATES。 此文件夹的默认路径为 %ALLUSERSPROFILE%\Templates。
        /// CSIDL_TEMPLATES。 此文件夹的默认路径为 %USERPROFILE%\Templates。
        /// %SystemRoot%\shellnew
        /// 这是一个以 null 结尾的 Unicode 字符串，用于指定与新文件具有相同类型的现有文件，其中包含应用程序希望包含在任何新文件中的最少内容。
        /// 此参数通常为 NULL ，用于指定新的空白文件。
        /// </param>
        /// <param name="pfopsItem">指向要用于状态和失败通知的 IFileOperationProgressSink 对象的指针。 如果为整个操作调用 IFileOperation：：Advise ，则会包含创建操作的进度状态和错误通知，因此请将此参数设置为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int NewItem([MarshalAs(UnmanagedType.Interface)] IShellItem psiDestinationFolder, FileAttributes dwFileAttributes, [MarshalAs(UnmanagedType.LPWStr)] string pszName, [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName, nint pfopsItem);

        /// <summary>
        /// 执行所有选定的操作。
        /// </summary>
        /// <returns>如果成功，则返回 S_OK，否则返回错误值。 请注意，如果用户取消了操作，此方法仍可返回成功代码。 使用 GetAnyOperationsAborted 方法确定是否是这种情况。</returns>
        [PreserveSig]
        int PerformOperations();

        /// <summary>
        /// 获取一个值，该值指示调用 IFileOperation：:P erformOperations 启动的任何文件操作在完成之前是否已停止。 可以通过用户操作或系统以无提示方式停止操作。
        /// </summary>
        /// <param name="pfAnyOperationsAborted">此方法返回时，如果任何文件操作在完成之前中止，则指向 TRUE ;否则为 FALSE。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetAnyOperationsAborted([MarshalAs(UnmanagedType.Bool)] out bool pfAnyOperationsAborted);
    }
}
