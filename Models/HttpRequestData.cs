namespace GetStoreApp.Models
{
    /// <summary>
    /// 网页数据请求内容数据模型
    /// Web page data request content data model
    /// </summary>
    public class HttpRequestData
    {
        public int RequestId { get; set; }

        public string RequestStatusCode { get; set; }

        public string RequestContent { get; set; }

        public string RequestExpectionContent { get; set; }
    }
}
