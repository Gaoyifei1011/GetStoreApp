using GetStoreApp.Core.Models;

using HtmlAgilityPack;

using System.Collections.ObjectModel;
using System.Linq;

namespace GetStoreApp.Services.Main
{
    public class HtmlParseService
    {
        private HtmlDocument htmlDocument;

        public ObservableCollection<ResultDataModel> ResultDataList;

        public HtmlParseService(HttpRequestDataModel HttpRequestData)
        {
            htmlDocument = new HtmlDocument();
            // 添加网页请求返回的具体内容
            htmlDocument.LoadHtml(HttpRequestData.RequestContent);

            ResultDataList = new ObservableCollection<ResultDataModel>();
        }

        /// <summary>
        /// 检查服务器返回的内容是空列表还是带有具体内容的信息
        /// </summary>
        /// <returns></returns>
        public bool IsSuccessfulRequest()
        {
            // 加载带有返回结果的标签
            HtmlNode RequestStateNode = htmlDocument.DocumentNode.SelectSingleNode("//p");

            // 加载带有返回结果的文本信息
            string ReqeustContext = RequestStateNode.InnerText;

            // 服务器获取到正确的信息
            if (ReqeustContext == "The links were successfully received from the Microsoft Store server.")
            {
                return true;
            }
            // 服务器获取到的内容不正确
            else if (ReqeustContext == "The server returned an empty list.<br>Either you have not entered the link correctly, or this service does not support generation for this product.")
            {
                return false;
            }
            // 其他状况
            return false;
        }

        // 解析网页内容
        public string HtmlParseCID()
        {
            // 加载带有返回内容的标签
            HtmlNode RequestCIDNode = htmlDocument.DocumentNode.SelectSingleNode("//i");

            // 获取服务器返回应用的CategoryID信息
            string CategoryID = RequestCIDNode.InnerText;

            return CategoryID;
        }

        public ObservableCollection<ResultDataModel> HtmlParseLinks()
        {
            // 获取<table class="tftable" border="1" align="center">标签信息
            HtmlNode RequestLinkNode = htmlDocument.DocumentNode.SelectSingleNode("//table[@class='tftable' and @border='1' and @align='center']");

            // 获取<table class="tftable" border="1" align="center">下所有的tr标签
            var RequestLinkNodeList = RequestLinkNode
                 .Descendants("tr")
                 .Where(x => x.Attributes.Contains("style"));

            // 集合不为空时，清空列表
            if (ResultDataList.Count != 0)
            {
                ResultDataList.Clear();
            }

            // 遍历tr标签集合
            foreach (var item in RequestLinkNodeList)
            {
                // 访问每一个tr标签的所有td标签，td标签包括应用包名称，链接名称，链接过期时间，应用包文件SHA-1值，应用包文件大小
                HtmlNodeCollection TdNodeList = item.ChildNodes;

                // 应用包名称
                string AppPackageName = TdNodeList[0].InnerText;

                // 链接名称
                string AppLink = TdNodeList[0].SelectSingleNode("a").Attributes["href"].Value;

                // 链接过期时间
                string AppLinkExpireTime = TdNodeList[1].InnerText;

                // 应用包文件SHA-1值
                string AppSHA1 = TdNodeList[2].InnerText;

                // 应用包文件大小
                string AppPackageSize = TdNodeList[3].InnerText;

                // 将获取到的数据添加到ResultDataList集合中
                ResultDataList.Add(new ResultDataModel(AppPackageName, AppLink, AppLinkExpireTime, AppSHA1, AppPackageSize));
            }

            return ResultDataList;
        }
    }
}
