namespace GetStoreApp.Models.Controls.Store
{
    /// <summary>
    /// 网页请求数据模型
    /// </summary>
    public class RequestModel
    {
        /// <summary>
        /// 网页请求返回的ID值
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// 网页请求返回的状态码
        /// </summary>
        public string RequestStatusCode { get; set; }

        /// <summary>
        /// 正常网页请求返回的信息
        /// </summary>
        public string RequestContent { get; set; }

        /// <summary>
        /// 异常网页请求返回的信息
        /// </summary>
        public string RequestExceptionContent { get; set; }
    }
}
