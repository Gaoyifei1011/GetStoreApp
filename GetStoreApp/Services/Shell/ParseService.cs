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
        public static List<QueryLinksModel> _queryLinksList = new List<QueryLinksModel>();

        /// <summary>
        /// 解析得到的数据
        /// </summary>
        public static async Task ParseDataAsync(AppInfoModel appInfo, List<QueryLinksModel> queryLinksList)
        {
            _queryLinksList.Clear();
            ResultListFilter(ref queryLinksList);
            foreach (QueryLinksModel queryLinksItem in queryLinksList)
            {
                _queryLinksList.Add(queryLinksItem);
            }

            PrintAppInformation(appInfo);
            PrintResultList();
            await DownloadService.QueryDownloadIndexAsync();
        }

        /// <summary>
        /// 按指定条件过滤数据
        /// </summary>
        private static void ResultListFilter(ref List<QueryLinksModel> queryLinksList)
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
        }

        /// <summary>
        /// 向控制台输出获取到的应用信息
        /// </summary>
        private static void PrintAppInformation(AppInfoModel appInfo)
        {
            ConsoleHelper.Write(ResourceService.GetLocalized("Console/AppName") + appInfo.Name + Environment.NewLine);
            ConsoleHelper.Write(ResourceService.GetLocalized("Console/AppPublisher") + appInfo.Publisher + Environment.NewLine);
            ConsoleHelper.Write(ResourceService.GetLocalized("Console/AppDescription") + Environment.NewLine + appInfo.Description + Environment.NewLine);
        }

        /// <summary>
        /// 向控制台输出获取到的结果列表
        /// </summary>
        private static void PrintResultList()
        {
            string serialNumberHeader = ResourceService.GetLocalized("Console/SerialNumber");
            string fileNameHeader = ResourceService.GetLocalized("Console/FileName");
            string fileSizeHeader = ResourceService.GetLocalized("Console/FileSize");

            int serialNumberHeaderLength = CharExtension.GetStringDisplayLengthEx(serialNumberHeader);
            int fileNameHeaderLength = CharExtension.GetStringDisplayLengthEx(fileNameHeader);
            int fileSizeHeaderLength = CharExtension.GetStringDisplayLengthEx(fileSizeHeader);

            int serialNumberColumnLength = (serialNumberHeaderLength > _queryLinksList.Count.ToString().Length ? serialNumberHeaderLength : _queryLinksList.Count.ToString().Length) + 3;

            int fileNameContentMaxLength = 0;
            foreach (QueryLinksModel queryLinksItem in _queryLinksList)
            {
                if (queryLinksItem.FileName.Length > fileNameContentMaxLength)
                {
                    fileNameContentMaxLength = queryLinksItem.FileName.Length;
                }
            }
            int FileNameColumnLength = ((fileNameHeaderLength > fileNameContentMaxLength) ? fileNameHeaderLength : fileNameContentMaxLength) + 3;

            ConsoleHelper.Write(Environment.NewLine);
            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/QueryLinksCollection"));

            // 打印标题
            ConsoleHelper.Write(serialNumberHeader + new string(ConsoleLaunchService.RowSplitCharacter, serialNumberColumnLength - serialNumberHeaderLength));
            ConsoleHelper.Write(fileNameHeader + new string(ConsoleLaunchService.RowSplitCharacter, FileNameColumnLength - fileNameHeaderLength));
            ConsoleHelper.Write(fileSizeHeader + Environment.NewLine);

            // 打印标题与内容的分割线
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, serialNumberHeaderLength).PadRight(serialNumberColumnLength));
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, fileNameHeaderLength).PadRight(FileNameColumnLength));
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, fileSizeHeaderLength) + Environment.NewLine);

            // 打印内容
            for (int resultDataIndex = 0; resultDataIndex < _queryLinksList.Count; resultDataIndex++)
            {
                ConsoleHelper.Write(Convert.ToString(resultDataIndex + 1) + new string(ConsoleLaunchService.RowSplitCharacter, serialNumberColumnLength - Convert.ToString(resultDataIndex + 1).Length));
                ConsoleHelper.Write(_queryLinksList[resultDataIndex].FileName + new string(ConsoleLaunchService.RowSplitCharacter, FileNameColumnLength - _queryLinksList[resultDataIndex].FileName.Length));
                ConsoleHelper.Write(_queryLinksList[resultDataIndex].FileSize + Environment.NewLine);
            }

            ConsoleHelper.Write(Environment.NewLine);
        }
    }
}
