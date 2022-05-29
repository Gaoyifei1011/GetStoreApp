using GetStoreApp.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Home
{
    /// <summary>
    /// 解析Post请求后API返回的状态值
    /// The status value returned by the API after parsing the Post request
    /// </summary>
    public class HtmlRequestStateService
    {
        private HtmlDocument HtmlDocument { get; set; }

        /// <summary>
        /// 状态检查
        /// Status checks
        /// </summary>
        /// <param name="HttpRequestData">HtmlRequestService获得到的数据</param>
        /// <returns>对应的状态值</returns>
        public int CheckRequestState(HttpRequestData HttpRequestData)
        {
            // 服务器请求异常，返回状态值0
            if (HttpRequestData.RequestId != 0)
            {
                return 0;
            }

            // 服务器请求成功
            else
            {
                HtmlDocument = new HtmlDocument();

                // 添加网页请求返回的具体内容
                HtmlDocument.LoadHtml(HttpRequestData.RequestContent);

                // 加载带有返回结果的文本信息
                string ReqeustContext = HtmlDocument.DocumentNode.SelectSingleNode("//p").InnerText;

                // 成功接收到数据，返回状态值1
                if (Convert.ToBoolean(string.Compare(ReqeustContext, "The links were successfully received from the Microsoft Store server.")))
                {
                    return 1;
                }

                // 错误接收数据，返回状态值3
                else
                {
                    return 3;
                }
            }
        }
    }
}
