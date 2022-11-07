using GetStoreAppCore.Data;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace GetStoreAppCore.Html
{
    /// <summary>
    /// 网页解析
    /// </summary>
    public class HtmlParseHelper
    {
        private HtmlDocument HtmlDocument { get; set; }

        /// <summary>
        /// 初始化HtmlParseService类时添加HtmlReqeustService生成的字符串数据
        /// </summary>
        /// <param name="HttpRequestData">HtmlReqeustService生成的数据</param>
        public HtmlParseHelper(RequestData HttpRequestData)
        {
            HtmlDocument = new HtmlDocument();

            // 添加网页请求返回的具体内容
            HtmlDocument.LoadHtml(HttpRequestData.RequestContent);
        }

        /// <summary>
        /// 解析网页数据中包含的CategoryID信息
        /// Parse the CategoryID information contained in the web page data
        /// </summary>
        public string HtmlParseCID()
        {
            return HtmlDocument.DocumentNode.SelectSingleNode("//i").InnerText;
        }

        /// <summary>
        /// 解析网页数据中包含的所有信息
        /// Parse all the information contained in the web page data
        /// </summary>
        public List<ResultData> HtmlParseLinks()
        {
            List<ResultData> ResultDataList = new List<ResultData>();

            HtmlNode RequestLinkNode = HtmlDocument.DocumentNode.SelectSingleNode("//table[@class='tftable' and @border='1' and @align='center']");

            IEnumerable<HtmlNode> RequestLinkNodeList = RequestLinkNode
                 .Descendants("tr")
                 .Where(x => x.Attributes.Contains("style"));

            foreach (HtmlNode htmlNode in RequestLinkNodeList)
            {
                HtmlNodeCollection TdNodeList = htmlNode.ChildNodes;

                ResultDataList.Add(new ResultData
                {
                    FileName = TdNodeList[0].InnerText,
                    FileLink = TdNodeList[0].SelectSingleNode("a").Attributes["href"].Value,
                    FileLinkExpireTime = TdNodeList[1].InnerText,
                    FileSHA1 = TdNodeList[2].InnerText,
                    FileSize = TdNodeList[3].InnerText
                });
            }

            return ResultDataList;
        }
    }
}
