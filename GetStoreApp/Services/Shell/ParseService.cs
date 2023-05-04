using GetStoreApp.Extensions.Console;
using GetStoreApp.Helpers.Controls.Store;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Settings.Common;
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
        private static string CategoryId = string.Empty;

        public static List<ResultModel> ResultDataList = new List<ResultModel>();

        /// <summary>
        /// 解析或得到的数据
        /// </summary>
        public static async Task ParseDataAsync(RequestModel requestData)
        {
            ResultDataList.Clear();

            HtmlParseHelper.InitializeParseData(requestData);

            CategoryId = HtmlParseHelper.HtmlParseCID();
            ResultDataList = HtmlParseHelper.HtmlParseLinks();

            ResultListFilter(ref ResultDataList);

            PrintCategoryID();
            PrintResultList();
            await DownloadService.QueryDownloadIndexAsync();
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

        /// <summary>
        /// 向控制台输出获取到CategoryID
        /// </summary>
        private static void PrintCategoryID()
        {
            Console.Write(ResourceService.GetLocalized("Console/CategoryID") + CategoryId + ConsoleLaunchService.LineBreaks);
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

            Console.Write(ConsoleLaunchService.LineBreaks);
            Console.WriteLine(ResourceService.GetLocalized("Console/ResultDataList"));

            // 打印标题
            Console.Write(SerialNumberHeader + new string(ConsoleLaunchService.RowSplitCharacter, SerialNumberColumnLength - SerialNumberHeaderLength));
            Console.Write(FileNameHeader + new string(ConsoleLaunchService.RowSplitCharacter, FileNameColumnLength - FileNameHeaderLength));
            Console.Write(FileSizeHeader + ConsoleLaunchService.LineBreaks);

            // 打印标题与内容的分割线
            Console.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, SerialNumberHeaderLength).PadRight(SerialNumberColumnLength));
            Console.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, FileNameHeaderLength).PadRight(FileNameColumnLength));
            Console.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, FileSizeHeaderLength) + ConsoleLaunchService.LineBreaks);

            // 打印内容
            for (int resultDataIndex = 0; resultDataIndex < ResultDataList.Count; resultDataIndex++)
            {
                Console.Write(Convert.ToString(resultDataIndex + 1) + new string(ConsoleLaunchService.RowSplitCharacter, SerialNumberColumnLength - Convert.ToString(resultDataIndex + 1).Length));
                Console.Write(ResultDataList[resultDataIndex].FileName + new string(ConsoleLaunchService.RowSplitCharacter, FileNameColumnLength - ResultDataList[resultDataIndex].FileName.Length));
                Console.Write(ResultDataList[resultDataIndex].FileSize + ConsoleLaunchService.LineBreaks);
            }

            Console.Write(ConsoleLaunchService.LineBreaks);
        }
    }
}
