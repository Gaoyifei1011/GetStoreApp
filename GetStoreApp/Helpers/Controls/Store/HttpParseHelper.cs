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
        [GeneratedRegex(@"<index>(.*?)<\/index>", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex CIDRegularExpression();

        private static readonly Regex CIDRegex = CIDRegularExpression();

        [GeneratedRegex(@"<tr\sstyle=\\?""background-color:rgba\(\d{3},\s\d{3},\s\d{3},\s0.8\)\\?"">\s{0,}<td>\s{0,}<a\shref=\\?""(.*?)\\?""\srel=\\?""noreferrer\\?"">(.*?)<\/a>\s{0,}<\/td>\s{0,}<td\salign=\\?""center\\?"">(.*?GMT)<\/td>\s{0,}<td\salign=\\?""center\\?"">(.*?)<\/td>\s{0,}<td\salign=\\?""center\\?"">(.*?)<\/td>\s{0,}<\/tr>", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex ResultDataListRegularExpression();

        private static readonly Regex ResultDataListRegex = ResultDataListRegularExpression();

        private static string parseContent = string.Empty;

        /// <summary>
        /// 初始化HtmlParseService类时添加HtmlReqeustHelper生成的字符串数据
        /// </summary>
        public static void InitializeParseData(RequestModel httpRequestData)
        {
            parseContent = httpRequestData.RequestContent;
        }

        /// <summary>
        /// 解析网页数据中包含的CategoryID信息
        /// </summary>
        public static string HtmlParseCID()
        {
            if (!string.IsNullOrEmpty(parseContent))
            {
                MatchCollection cidCollection = CIDRegex.Matches(parseContent);
                if (cidCollection.Count > 0)
                {
                    GroupCollection cidGroups = cidCollection[0].Groups;

                    if (cidGroups.Count > 0)
                    {
                        return cidGroups[1].Value;
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
            List<QueryLinksModel> resultDataList = [];

            if (!string.IsNullOrEmpty(parseContent))
            {
                MatchCollection resultDataListCollection = ResultDataListRegex.Matches(parseContent);

                for (int index = 0; index < resultDataListCollection.Count; index++)
                {
                    Match matchItem = resultDataListCollection[index];
                    GroupCollection ResultDataListGroups = matchItem.Groups;

                    if (ResultDataListGroups.Count is 6)
                    {
                        QueryLinksModel queryLinksData = new()
                        {
                            FileLink = ResultDataListGroups[1].Value,
                            FileName = ResultDataListGroups[2].Value,
                            FileLinkExpireTime = ResultDataListGroups[3].Value,
                            FileSHA1 = ResultDataListGroups[4].Value,
                            FileSize = ResultDataListGroups[5].Value
                        };

                        resultDataList.Add(queryLinksData);
                    }
                }
            }
            return resultDataList;
        }
    }
}
