using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 使用 IBackgroundCopyError 接口，用于确定错误原因以及传输过程是否可以继续。
    /// 仅当作业的状态为BG_JOB_STATE_ERROR或BG_JOB_STATE_TRANSIENT_ERROR时，BITS 才会创建错误对象。 当 IBackgroundCopyXXXX 接口方法失败时，BITS 不会创建错误对象。 在 BITS 开始传输数据(作业的状态更改为作业BG_JOB_STATE_TRANSFERRING) 或应用程序退出之前，错误对象才可用。
    /// 若要获取 IBackgroundCopyError 对象，请调用 IBackgroundCopyJob：：GetError 方法。
    /// </summary>
    [GeneratedComInterface, Guid("19C613A0-FCB8-4F28-81AE-897C3D078F81")]
    public partial interface IBackgroundCopyError
    {
        /// <summary>
        /// 检索错误代码并确定发生错误的上下文。
        /// </summary>
        /// <param name="pContext">发生错误的上下文。 有关上下文值的列表，请参阅 BG_ERROR_CONTEXT 枚举。</param>
        /// <param name="pCode">发生的错误的错误代码。</param>
        /// <returns>此方法在成功时返回 S_OK 或错误时返回标准 COM HRESULT 值之一。</returns>
        [PreserveSig]
        int GetError(out BG_ERROR_CONTEXT pContext, out int pCode);

        /// <summary>
        /// 检索指向与错误关联的文件对象的接口指针。
        /// </summary>
        /// <param name="pVal">IBackgroundCopyFile 接口指针，用于确定与错误关联的本地和远程文件名的方法。 如果错误未与本地或远程文件关联， 则 ppFile 参数设置为 NULL 。 完成后，释放 ppFile。</param>
        /// <returns>此方法返回以下 HRESULT 值。</returns>
        [PreserveSig]
        int GetFile(out IntPtr pVal);

        /// <summary>
        /// 检索与错误关联的错误文本。
        /// </summary>
        /// <param name="LanguageId">标识用于生成说明的区域设置。</param>
        /// <param name="pErrorDescription">包含与错误关联的错误文本的以 Null 结尾的字符串。 完成后，调用 CoTaskMemFree 函数以释放 ppErrorDescription 。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetErrorDescription(uint LanguageId, [MarshalAs(UnmanagedType.LPWStr)] out string pErrorDescription);

        /// <summary>
        /// 检索发生错误的上下文的说明。
        /// </summary>
        /// <param name="LanguageId">标识用于生成说明的区域设置。</param>
        /// <param name="pContextDescription">包含与错误关联的错误文本的以 Null 结尾的字符串。 完成后，调用 CoTaskMemFree 函数以释放 ppErrorDescription 。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetErrorContextDescription(uint LanguageId, [MarshalAs(UnmanagedType.LPWStr)] out string pContextDescription);

        /// <summary>
        /// 检索用于传输文件的协议。 远程文件名标识用于传输文件的协议。
        /// </summary>
        /// <param name="pProtocol">以 Null 结尾的字符串，其中包含用于传输文件的协议。 字符串包含 HTTP 协议的“http”和 SMB 协议的“file”。 如果错误与传输协议无关，则 ppProtocol 参数设置为 NULL 。 完成后，调用 CoTaskMemFree 函数以释放 ppProtocol 。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetProtocol([MarshalAs(UnmanagedType.LPWStr)] out string pProtocol);
    }
}
