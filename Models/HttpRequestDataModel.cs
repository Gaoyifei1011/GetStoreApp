namespace GetStoreApp.Models
{
    /// <summary>
    /// 网页数据请求内容数据模型
    /// Web page data request content data model
    /// </summary>
    public class HttpRequestDataModel
    {
        /// <summary>
        /// 向API发送请求后API传回的数据请求状态
        /// The status of the data request sent back by the API after sending a request to the API
        /// </summary>
        public int RequestId { get; set; }

        // 网页请求状态码
        /// <summary>
        /// 向API发送请求后API传回的请求状态码
        /// The status code of the request that is passed back by the API after sending the request to the API
        /// </summary>
        public string RequestStatusCode { get; set; }

        /// <summary>
        /// 向API发送请求后正常状态下传回的信息
        /// The information sent back normally after sending the request to the API
        /// </summary>
        public string RequestContent { get; set; }

        /// <summary>
        /// 向API发送请求后异常状态下传回的信息
        /// The information sent back to the API in an abnormal state after sending the request
        /// </summary>
        public string RequestExpectionContent { get; set; }

        public HttpRequestDataModel(int requestId, string requestStatusCode, string requestContent, string requestExpectionContent)
        {
            RequestId = requestId;
            RequestStatusCode = requestStatusCode;
            RequestContent = requestContent;
            RequestExpectionContent = requestExpectionContent;
        }
    }
}
