using GetStoreApp.Extensions.Console;
using GetStoreApp.Helpers.Controls.Store;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 控制台请求服务
    /// </summary>
    public static class RequestService
    {
        private static string selectedType;
        private static string selectedChannel;
        private static string linkText;

        private static List<string> TypeList = new List<string>()
        {
            "url",
            "ProductId",
        };

        private static List<string> ChannelList = new List<string>()
        {
            "WIF",
            "WIS",
            "RP",
            "Retail"
        };

        /// <summary>
        /// 有参数模式下初始化请求的数据
        /// </summary>
        public static void InitializeWithoutQueryData()
        {
            selectedType = Convert.ToInt32(ConsoleLaunchService.LaunchArgs["TypeName"]) is -1 ? TypeList[0] : TypeList[Convert.ToInt32(ConsoleLaunchService.LaunchArgs["TypeName"])];

            selectedChannel = Convert.ToInt32(ConsoleLaunchService.LaunchArgs["ChannelName"]) is -1 ? ChannelList[0] : ChannelList[Convert.ToInt32(ConsoleLaunchService.LaunchArgs["ChannelName"])];

            linkText = ConsoleLaunchService.LaunchArgs["Link"] is null ? string.Empty : Convert.ToString(ConsoleLaunchService.LaunchArgs["Link"]);
        }

        /// <summary>
        /// 无参数模式下初始化请求的数据
        /// </summary>
        public static void InitializeQueryData(int typeIndex, int channelIndex, string link)
        {
            selectedType = TypeList[typeIndex - 1];
            selectedChannel = ChannelList[channelIndex - 1];
            linkText = link;
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        public static async Task GetLinksAsync()
        {
            // 解析链接对应的产品 ID
            string productId = selectedType.Equals(TypeList[0], StringComparison.OrdinalIgnoreCase) ? QueryLinksHelper.ParseRequestContent(linkText) : linkText;

            bool requestState = true;

            while (requestState)
            {
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/GettingNow"));

                string cookie = await QueryLinksHelper.GetCookieAsync();

                List<QueryLinksModel> queryLinksList = new List<QueryLinksModel>();
                AppInfoModel appInfo = null;
                int state = 0;

                // 获取应用信息
                Tuple<bool, AppInfoModel> appInformationResult = await QueryLinksHelper.GetAppInformationAsync(productId);

                if (appInformationResult.Item1)
                {
                    // 解析非商店应用数据
                    if (string.IsNullOrEmpty(appInformationResult.Item2.CategoryID))
                    {
                        List<QueryLinksModel> nonAppxPackagesList = await QueryLinksHelper.GetNonAppxPackagesAsync(productId);
                        foreach (QueryLinksModel nonAppxPackage in nonAppxPackagesList)
                        {
                            queryLinksList.Add(nonAppxPackage);
                        }
                        state = queryLinksList.Count is 0 ? 2 : 1;
                    }
                    // 解析商店应用数据
                    else
                    {
                        string fileListXml = await QueryLinksHelper.GetFileListXmlAsync(cookie, appInformationResult.Item2.CategoryID, selectedChannel);

                        if (!string.IsNullOrEmpty(fileListXml))
                        {
                            List<QueryLinksModel> appxPackagesList = QueryLinksHelper.GetAppxPackages(fileListXml, selectedChannel);
                            foreach (QueryLinksModel appxPackage in appxPackagesList)
                            {
                                queryLinksList.Add(appxPackage);
                            }
                            state = queryLinksList.Count is 0 ? 2 : 1;
                        }
                    }

                    appInfo = appInformationResult.Item2;
                }
                else
                {
                    state = 3;
                }

                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/GetCompleted"));

                switch (state)
                {
                    case 1:
                        {
                            ConsoleHelper.SetTextColor(0x02);
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/RequestSuccessfully"));
                            ConsoleHelper.ResetTextColor();
                            requestState = false;
                            await ParseService.ParseDataAsync(appInfo, queryLinksList);
                            break;
                        }
                    case 2:
                        {
                            ConsoleHelper.SetTextColor(0x06);
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/RequestFailed"));
                            ConsoleHelper.ResetTextColor();
                            PrintRequestFailedData();
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/AskContinue"));
                            string RegainString = ConsoleHelper.ReadLine();
                            requestState = RegainString is "Y" || RegainString is "y";
                            break;
                        }
                    case 3:
                        {
                            ConsoleHelper.SetTextColor(0x04);
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/RequestError"));
                            ConsoleHelper.ResetTextColor();
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/AskContinue"));
                            string RegainString = ConsoleHelper.ReadLine();
                            requestState = RegainString is "Y" || RegainString is "y";
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 获取失败时，打印获取失败时的数据
        /// </summary>
        private static void PrintRequestFailedData()
        {
            string SerialNumberHeader = ResourceService.GetLocalized("Console/SerialNumber");
            string FileNameHeader = ResourceService.GetLocalized("Console/FileName");
            string FileSizeHeader = ResourceService.GetLocalized("Console/FileSize");
            string None = ResourceService.GetLocalized("Console/None");

            int SerialNumberHeaderLength = CharExtension.GetStringDisplayLengthEx(SerialNumberHeader);
            int FileNameHeaderLength = CharExtension.GetStringDisplayLengthEx(FileNameHeader);
            int FileSizeHeaderLength = CharExtension.GetStringDisplayLengthEx(FileSizeHeader);
            int NoneLength = CharExtension.GetStringDisplayLengthEx(None);

            int SerialNumberColumnLength = (SerialNumberHeaderLength > "1".Length ? SerialNumberHeaderLength : "1".Length) + 3;
            int FileNameColumnLength = (FileNameHeaderLength > NoneLength ? FileNameHeaderLength : NoneLength) + 3;

            ConsoleHelper.Write(Environment.NewLine);
            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/FileInfoList"));

            // 打印标题
            ConsoleHelper.Write(SerialNumberHeader + new string(ConsoleLaunchService.RowSplitCharacter, SerialNumberColumnLength - SerialNumberHeaderLength));
            ConsoleHelper.Write(FileNameHeader + new string(ConsoleLaunchService.RowSplitCharacter, FileNameColumnLength - FileNameHeaderLength));
            ConsoleHelper.Write(FileSizeHeader + Environment.NewLine);

            // 打印标题与内容的分割线
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, SerialNumberHeaderLength).PadRight(SerialNumberColumnLength));
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, FileNameHeaderLength).PadRight(FileNameColumnLength));
            ConsoleHelper.Write(new string(ConsoleLaunchService.ColumnSplitCharacter, FileSizeHeaderLength) + Environment.NewLine);

            // 输出内容
            ConsoleHelper.Write("1" + new string(ConsoleLaunchService.RowSplitCharacter, SerialNumberColumnLength - 1));
            ConsoleHelper.Write(None + new string(ConsoleLaunchService.RowSplitCharacter, FileNameColumnLength - NoneLength));
            ConsoleHelper.Write(None + Environment.NewLine);
        }
    }
}
