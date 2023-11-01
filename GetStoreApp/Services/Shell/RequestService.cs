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
        private static string SelectedType;
        private static string SelectedChannel;
        private static string LinkText;

        private static List<string> TypeList = new List<string>()
        {
            "url",
            "ProductId",
            "PackageFamilyName",
            "CategoryId"
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
            SelectedType = Convert.ToInt32(ConsoleLaunchService.LaunchArgs["TypeName"]) is -1 ? TypeList[0] : TypeList[Convert.ToInt32(ConsoleLaunchService.LaunchArgs["TypeName"])];

            SelectedChannel = Convert.ToInt32(ConsoleLaunchService.LaunchArgs["ChannelName"]) is -1 ? ChannelList[0] : ChannelList[Convert.ToInt32(ConsoleLaunchService.LaunchArgs["ChannelName"])];

            LinkText = ConsoleLaunchService.LaunchArgs["Link"] is null ? string.Empty : Convert.ToString(ConsoleLaunchService.LaunchArgs["Link"]);
        }

        /// <summary>
        /// 无参数模式下初始化请求的数据
        /// </summary>
        public static void InitializeQueryData(int typeIndex, int channelIndex, string link)
        {
            SelectedType = TypeList[typeIndex - 1];
            SelectedChannel = ChannelList[channelIndex - 1];
            LinkText = link;
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        public static async Task GetLinksAsync()
        {
            string generateContent = GenerateContentHelper.GenerateRequestContent(SelectedType, LinkText, SelectedChannel);
            bool RequestState = true;

            while (RequestState)
            {
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/GettingNow"));

                // 获取网页反馈回的原始数据
                RequestModel httpRequestData = await HtmlRequestHelper.HttpRequestAsync(generateContent);

                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/GetCompleted"));

                int state = HtmlRequestStateHelper.CheckRequestState(httpRequestData);

                switch (state)
                {
                    case 1:
                        {
                            ConsoleHelper.SetTextColor(0x02);
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/RequestSuccessfully"));
                            ConsoleHelper.ResetTextColor();
                            RequestState = false;
                            await ParseService.ParseDataAsync(httpRequestData);
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
                            RequestState = RegainString is "Y" || RegainString is "y";
                            break;
                        }
                    case 3:
                        {
                            ConsoleHelper.SetTextColor(0x04);
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/RequestError"));
                            ConsoleHelper.ResetTextColor();
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/AskContinue"));
                            string RegainString = ConsoleHelper.ReadLine();
                            RequestState = RegainString is "Y" || RegainString is "y";
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
            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ResultCollection"));

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
