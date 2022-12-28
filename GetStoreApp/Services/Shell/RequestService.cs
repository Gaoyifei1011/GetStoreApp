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
            SelectedType = Convert.ToInt32(ConsoleLaunchService.LaunchArgs["TypeName"]) == -1 ? TypeList[0] : TypeList[Convert.ToInt32(ConsoleLaunchService.LaunchArgs["TypeName"])];

            SelectedChannel = Convert.ToInt32(ConsoleLaunchService.LaunchArgs["ChannelName"]) == -1 ? ChannelList[0] : ChannelList[Convert.ToInt32(ConsoleLaunchService.LaunchArgs["ChannelName"])];

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
                Console.WriteLine(ResourceService.GetLocalized("/Console/GetCompleted"));

                int state = HtmlRequestStateHelper.CheckRequestState(httpRequestData);

                switch (state)
                {
                    case 1:
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(ResourceService.GetLocalized("/Console/RequestSuccessfully"));
                            Console.ResetColor();
                            RequestState = true;
                            // 解析内容并打印输出
                            break;
                        }
                    case 2:
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(ResourceService.GetLocalized("/Console/RequestFailed"));
                            Console.ResetColor();
                            PrintRequestFailedData();
                            Console.WriteLine(ResourceService.GetLocalized("/Console/AskContinue"));
                            string RegainString = Console.ReadLine();
                            RequestState = RegainString == "Y" || RegainString == "y";
                            break;
                        }
                    case 3:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ResourceService.GetLocalized("/Console/RequestError"));
                            Console.ResetColor();
                            Console.WriteLine(ResourceService.GetLocalized("/Console/AskContinue"));
                            string RegainString = Console.ReadLine();
                            RequestState = RegainString == "Y" || RegainString == "y";
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
            Console.Write(ResourceService.GetLocalized("/Console/GettingNow") + new string('.', ElapsedTime % 4));
            ElapsedTime++;
        }

        /// <summary>
        /// 获取失败时，打印获取失败时的数据
        /// </summary>
        private static void PrintRequestFailedData()
        {
            int Column1EmptyLength = 10;
            int Column2EmptyLength = 15;

            string FileNameHeader = ResourceService.GetLocalized("/Console/FileName");
            string FileSizeHeader = ResourceService.GetLocalized("/Console/FileSIze");
            string None = ResourceService.GetLocalized("/Console/None");
            string SerialNumberHeader = ResourceService.GetLocalized("/Console/SerialNumber");

            int FileNameHeaderLength = CharExtension.GetStringDisplayLengthEx(FileNameHeader);
            int NoneLength = CharExtension.GetStringDisplayLength(None);
            int SerialNumberHeaderLength = CharExtension.GetStringDisplayLengthEx(SerialNumberHeader);

            Console.Write(ConsoleLaunchService.LineBreaks);

            Console.Write(SerialNumberHeader);
            if (SerialNumberHeaderLength < Column1EmptyLength)
            {
                Console.Write(new string(' ', Column1EmptyLength - SerialNumberHeaderLength));
            }

            Console.Write(FileNameHeader);
            if (SerialNumberHeaderLength < Column2EmptyLength)
            {
                Console.Write(new string(' ', Column2EmptyLength - FileNameHeaderLength));
            }
            Console.Write(FileSizeHeader + ConsoleLaunchService.LineBreaks);

            Console.Write(new string('-', CharExtension.GetStringDisplayLengthEx(SerialNumberHeader)).PadRight(Column1EmptyLength));
            Console.Write(new string('-', CharExtension.GetStringDisplayLengthEx(FileNameHeader)).PadRight(Column2EmptyLength));
            Console.Write(new string('-', CharExtension.GetStringDisplayLengthEx(FileSizeHeader)) + ConsoleLaunchService.LineBreaks);

            if (SerialNumberHeaderLength < Column1EmptyLength)
            {
                Console.Write("1" + new string(' ', Column1EmptyLength - 1));
            }
            else
            {
                Console.Write("1" + new string(' ', SerialNumberHeaderLength - 1));
            }

            if (FileNameHeaderLength < Column2EmptyLength)
            {
                Console.Write(None + new string(' ', Column2EmptyLength - NoneLength));
            }
            else
            {
                Console.Write(None + new string(' ', FileNameHeaderLength - NoneLength));
            }

            Console.WriteLine(ResourceService.GetLocalized("/Console/None") + ConsoleLaunchService.LineBreaks);
        }
    }
}
