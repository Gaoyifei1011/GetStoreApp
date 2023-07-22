using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Shell;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 控制台应用启动方式服务
    /// </summary>
    public static class ConsoleLaunchService
    {
        // 换行符
        public static string LineBreaks = "\r\n";

        // 行分隔符
        public static char RowSplitCharacter = ' ';

        // 列分隔符
        public static char ColumnSplitCharacter = '-';

        public static bool IsAppRunning = true;

        private static List<string> ConsoleLaunchArgs;

        /// <summary>
        /// 应用启动时使用的参数
        /// </summary>
        public static Dictionary<string, object> LaunchArgs { get; set; } = new Dictionary<string, object>()
        {
            {"TypeName",-1 },
            {"ChannelName",-1 },
            {"Link",null},
        };

        /// <summary>
        /// 处理控制台应用启动的方式
        /// </summary>
        public static async Task InitializeLaunchAsync(string[] args)
        {
            ConsoleLaunchArgs = args.ToList();
            ConsoleEventDelegate ctrlDelegate = new ConsoleEventDelegate(OnConsoleCtrlHandler);
            Kernel32Library.SetConsoleCtrlHandler(ctrlDelegate, true);

            InitializeIntroduction();
            InitializeRequestContent();
            await RequestService.GetLinksAsync();

            ConsoleHelper.WriteLine(LineBreaks + ResourceService.GetLocalized("Console/ApplicationExit"));
        }

        /// <summary>
        /// 控制台程序捕捉键盘 Ctrl + C/Break 退出事件并询问用户是否退出
        /// </summary>
        private static bool OnConsoleCtrlHandler(int dwCtrlType)
        {
            ConsoleHelper.WriteLine(LineBreaks + ResourceService.GetLocalized("Console/ApplicationExit"));
            ConsoleHelper.IsExited = true;
            IsAppRunning = false;
            DownloadService.StopDownloadFile();
            return false;
        }

        /// <summary>
        /// 控制台程序运行时初始化介绍信息
        /// </summary>
        private static void InitializeIntroduction()
        {
            ConsoleHelper.SetTitle(ResourceService.GetLocalized("Console/Title"));

            ConsoleHelper.WriteLine(string.Format(ResourceService.GetLocalized("Console/HeaderDescription1"),
                InfoHelper.AppVersion.Major,
                InfoHelper.AppVersion.Minor,
                InfoHelper.AppVersion.Build,
                InfoHelper.AppVersion.Revision
                ) + LineBreaks);
            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/HeaderDescription2"));
            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/HeaderDescription3") + LineBreaks);
        }

        /// <summary>
        /// 初始化请求链接前要输入的信息
        /// </summary>
        private static void InitializeRequestContent()
        {
            if (GetLaunchMode() is ConsoleLaunchModeArgs.WithoutQuery)
            {
                ParseLaunchArgs();
                RequestService.InitializeWithoutQueryData();
            }
            else
            {
                // 选择类型
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/TypeInformation"));
                ConsoleHelper.Write(ResourceService.GetLocalized("Console/SelectType"));
                int typeIndex;
                try
                {
                    typeIndex = int.Parse(ConsoleHelper.ReadLine());
                    if (typeIndex < 1 || typeIndex > 4)
                    {
                        typeIndex = 1;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogType.WARNING, "Parse console params(type) failed.", e);
                    typeIndex = 1;
                }

                // 选择通道
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ChannelInformation"));
                ConsoleHelper.Write(ResourceService.GetLocalized("Console/SelectChannel"));
                int channelIndex;
                try
                {
                    channelIndex = int.Parse(ConsoleHelper.ReadLine());
                    if (channelIndex < 1 || channelIndex > 4)
                    {
                        channelIndex = 4;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogType.WARNING, "Parse console params(channel) failed.", e);
                    channelIndex = 4;
                }

                // 输入链接
                ConsoleHelper.Write(ResourceService.GetLocalized("Console/InputLink"));
                string link = ConsoleHelper.ReadLine();

                RequestService.InitializeQueryData(typeIndex, channelIndex, link);
            }
        }

        /// <summary>
        /// 获取控制台应用的启动方式
        /// </summary>
        private static ConsoleLaunchModeArgs GetLaunchMode()
        {
            if (ConsoleLaunchArgs.Count is 1)
            {
                return ConsoleLaunchModeArgs.NeedQuery;
            }
            else
            {
                return ConsoleLaunchModeArgs.WithoutQuery;
            }
        }

        /// <summary>
        /// 解析启动命令参数
        /// </summary>
        private static void ParseLaunchArgs()
        {
            if (ConsoleLaunchArgs.Count is 2)
            {
                LaunchArgs["Link"] = ConsoleLaunchArgs[1];
            }
            else
            {
                if (ConsoleLaunchArgs.Count % 2 is not 1)
                {
                    ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ParameterError"));
                    return;
                }

                int TypeNameIndex = ConsoleLaunchArgs.FindIndex(item => item.Equals("-t", StringComparison.OrdinalIgnoreCase) || item.Equals("--type", StringComparison.OrdinalIgnoreCase));
                int ChannelNameIndex = ConsoleLaunchArgs.FindIndex(item => item.Equals("-c", StringComparison.OrdinalIgnoreCase) || item.Equals("--channel", StringComparison.OrdinalIgnoreCase));
                int LinkIndex = ConsoleLaunchArgs.FindIndex(item => item.Equals("-l", StringComparison.OrdinalIgnoreCase) || item.Equals("--link", StringComparison.OrdinalIgnoreCase));

                LaunchArgs["TypeName"] = TypeNameIndex is -1 ? LaunchArgs["TypeName"] : ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(ConsoleLaunchArgs[TypeNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                LaunchArgs["ChannelName"] = ChannelNameIndex is -1 ? LaunchArgs["ChannelName"] : ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(ConsoleLaunchArgs[ChannelNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                LaunchArgs["Link"] = LinkIndex is -1 ? LaunchArgs["Link"] : ConsoleLaunchArgs[LinkIndex + 1];
            }
        }
    }
}
