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
        private static string parseContent = string.Empty;

        [GeneratedRegex(@"<i>(.*?)<\/i>", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex CIDRegex { get; }

        [GeneratedRegex(@"<tr\sstyle=\\?""background-color:rgba\(\d{3},\s\d{3},\s\d{3},\s0.8\)\\?"">\s{0,}<td>\s{0,}<a\shref=\\?""(.*?)\\?""\srel=\\?""noreferrer\\?"">(.*?)<\/a>\s{0,}<\/td>\s{0,}<td\salign=\\?""center\\?"">(.*?GMT)<\/td>\s{0,}<td\salign=\\?""center\\?"">(.*?)<\/td>\s{0,}<td\salign=\\?""center\\?"">(.*?)<\/td>\s{0,}<\/tr>", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex PackagedAppRegex { get; }

        [GeneratedRegex(@"<a href=""([\s\S]*?)"" rel=""noreferrer"">([\s\S]*?)<\/a><\/td><td align=""center"">([\s\S]*?)<\/td><\/tr>", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex NonPackagedAppRegex { get; }

        /// <summary>
        /// 初始化 HtmlParseService 类时添加 HtmlReqeustHelper 生成的字符串数据
        /// </summary>
        public static void InitializeParseData(RequestModel httpRequestData)
        {
            parseContent = httpRequestData.RequestContent;
        }

        /// <summary>
        /// 解析网页数据中包含的 CategoryID 信息
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
        /// 解析网页数据中包含的打包应用所有信息
        /// </summary>
        public static List<QueryLinksModel> HtmlParsePackagedAppLinks()
        {
            List<QueryLinksModel> resultDataList = [];

            if (!string.IsNullOrEmpty(parseContent))
            {
                MatchCollection resultDataListCollection = PackagedAppRegex.Matches(parseContent);

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
                            FileSHA256 = ResultDataListGroups[4].Value,
                            FileSize = ResultDataListGroups[5].Value
                        };

                        resultDataList.Add(queryLinksData);
                    }
                }
            }
            return resultDataList;
        }

        /// <summary>
        /// 解析网页数据中包含的非打包应用所有信息
        /// </summary>
        public static List<QueryLinksModel> HtmlParseNonPackagedAppLinks()
        {
            List<QueryLinksModel> resultDataList = [];

            if (!string.IsNullOrEmpty(parseContent))
            {
                MatchCollection resultDataListCollection = NonPackagedAppRegex.Matches(parseContent);

                for (int index = 0; index < resultDataListCollection.Count; index++)
                {
                    Match matchItem = resultDataListCollection[index];
                    GroupCollection ResultDataListGroups = matchItem.Groups;

                    if (ResultDataListGroups.Count is 4)
                    {
                        QueryLinksModel queryLinksData = new()
                        {
                            FileLink = ResultDataListGroups[1].Value,
                            FileName = ResultDataListGroups[2].Value,
                            FileLinkExpireTime = string.Empty,
                            FileSHA256 = ResultDataListGroups[3].Value,
                            FileSize = string.Empty
                        };

                        resultDataList.Add(queryLinksData);
                    }
                }
            }
            return resultDataList;
        }
    }
}
