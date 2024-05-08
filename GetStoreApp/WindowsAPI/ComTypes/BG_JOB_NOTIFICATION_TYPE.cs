using System;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 标识要接收的事件。
    /// </summary>
    [Flags]
    public enum BG_JOB_NOTIFICATION_TYPE : uint
    {
        /// <summary>
        /// 作业中的所有文件都已传输。
        /// </summary>
        BG_NOTIFY_JOB_TRANSFERRED = 0x0001,

        /// <summary>
        /// 出现错误。
        /// </summary>
        BG_NOTIFY_JOB_ERROR = 0x0002,

        /// <summary>
        /// 已禁用事件通知。 BITS 忽略其他标志。
        /// </summary>
        BG_NOTIFY_DISABLE = 0x0004,

        /// <summary>
        /// 作业已修改。 例如，属性值已更改、作业状态已更改或文件传输进度。 如果指定了命令行 通知 ，则命令行回调中将忽略此标志。
        /// </summary>
        BG_NOTIFY_JOB_MODIFICATION = 0x0008,

        /// <summary>
        /// 作业中的文件已传输。 如果指定了命令行 通知 ，则命令行回调中将忽略此标志。
        /// </summary>
        BG_NOTIFY_FILE_TRANSFERRED = 0x0010,

        /// <summary>
        /// 已传输文件中的字节范围。 如果指定了命令行 通知 ，则命令行回调中将忽略此标志。 可以为任何作业指定标志，但你只会收到满足 BITS_JOB_PROPERTY_ON_DEMAND_MODE 作业要求的作业的通知。
        /// </summary>
        BG_NOTIFY_FILE_RANGES_TRANSFERRED = 0x0020
    }
}
