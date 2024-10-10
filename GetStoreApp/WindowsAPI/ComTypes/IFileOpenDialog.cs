using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 通过添加特定于打开对话框的方法扩展 IFileOpenDialog 接口。
    /// </summary>
    [GeneratedComInterface, Guid("42F85136-DB7E-439C-85F1-E4075D135FC8")]
    public partial interface IFileOpenDialog
    {
        /// <summary>
        /// 启动模式窗口。
        /// </summary>
        /// <param name="hwndOwner">所有者窗口的句柄。 此值可以为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>

        [PreserveSig]
        int Show(IntPtr hwndOwner);

        /// <summary>
        /// 设置对话框可以打开或保存的文件类型。
        /// </summary>
        /// <param name="cFileTypes">rgFilterSpec 指定的数组中的元素数。</param>
        /// <param name="rgFilterSpec">指向 COMDLG_FILTERSPEC 结构数组的指针，每个结构表示一个文件类型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        /// </summary>
        [PreserveSig]
        int SetFileTypes(uint cFileTypes, IntPtr rgFilterSpec);

        /// <summary>
        /// 设置在对话框中显示为所选的文件类型。
        /// </summary>
        /// <param name="iFileType">传递给其 cFileTypes 参数中的 IFileDialog::SetFileTypes 的文件类型数组中的文件类型的索引。 请注意，这是一个从 1 开始的索引，而不是从零开始的索引。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFileTypeIndex(uint iFileType);

        /// <summary>
        /// 获取当前选定的文件类型。
        /// </summary>
        /// <param name="piFileType">指向 UINT 值的指针，该值接收传递给其 cFileTypes 参数中 IFileDialog::SetFileTypes 的 文件类型数组中所选 文件类型的索引。注意 这是一个从 1 开始的索引，而不是从零开始的索引。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</return>
        [PreserveSig]
        int GetFileTypeIndex(out uint piFileType);

        /// <summary>
        /// 分配侦听来自对话的事件的事件处理程序。
        /// </summary>
        /// <param name="pfde">指向将从对话接收事件的 IFileDialogEvents 实现的指针。</param>
        /// <param name="pdwCookie">指向 DWORD 的指针，该 DWORD 接收标识此事件处理程序的值。 当客户端完成对话时，该客户端必须使用此值调用 IFileDialog::Unadvise 方法。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Advise(IntPtr pfde, out uint pdwCookie);

        /// <summary>
        /// 删除通过 Advise 方法附加的事件处理程序。
        /// </summary>
        /// <param name="dwCookie">表示事件处理程序的 DWORD 值。 此值是通过 IFileDialog::Advise 方法的 pdwCookie 参数获取的。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Unadvise(uint dwCookie);

        /// <summary>
        /// 设置标志以控制对话的行为。
        /// </summary>
        /// <param name="fos">FILEOPENDIALOGOPTIONS 值的一个或多个。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetOptions(FILEOPENDIALOGOPTIONS fos);

        /// <summary>
        /// 获取设置为控制对话框行为的当前标志。
        /// </summary>
        /// <param name="pfos">此方法成功返回时，指向由一个或多个 FILEOPENDIALOGOPTIONS 值组成的值。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetOptions(out FILEOPENDIALOGOPTIONS pfos);

        /// <summary>
        /// 如果最近使用的文件夹值不可用，则设置用作默认值的文件夹。
        /// </summary>
        /// <param name="psi">指向表示文件夹的接口的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetDefaultFolder([MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        /// <summary>
        /// 设置在打开对话框时始终选择的文件夹，而不考虑以前的用户操作。
        /// </summary>
        /// <param name="psi">指向表示文件夹的接口的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFolder([MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        /// <summary>
        /// 获取当前在对话框中选择的文件夹，或者，如果当前未显示对话框，则打开对话框时要选择的文件夹。
        /// </summary>
        /// <param name="ppsi">指向表示文件夹的接口的指针的地址。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// 获取对话框中用户的当前选择。
        /// </summary>
        /// <param name="ppsi">指向接口的指针的地址，该接口表示当前在对话框中选择的项。 此项可以是在视图窗口中选择的文件或文件夹，也可以是用户在对话框的编辑框中输入的内容。 后一种情况可能需要分析操作， (阻止当前线程的用户) 取消。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// 设置打开该对话框时显示在“ 文件名 编辑”框中的文件名。
        /// </summary>
        /// <param name="pszName">指向文件名称的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>
        /// 检索对话框的 “文件名 编辑”框中当前输入的文本。
        /// </summary>
        /// <param name="pszName">指向缓冲区的指针的地址，此方法成功返回时接收文本。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        /// <summary>
        /// 设置对话框的标题。
        /// </summary>
        /// <param name="pszTitle">指向包含标题文本的缓冲区的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        /// <summary>
        /// 设置 “打开 ”或 “保存 ”按钮的文本。
        /// </summary>
        /// <param name="pszText">指向包含按钮文本的缓冲区的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);

        /// <summary>
        /// 设置文件名编辑框旁边的标签文本。
        /// </summary>
        /// <param name="pszLabel">指向包含标签文本的缓冲区的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        /// <summary>
        /// 获取用户在对话框中所做的选择。
        /// </summary>
        /// <param name="ppsi">指向表示用户选择的 IShellItem 的指针的地址。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// 将文件夹添加到可供用户打开或保存项的位置列表中。
        /// </summary>
        /// <param name="psi">指向 IShellItem 的指针，该 IShellItem 表示要提供给用户的文件夹。 这只能是文件夹。</param>
        /// <param name="fdcp">指定文件夹在列表中的放置位置。 请参阅 FDAP。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int AddPlace([MarshalAs(UnmanagedType.Interface)] IShellItem psi, int fdcp);

        /// <summary>
        /// 设置要添加到文件名的默认扩展名。
        /// </summary>
        /// <param name="pszDefaultExtension">指向包含扩展文本的缓冲区的指针。 此字符串不应包含前导句点。 例如，“jpg”是正确的，而“.jpg”不正确。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        /// <summary>
        /// 关闭对话框。
        /// </summary>
        /// <param name="hr">Show 将返回的代码，指示对话框在进行选择之前已关闭。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Close(int hr);

        /// <summary>
        /// 使调用应用程序能够将 GUID 与对话框的持久状态相关联。
        /// </summary>
        /// <param name="guid">要与此对话状态关联的 GUID。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetClientGuid(in Guid guid);

        /// <summary>
        /// 指示对话框清除所有持久状态信息。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int ClearClientData();

        /// <summary>
        /// 设置筛选器。
        /// 已弃用。 SetFilter 不再可用于 Windows 7。
        /// </summary>
        /// <param name="pFilter">指向要设置的 IShellItemFilter 的 指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFilter(IntPtr pFilter);

        /// <summary>
        /// 在允许多项选择的对话框中获取用户的选择。
        /// </summary>
        /// <param name="ppenum">指向 IShellItemArray 的指针的地址，可通过该地址访问对话框中选择的项。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetResults(out IntPtr ppenum);

        /// <summary>
        /// 获取对话框中当前选定的项。这些项目可能是在视图中选择的项目，也可能是在文件名编辑框中选择的文本。
        /// </summary>
        /// <param name="ppsai">指向 IShellItemArray 的指针的地址，可通过该地址访问所选项。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetSelectedItems(out IntPtr ppsai);
    }
}
