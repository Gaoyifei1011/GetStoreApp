using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 实现 IBackgroundCopyCallback 接口以接收作业已完成、已修改或出错的 通知 。 客户端使用此接口，而不是轮询作业的状态。
    /// </summary>
    [GeneratedComInterface, Guid("97EA99C7-0186-4AD4-8DF9-C5B4E0ED6B22")]
    public partial interface IBackgroundCopyCallback
    {
        /// <summary>
        /// 当作业中的所有文件都已成功传输时，BITS 会调用 JobTransferred 方法的实现。 对于BG_JOB_TYPE_UPLOAD_REPLY作业，BITS 在上传文件传输到服务器并将回复传输到客户端后调用 JobTransferred 方法。
        /// </summary>
        /// <param name="pJob">包含与作业相关的信息，例如作业完成的时间、传输的字节数和传输的文件数。 不释放 pJob;当 方法返回时，BITS 释放接口。</param>
        /// <returns>
        /// 此方法应返回 S_OK;否则，BITS 将继续调用此方法，直到返回 S_OK 。 出于性能原因，应将返回除 S_OK 以外的值的次数限制为多次。 作为返回错误代码的替代方法，请考虑始终返回 S_OK 并在内部处理错误。 调用此方法的间隔是任意的。
        /// 请注意，如果此方法失败，并且你调用 了 IBackgroundCopyJob2：：SetNotifyCmdLine 方法，则执行命令行，并且不会再次调用此方法。
        /// </returns>
        [PreserveSig]
        int JobTransferred([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob);

        /// <summary>
        /// 当作业的状态更改为BG_JOB_STATE_ERROR时，BITS 调用 JobError 方法的实现。
        /// </summary>
        /// <param name="pJob">包含与作业相关的信息，例如在发生错误之前传输的字节数和文件数。 它还包含恢复和取消作业的方法。 不释放 pJob; 当 JobError 方法返回时，BITS 释放接口。</param>
        /// <param name="pError">包含错误信息，例如发生严重错误时正在处理的文件和错误说明。 不释放 pError; 当 JobError 方法返回时，BITS 释放接口。</param>
        /// <returns>
        /// 此方法应返回 S_OK;否则，BITS 会继续调用此方法，直到返回 S_OK 。 出于性能原因，应将返回 非S_OK 值的次数限制为几次。 作为返回错误代码的替代方法，请考虑始终返回 S_OK 并在内部处理错误。 调用此方法的间隔是任意的。
        /// 请注意，如果此方法失败，并且你调用 了 IBackgroundCopyJob2：：SetNotifyCmdLine 方法，则会执行命令行，并且不会再次调用此方法。
        /// </returns>
        [PreserveSig]
        int JobError([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob, [MarshalAs(UnmanagedType.Interface)] IBackgroundCopyError pError);

        /// <summary>
        /// 修改作业后，BITS 会调用 JobModification 方法的实现。 当传输字节、文件已添加到作业、修改属性或作业状态更改时，服务将生成此事件。
        /// </summary>
        /// <param name="pJob">包含用于访问作业的属性、进度和状态信息的方法。 不释放 pJob; 当 JobModification 方法返回时，BITS 释放接口。</param>
        /// <param name="dwReserved">保留供将来使用。</param>
        /// <returns>此方法应返回 S_OK。</returns>
        [PreserveSig]
        int JobModification([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob, uint dwReserved);
    }
}
