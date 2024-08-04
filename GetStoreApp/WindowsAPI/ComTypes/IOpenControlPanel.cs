using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开检索控制面板的视图状态、单个控制面板项的路径以及打开控制面板本身或单个控制面板项的方法。
    /// </summary>
    [GeneratedComInterface, Guid("D11AD862-66DE-4DF4-BF6C-1F5621996AF1")]
    public partial interface IOpenControlPanel
    {
        /// <summary>
        /// 打开指定的控制面板项（可选）到特定页面。
        /// </summary>
        /// <param name="pszName">以 Unicode 字符串的形式指向项的规范名称的指针。 此参数是可选的，可以为 NULL。 如果调用应用程序传递 NULL，则控制面板本身将打开。 有关控制面板项规范名称的完整列表，请参阅控制面板项的规范名称。</param>
        /// <param name="page">指向要显示的项内页面名称的指针。 此字符串追加到 shell 文件夹控制面板项的路径末尾，或作为控制面板 (.cpl) 文件项的命令行参数追加。 此参数可以为 NULL，在这种情况下，将显示第一页。</param>
        /// <param name="punkSite">指向站点的指针，用于在框架中导航 Shell 文件夹控制面板项。 此参数可以为 NULL。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Open([MarshalAs(UnmanagedType.LPWStr)] string pszName, [MarshalAs(UnmanagedType.LPWStr)] string page, IntPtr punkSite);

        /// <summary>
        /// 获取指定控制面板项的路径。
        /// </summary>
        /// <param name="pszName">指向项的规范名称或其 GUID 的指针。 此值可以为 NULL。 有关更多详细信息，请参阅“备注”。 有关控制面板项规范名称的完整列表，请参阅控制面板项的规范名称。</param>
        /// <param name="pszPath">此方法返回时，将指定控制面板项的路径作为 Unicode 字符串包含在内。</param>
        /// <param name="cchPath">pszPath 指向的缓冲区的大小（以 WCHAR 表示）。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPath([MarshalAs(UnmanagedType.LPWStr)] string pszName, out IntPtr pszPath, uint cchPath);

        /// <summary>
        /// 获取最新的控制面板视图：经典视图或类别视图。
        /// </summary>
        /// <param name="pView">接收最新视图的指针。 </param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetCurrentView(out IntPtr pView);
    }
}
