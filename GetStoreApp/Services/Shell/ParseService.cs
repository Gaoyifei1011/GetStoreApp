using GetStoreApp.Extensions.Console;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 控制台解析服务
    /// </summary>
    public static class ParseService
    {
        public static List<ResultModel> ResultDataList = new List<ResultModel>();

        /// <summary>
        /// 解析得到的数据
        /// </summary>
        public static async Task ParseDataAsync(AppInfoModel appInfo, List<ResultModel> resultList)
        {
            ResultDataList.Clear();
            ResultListFilter(ref resultList);
            foreach (ResultModel resultItem in resultList)
            {
                ResultDataList.Add(resultItem);
            }

            PrintAppInformation(appInfo);
            PrintResultList();
            await DownloadService.QueryDownloadIndexAsync();
        }

        /// <summary>
        /// 按指定条件过滤数据
        /// </summary>
        private static void ResultListFilter(ref List<ResultModel> resultList)
        {
            // 按要求过滤列表内容
            if (LinkFilterService.EncryptedPackageFilterValue)
            {
                resultList.RemoveAll(item =>
                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (LinkFilterService.BlockMapFilterValue)
            {
                resultList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
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
            string SerialNumberHeader = ResourceService.GetLocalized("Console/SerialNumber");
            string FileNameHeader = ResourceService.GetLocalized("Console/FileName");
            string FileSizeHeader = ResourceService.GetLocalized("Console/FileSize");

            int SerialNumberHeaderLength = CharExtension.GetStringDisplayLengthEx(SerialNumberHeader);
            int FileNameHeaderLength = CharExtension.GetStringDisplayLengthEx(FileNameHeader);
            int FileSizeHeaderLength = CharExtension.GetStringDisplayLengthEx(FileSizeHeader);

            int SerialNumberColumnLength = (SerialNumberHeaderLength > ResultDataList.Count.ToString().Length ? SerialNumberHeaderLength : ResultDataList.Count.ToString().Length) + 3;

            int FileNameContentMaxLength = 0;
            foreach (ResultModel resultItem in ResultDataList.Where(resultItem => resultItem.FileName.Length > FileNameContentMaxLength))
            {
                FileNameContentMaxLength = resultItem.FileName.Length;
            }
            int FileNameColumnLength = ((FileNameHeaderLength > FileNameContentMaxLength) ? FileNameHeaderLength : FileNameContentMaxLength) + 3;

            ConsoleHelper.Write(Environment.NewLine);
            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ResultCollection"));

            // 打印标题
            ConsoleHelper.Write(SerialNumberHeader + new string(ConsoleLaunchService.RowSplitCharacter, SerialNumberColumnLength - SerialNumberHeaderLength));
            ConsoleHelper.Write(FileNameHeader + new string(ConsoleLaunchService.RowSplitCharacter, FileNameColumnLength - FileNameHeaderLength));
            ConsoleHelper.Write(FileSizeHeader + Environment.NewLine);

            // 打印标题与内容的分割线
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, SerialNumberHeaderLength).PadRight(SerialNumberColumnLength));
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, FileNameHeaderLength).PadRight(FileNameColumnLength));
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, FileSizeHeaderLength) + Environment.NewLine);

            // 打印内容
            for (int resultDataIndex = 0; resultDataIndex < ResultDataList.Count; resultDataIndex++)
            {
                ConsoleHelper.Write(Convert.ToString(resultDataIndex + 1) + new string(ConsoleLaunchService.RowSplitCharacter, SerialNumberColumnLength - Convert.ToString(resultDataIndex + 1).Length));
                ConsoleHelper.Write(ResultDataList[resultDataIndex].FileName + new string(ConsoleLaunchService.RowSplitCharacter, FileNameColumnLength - ResultDataList[resultDataIndex].FileName.Length));
                ConsoleHelper.Write(ResultDataList[resultDataIndex].FileSize + Environment.NewLine);
            }

            ConsoleHelper.Write(Environment.NewLine);
        }
    }
}
