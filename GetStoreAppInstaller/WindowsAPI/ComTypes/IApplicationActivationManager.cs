using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 为当前会话中的 Windows.Launch) (通用启动协定激活指定的 Windows 应用商店应用。
    /// </summary>
    [GeneratedComInterface, Guid("2E941141-7F97-4756-BA1D-9DECDE894A3D")]
    public partial interface IApplicationActivationManager
    {
        /// <summary>
        /// 为当前会话中的 Windows.Launch) (通用启动协定激活指定的 Windows 应用商店应用。
        /// </summary>
        /// <param name="appUserModelId">Windows 应用商店应用的应用程序用户模型 ID。</param>
        /// <param name="arguments">指向特定于应用的可选参数字符串的指针。</param>
        /// <param name="options">以下一个或多个标志用于支持设计模式、调试和测试方案。</param>
        /// <param name="processId">指向一个值的指针，当此方法成功返回时，该值接收满足此协定的应用实例的进程 ID。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int ActivateApplication([MarshalAs(UnmanagedType.LPWStr)] string appUserModelId, [MarshalAs(UnmanagedType.LPWStr)] string arguments, ACTIVATEOPTIONS options, out uint processId);

        /// <summary>
        /// 为文件协定激活指定的 Windows 应用商店应用 (Windows.File) 。
        /// </summary>
        /// <param name="appUserModelId">Windows 应用商店应用的应用程序用户模型 ID。</param>
        /// <param name="itemArray">指向 Shell 项数组的指针，每个项表示一个文件。 此值转换为通过 FileActivatedEventArgs 传递给应用的 StorageItem 对象的 VectorView。</param>
        /// <param name="verb">应用于 itemArray 指定的一个或多个文件的谓词。</param>
        /// <param name="processId">指向一个值的指针，该值在此方法成功返回时接收满足此协定的应用实例的进程 ID。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int ActivateForFile([MarshalAs(UnmanagedType.LPWStr)] string appUserModelId, IntPtr itemArray, [MarshalAs(UnmanagedType.LPWStr)] string verb, out uint processId);

        /// <summary>
        /// 为 Windows.Protocol 协议协定激活指定的 Windows 应用商店应用。
        /// </summary>
        /// <param name="appUserModelId">Windows 应用商店应用的应用程序用户模型 ID。</param>
        /// <param name="itemArray">指向单个 Shell 项数组的指针。 数组中的第一项将转换为 Uri 对象，该对象通过 ProtocolActivatedEventArgs 传递给应用。 数组中除第一个元素之外的所有项都被忽略。</param>
        /// <param name="processId">指向一个值的指针，该值在此方法成功返回时接收满足此协定的应用实例的进程 ID。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int ActivateForProtocol([MarshalAs(UnmanagedType.LPWStr)] string appUserModelId, IntPtr itemArray, out uint processId);
    }
}
