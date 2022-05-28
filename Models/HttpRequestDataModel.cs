namespace GetStoreApp.Models
{
    public class HttpRequestDataModel
    {
        // 数据的请求状态
        public int RequestId { get; set; }

        // 网页请求状态码
        public string RequestStatusCode { get; set; }

        // 数据请求状态正常时包含的数据内容
        public string RequestContent { get; set; }

        // 数据请求状态异常时包含的异常信息
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