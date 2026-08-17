namespace GetStoreApp.Models
{
    /// <summary>
    /// 网页请求数据模型
    /// </summary>
    internal sealed class RequestModel
    {
        /// <summary>
        /// 网页请求返回的ID值
        /// </summary>
        internal int RequestId { get; set; }

        /// <summary>
        /// 网页请求返回的状态码
        /// </summary>
        internal string RequestStatusCode { get; set; }

        /// <summary>
        /// 正常网页请求返回的信息
        /// </summary>
        internal string RequestContent { get; set; }

        /// <summary>
        /// 异常网页请求返回的信息
        /// </summary>
        internal string RequestExceptionContent { get; set; }
    }
}
