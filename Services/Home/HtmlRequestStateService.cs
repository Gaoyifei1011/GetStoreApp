using GetStoreApp.Models;
using HtmlAgilityPack;
using System;

namespace GetStoreApp.Services.Home
{
    public class HtmlRequestStateService
    {
        private HtmlDocument HtmlDocument { get; set; }

        public int CheckRequestState(RequestModel HttpRequestData)
        {
            // 服务器请求异常，返回状态值3
            if (HttpRequestData.RequestId != 0)
            {
                return 3;
            }

            // 服务器请求成功
            else
            {
                HtmlDocument = new HtmlDocument();

                HtmlDocument.LoadHtml(HttpRequestData.RequestContent);

                string ReqeustContext = HtmlDocument.DocumentNode.SelectSingleNode("//p").InnerText;

                // 比对成功时返回的值为0，转换布尔值为False
                bool CompareResult = Convert.ToBoolean(string.Compare(ReqeustContext, "The links were successfully received from the Microsoft Store server.", StringComparison.Ordinal));

                // 成功下返回值为成功，否则返回警告
                return CompareResult ? 2 : 1;
            }
        }
    }
}
