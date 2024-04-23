using GetStoreApp.Models.Controls.Store;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetStoreApp.Helpers.Controls.Store
{
    /// <summary>
    /// 网页解析辅助类
    /// </summary>
    public static partial class HtmlParseHelper
    {
        [GeneratedRegex(@"<i>(.*?)<\/i>", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex CIDRegularExpression();

        private static Regex CIDRegex = CIDRegularExpression();

        [GeneratedRegex(@"<tr\sstyle=\\?""background-color:rgba\(\d{3},\s\d{3},\s\d{3},\s0.8\)\\?"">\s{0,}<td>\s{0,}<a\shref=\\?""(.*?)\\?""\srel=\\?""noreferrer\\?"">(.*?)<\/a>\s{0,}<\/td>\s{0,}<td\salign=\\?""center\\?"">(.*?GMT)<\/td>\s{0,}<td\salign=\\?""center\\?"">(.*?)<\/td>\s{0,}<td\salign=\\?""center\\?"">(.*?)<\/td>\s{0,}<\/tr>", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex ResultDataListRegularExpression();

        private static Regex ResultDataListRegex = ResultDataListRegularExpression();

        private static string ParseContent = string.Empty;

        /// <summary>
        /// 初始化HtmlParseService类时添加HtmlReqeustHelper生成的字符串数据
        /// </summary>
        public static void InitializeParseData(RequestModel HttpRequestData)
        {
            ParseContent = HttpRequestData.RequestContent;
        }

        /// <summary>
        /// 解析网页数据中包含的CategoryID信息
        /// </summary>
        public static string HtmlParseCID()
        {
            if (!string.IsNullOrEmpty(ParseContent))
            {
                MatchCollection CIDCollection = CIDRegex.Matches(ParseContent);
                if (CIDCollection.Count > 0)
                {
                    GroupCollection CIDGroups = CIDCollection[0].Groups;

                    if (CIDGroups.Count > 0)
                    {
                        return CIDGroups[1].Value;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 解析网页数据中包含的所有信息
        /// </summary>
        public static List<QueryLinksModel> HtmlParseLinks()
        {
            List<QueryLinksModel> ResultDataList = new List<QueryLinksModel>();

            if (!string.IsNullOrEmpty(ParseContent))
            {
                MatchCollection ResultDataListCollection = ResultDataListRegex.Matches(ParseContent);

                foreach (Match matchItem in ResultDataListCollection)
                {
                    GroupCollection ResultDataListGroups = matchItem.Groups;

                    if (ResultDataListGroups.Count is 6)
                    {
                        QueryLinksModel queryLinksData = new QueryLinksModel();
                        queryLinksData.FileLink = ResultDataListGroups[1].Value;
                        queryLinksData.FileName = ResultDataListGroups[2].Value;
                        queryLinksData.FileLinkExpireTime = ResultDataListGroups[3].Value;
                        queryLinksData.FileSHA1 = ResultDataListGroups[4].Value;
                        queryLinksData.FileSize = ResultDataListGroups[5].Value;

                        ResultDataList.Add(queryLinksData);
                    }
                }
            }
            return ResultDataList;
        }
    }
}
