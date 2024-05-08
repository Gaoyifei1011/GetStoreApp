using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供与作业相关的进度信息，例如传输的字节数和文件数。 对于上传作业，进度适用于上传文件，而不是回复文件。 若要查看回复文件进度，请参阅 BG_JOB_REPLY_PROGRESS 结构。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct BG_JOB_PROGRESS
    {
        /// <summary>
        /// 作业中所有文件要传输的总字节数。 如果值为 BG_SIZE_UNKNOWN，则尚未确定作业中所有文件的总大小。 如果 BITS 无法确定其中一个文件的大小，则不会设置此值。 例如，如果指定的文件或服务器不存在，则 BITS 无法确定文件大小。
        /// 如果要从文件下载范围， 则 BytesTotal 包括要从文件下载的字节总数。
        /// </summary>
        public ulong BytesTotal;

        /// <summary>
        /// 传输的字节数。
        /// </summary>
        public ulong BytesTransferred;

        /// <summary>
        /// 要为此作业传输的文件总数。
        /// </summary>
        public uint FilesTotal;

        /// <summary>
        /// 传输的文件数。
        /// </summary>
        public uint FilesTransferred;
    }
}
