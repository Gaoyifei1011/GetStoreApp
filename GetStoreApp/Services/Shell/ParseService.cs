using GetStoreApp.Helpers.Controls.Home;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Services.Controls.Settings.Common;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 控制台解析服务
    /// </summary>
    public static class ParseService
    {
        private static string CategoryId = string.Empty;

        private static List<ResultModel> ResultDataList = new List<ResultModel>();

        /// <summary>
        /// 解析或得到的数据
        /// </summary>
        public static void ParseData(RequestModel requestData)
        {
            CategoryId = string.Empty;
            ResultDataList.Clear();

            HtmlParseHelper.InitializeParseData(requestData);

            CategoryId = HtmlParseHelper.HtmlParseCID();
            ResultDataList = HtmlParseHelper.HtmlParseLinks();

            ResultListFilter(ref ResultDataList);
        }

        /// <summary>
        /// 按指定条件过滤数据
        /// </summary>
        private static void ResultListFilter(ref List<ResultModel> resultDataList)
        {
            // 按要求过滤列表内容
            if (LinkFilterService.StartWithEFilterValue)
            {
                resultDataList.RemoveAll(item =>
                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (LinkFilterService.BlockMapFilterValue)
            {
                resultDataList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
