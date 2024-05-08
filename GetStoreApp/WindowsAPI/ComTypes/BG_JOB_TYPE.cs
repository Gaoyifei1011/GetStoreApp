namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 定义指定传输作业类型的常量，例如下载。
    /// </summary>
    public enum BG_JOB_TYPE
    {
        /// <summary>
        /// 指定作业将文件下载到客户端。
        /// </summary>
        BG_JOB_TYPE_DOWNLOAD,

        /// <summary>
        /// 指定作业将文件上传到服务器。
        /// </summary>
        BG_JOB_TYPE_UPLOAD,

        /// <summary>
        /// 指定作业将文件上传到服务器，并从服务器应用程序接收回复文件。
        /// </summary>
        BG_JOB_TYPE_UPLOAD_REPLY
    }
}
