using GetStoreApp.Extensions.Console;
using GetStoreApp.Helpers.Controls.Home;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 控制台请求服务
    /// </summary>
    public static class RequestService
    {
        private static string SelectedType { get; set; }

        private static string SelectedChannel { get; set; }

        private static string LinkText { get; set; }

        private static Timer ConsoleTimer { get; } = new Timer(1000);

        private static int ElapsedTime { get; set; } = 0;

        private static List<string> TypeList { get; } = new List<string>()
        {
            "url",
            "ProductId",
            "PackageFamilyName",
            "CategoryId"
        };

        private static List<string> ChannelList { get; } = new List<string>()
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
            string generateContent = GenerateContentHelper.GenerateRequestContent(SelectedType, LinkText, SelectedChannel, RegionService.AppRegion.ISO2);
            bool RequestState = true;

            while (RequestState)
            {
                ConsoleTimer.Elapsed += OnRequestElasped;
                ConsoleTimer.AutoReset = true;
                ConsoleTimer.Start();

                // 获取网页反馈回的原始数据
                RequestModel httpRequestData = await HtmlRequestHelper.HttpRequestAsync(generateContent);

                ConsoleTimer.Stop();
                ConsoleTimer.Elapsed -= OnRequestElasped;
                ElapsedTime = 0;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine(ResourceService.GetLocalized("Console/GetCompleted"));

                int state = HtmlRequestStateHelper.CheckRequestState(httpRequestData);

                switch (state)
                {
                    case 1:
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(ResourceService.GetLocalized("Console/RequestSuccessfully"));
                            Console.ResetColor();
                            RequestState = false;
                            await ParseService.ParseDataAsync(httpRequestData);
                            break;
                        }
                    case 2:
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(ResourceService.GetLocalized("Console/RequestFailed"));
                            Console.ResetColor();
                            PrintRequestFailedData();
                            Console.WriteLine(ResourceService.GetLocalized("Console/AskContinue"));
                            string RegainString = Console.ReadLine();
                            RequestState = RegainString is "Y" || RegainString is "y";
                            break;
                        }
                    case 3:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ResourceService.GetLocalized("Console/RequestError"));
                            Console.ResetColor();
                            Console.WriteLine(ResourceService.GetLocalized("Console/AskContinue"));
                            string RegainString = Console.ReadLine();
                            RequestState = RegainString is "Y" || RegainString is "y";
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 显示正在获取中的动画
        /// </summary>
        private static void OnRequestElasped(object sender, ElapsedEventArgs args)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(ResourceService.GetLocalized("Console/GettingNow") + new string('.', ElapsedTime % 4));
            ElapsedTime++;
        }

        /// <summary>
        /// 获取失败时，打印获取失败时的数据
        /// </summary>
        private static void PrintRequestFailedData()
        {
            string SerialNumberHeader = ResourceService.GetLocalized("Console/SerialNumber");
            string FileNameHeader = ResourceService.GetLocalized("Console/FileName");
            string FileSizeHeader = ResourceService.GetLocalized("Console/FileSize");
            string None = ResourceService.GetLocalized("Console/STPF_NONE");

            int SerialNumberHeaderLength = CharExtension.GetStringDisplayLengthEx(SerialNumberHeader);
            int FileNameHeaderLength = CharExtension.GetStringDisplayLengthEx(FileNameHeader);
            int FileSizeHeaderLength = CharExtension.GetStringDisplayLengthEx(FileSizeHeader);
            int NoneLength = CharExtension.GetStringDisplayLengthEx(None);

            int SerialNumberColumnLength = (SerialNumberHeaderLength > "1".Length ? SerialNumberHeaderLength : "1".Length) + 3;
            int FileNameColumnLength = (FileNameHeaderLength > NoneLength ? FileNameHeaderLength : NoneLength) + 3;

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

            // 输出内容
            Console.Write("1" + new string(ConsoleLaunchService.RowSplitCharacter, SerialNumberColumnLength - 1));
            Console.Write(None + new string(ConsoleLaunchService.RowSplitCharacter, FileNameColumnLength - NoneLength));
            Console.Write(None + ConsoleLaunchService.LineBreaks);
        }
    }
}
