using GetStoreApp.Extensions.Console;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 控制台解析服务
    /// </summary>
    public static class ParseService
    {
        /// <summary>
        /// 解析得到的数据
        /// </summary>
        public static async Task ParseDataAsync(AppInfoModel appInfo, List<QueryLinksModel> queryLinksList)
        {
            // 按要求过滤列表内容
            if (LinkFilterService.EncryptedPackageFilterValue)
            {
                queryLinksList.RemoveAll(item =>
                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (LinkFilterService.BlockMapFilterValue)
            {
                queryLinksList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
            }

            if (appInfo is not null)
            {
                PrintAppInformation(appInfo);
            }

            PrintResultList(queryLinksList);
            await DownloadService.QueryDownloadIndexAsync(queryLinksList);
        }

        /// <summary>
        /// 向控制台输出获取到的应用信息
        /// </summary>
        private static void PrintAppInformation(AppInfoModel appInfo)
        {
            ConsoleHelper.Write(ResourceService.GetLocalized("Console/AppName") + appInfo.Name + Environment.NewLine);
            ConsoleHelper.Write(ResourceService.GetLocalized("Console/AppPublisher") + appInfo.Publisher + Environment.NewLine);
            ConsoleHelper.Write(ResourceService.GetLocalized("Console/AppDescription") + Environment.NewLine + appInfo.Description + Environment.NewLine);
            ConsoleHelper.Write(Environment.NewLine);
        }

        /// <summary>
        /// 向控制台输出获取到的结果列表
        /// </summary>
        private static void PrintResultList(List<QueryLinksModel> queryLinksList)
        {
            string serialNumberHeader = ResourceService.GetLocalized("Console/SerialNumber");
            string fileNameHeader = ResourceService.GetLocalized("Console/FileName");
            string fileSizeHeader = ResourceService.GetLocalized("Console/FileSize");

            int serialNumberHeaderLength = CharExtension.GetStringDisplayLengthEx(serialNumberHeader);
            int fileNameHeaderLength = CharExtension.GetStringDisplayLengthEx(fileNameHeader);
            int fileSizeHeaderLength = CharExtension.GetStringDisplayLengthEx(fileSizeHeader);

            int serialNumberColumnLength = (serialNumberHeaderLength > Convert.ToString(queryLinksList.Count).Length ? serialNumberHeaderLength : Convert.ToString(queryLinksList.Count).Length) + 3;

            int fileNameContentMaxLength = 0;
            foreach (QueryLinksModel queryLinksItem in queryLinksList)
            {
                if (queryLinksItem.FileName.Length > fileNameContentMaxLength)
                {
                    fileNameContentMaxLength = queryLinksItem.FileName.Length;
                }
            }
            int fileNameColumnLength = ((fileNameHeaderLength > fileNameContentMaxLength) ? fileNameHeaderLength : fileNameContentMaxLength) + 3;

            ConsoleHelper.WriteLine(string.Format(ResourceService.GetLocalized("Console/QueryLinksCount"), queryLinksList.Count));
            ConsoleHelper.Write(Environment.NewLine);

            // 打印标题
            ConsoleHelper.Write(serialNumberHeader + new string(ConsoleLaunchService.RowSplitCharacter, serialNumberColumnLength - serialNumberHeaderLength));
            ConsoleHelper.Write(fileNameHeader + new string(ConsoleLaunchService.RowSplitCharacter, fileNameColumnLength - fileNameHeaderLength));
            ConsoleHelper.Write(fileSizeHeader + Environment.NewLine);

            // 打印标题与内容的分割线
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, serialNumberHeaderLength).PadRight(serialNumberColumnLength));
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, fileNameHeaderLength).PadRight(fileNameColumnLength));
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, fileSizeHeaderLength) + Environment.NewLine);

            // 打印内容
            for (int resultDataIndex = 0; resultDataIndex < queryLinksList.Count; resultDataIndex++)
            {
                ConsoleHelper.Write(Convert.ToString(resultDataIndex + 1) + new string(ConsoleLaunchService.RowSplitCharacter, serialNumberColumnLength - Convert.ToString(resultDataIndex + 1).Length));
                ConsoleHelper.Write(queryLinksList[resultDataIndex].FileName + new string(ConsoleLaunchService.RowSplitCharacter, fileNameColumnLength - queryLinksList[resultDataIndex].FileName.Length));
                ConsoleHelper.Write(queryLinksList[resultDataIndex].FileSize + Environment.NewLine);
            }

            ConsoleHelper.Write(Environment.NewLine);
        }
    }
}
