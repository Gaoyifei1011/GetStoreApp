using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 创建传输作业，检索包含队列中作业的枚举器对象，并从队列中检索单个作业。
    /// </summary>
    [GeneratedComInterface, Guid("5CE34C0D-0DC9-4C1F-897C-DAA1B78CEE7C")]
    public partial interface IBackgroundCopyManager
    {
        /// <summary>
        /// 创建作业。
        /// </summary>
        /// <param name="DisplayName">以 Null 结尾的字符串，其中包含作业的显示名称。 通常，显示名称用于在用户界面中标识作业。 请注意，多个作业可能具有相同的显示名称。 不得为 NULL。 名称限制为 256 个字符，不包括 null 终止符。</param>
        /// <param name="Type">传输作业的类型，例如BG_JOB_TYPE_DOWNLOAD。 有关传输类型的列表，请参阅 BG_JOB_TYPE 枚举。</param>
        /// <param name="pJobId">唯一标识队列中的作业。 调用 IBackgroundCopyManager：：GetJob 方法从队列中获取作业时使用此标识符。</param>
        /// <param name="ppJob">IBackgroundCopyJob 接口指针，用于修改作业的属性并指定要传输的文件。 若要激活队列中的作业，请调用 IBackgroundCopyJob：：Resume 方法。 完成后释放 ppJob 。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int CreateJob([MarshalAs(UnmanagedType.LPWStr)] string DisplayName, BG_JOB_TYPE Type, out Guid pJobId, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob ppJob);

        /// <summary>
        /// 从传输队列中检索指定的作业。 通常，应用程序会保留作业标识符，以便稍后可以从队列中检索作业。
        /// </summary>
        /// <param name="jobID">标识要从传输队列中检索的作业。 CreateJob 方法返回作业标识符。</param>
        /// <param name="ppJob">指向 JobID 指定的作业的 IBackgroundCopyJob 接口指针。 完成后，释放 ppJob。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int GetJob(in Guid jobID, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob ppJob);

        /// <summary>
        /// 检索指向枚举器对象的接口指针，该对象用于枚举传输队列中的 作业 。 枚举器中作业的顺序是任意的。
        /// </summary>
        /// <param name="dwFlags">指定要在枚举中包含其作业。</param>
        /// <param name="ppenum">用于枚举传输队列中的作业的 IEnumBackgroundCopyJobs 接口指针。 枚举器的内容取决于 dwFlags 的值。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int EnumJobs(uint dwFlags, out IntPtr ppenum);

        /// <summary>
        /// 检索指定错误代码的说明。
        /// </summary>
        /// <param name="hResult">以前调用 BITS 方法时出现错误代码。</param>
        /// <param name="LanguageId">标识用于生成说明的语言标识符。 若要创建语言标识符，请使用 MAKELANGID 宏。</param>
        /// <param name="pErrorDescription">包含错误说明的以 Null 结尾的字符串。 完成后，调用 CoTaskMemFree 函数以释放 ppErrorDescription 。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int GetErrorDescription([MarshalAs(UnmanagedType.Error)] int hResult, uint LanguageId, [MarshalAs(UnmanagedType.LPWStr)] out string pErrorDescription);
    }
}
