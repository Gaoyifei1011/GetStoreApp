using GetStoreApp.Extensions.Console;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Shell;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 控制台应用启动方式服务
    /// </summary>
    public static class ConsoleLaunchService
    {
        // 行分隔符
        public static char RowSplitCharacter { get; } = ' ';

        // 列分隔符
        public static char ColumnSplitCharacter { get; } = '-';

        public static bool IsAppRunning { get; private set; } = true;

        private static readonly List<string> consoleLaunchArgs = [];

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
            foreach (string arg in args)
            {
                consoleLaunchArgs.Add(arg);
            }
            ConsoleEventDelegate ctrlDelegate = new(OnConsoleCtrlHandler);
            Kernel32Library.SetConsoleCtrlHandler(ctrlDelegate, true);
            DownloadSchedulerService.InitializeDownloadScheduler(false);
            DownloadSchedulerService.DownloadCreated += DownloadService.OnDownloadCreated;
            DownloadSchedulerService.DownloadProgressing += DownloadService.OnDownloadProgressing;
            DownloadSchedulerService.DownloadCompleted += DownloadService.OnDownloadCompleted;

            InitializeIntroduction();
            InitializeRequestContent();
            await CharExtension.InitializeAsync();
            await RequestService.GetLinksAsync();

            DownloadSchedulerService.CloseDownloadScheduler(false);
            DownloadSchedulerService.DownloadCreated -= DownloadService.OnDownloadCreated;
            DownloadSchedulerService.DownloadProgressing -= DownloadService.OnDownloadProgressing;
            DownloadSchedulerService.DownloadCompleted -= DownloadService.OnDownloadCompleted;
            ConsoleHelper.WriteLine(Environment.NewLine + ResourceService.GetLocalized("Console/ApplicationExit"));
        }

        /// <summary>
        /// 控制台程序捕捉键盘 Ctrl + C/Break 退出事件并询问用户是否退出
        /// </summary>
        private static bool OnConsoleCtrlHandler(int dwCtrlType)
        {
            ConsoleHelper.WriteLine(Environment.NewLine + ResourceService.GetLocalized("Console/ApplicationExit"));
            ConsoleHelper.IsExited = true;
            IsAppRunning = false;

            try
            {
                DownloadSchedulerService.TerminateDownload();
                DownloadSchedulerService.CloseDownloadScheduler(false);
                DownloadSchedulerService.DownloadCreated -= DownloadService.OnDownloadCreated;
                DownloadSchedulerService.DownloadProgressing -= DownloadService.OnDownloadProgressing;
                DownloadSchedulerService.DownloadCompleted -= DownloadService.OnDownloadCompleted;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Unregister Download scheduler service event failed", e);
            }
            return false;
        }

        /// <summary>
        /// 控制台程序运行时初始化介绍信息
        /// </summary>
        private static void InitializeIntroduction()
        {
            ConsoleHelper.SetTitle(ResourceService.GetLocalized("Console/Title"));

            ConsoleHelper.WriteLine(string.Format(ResourceService.GetLocalized("Console/HeaderDescription1"), InfoHelper.AppVersion.ToString()));
            ConsoleHelper.Write(Environment.NewLine);
            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/HeaderDescription2"));
            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/HeaderDescription3") + Environment.NewLine);
        }

        /// <summary>
        /// 初始化请求链接前要输入的信息
        /// </summary>
        private static void InitializeRequestContent()
        {
            if (consoleLaunchArgs.Count is not 1)
            {
                ParseLaunchArgs();
                RequestService.InitializeWithoutQueryData();
            }
            else
            {
                // 选择类型
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/TypeInformation"));
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/URLSample"));
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ProductIDSample"));
                ConsoleHelper.Write(ResourceService.GetLocalized("Console/SelectType"));
                int typeIndex;
                try
                {
                    if (int.TryParse(ConsoleHelper.ReadLine(), out typeIndex) && (typeIndex is < 1 or > 4))
                    {
                        typeIndex = 1;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Parse console params(type) failed.", e);
                    typeIndex = 1;
                }

                // 选择通道
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ChannelInformation"));
                ConsoleHelper.Write(ResourceService.GetLocalized("Console/SelectChannel"));
                int channelIndex;
                try
                {
                    if (int.TryParse(ConsoleHelper.ReadLine(), out channelIndex) && (channelIndex is < 1 or > 4))
                    {
                        channelIndex = 4;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Parse console params(channel) failed.", e);
                    channelIndex = 4;
                }

                // 输入链接
                ConsoleHelper.Write(ResourceService.GetLocalized("Console/InputLink"));
                string link = ConsoleHelper.ReadLine();

                RequestService.InitializeQueryData(typeIndex, channelIndex, link);
            }
        }

        /// <summary>
        /// 解析启动命令参数
        /// </summary>
        private static void ParseLaunchArgs()
        {
            if (consoleLaunchArgs.Count is 2)
            {
                LaunchArgs["Link"] = consoleLaunchArgs[1];
            }
            else
            {
                if (consoleLaunchArgs.Count % 2 is not 1)
                {
                    ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ParameterError"));
                    return;
                }

                int TypeNameIndex = consoleLaunchArgs.FindIndex(item => item.Equals("-t", StringComparison.OrdinalIgnoreCase) || item.Equals("--type", StringComparison.OrdinalIgnoreCase));
                int ChannelNameIndex = consoleLaunchArgs.FindIndex(item => item.Equals("-c", StringComparison.OrdinalIgnoreCase) || item.Equals("--channel", StringComparison.OrdinalIgnoreCase));
                int LinkIndex = consoleLaunchArgs.FindIndex(item => item.Equals("-l", StringComparison.OrdinalIgnoreCase) || item.Equals("--link", StringComparison.OrdinalIgnoreCase));

                LaunchArgs["TypeName"] = TypeNameIndex is -1 ? LaunchArgs["TypeName"] : ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(consoleLaunchArgs[TypeNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                LaunchArgs["ChannelName"] = ChannelNameIndex is -1 ? LaunchArgs["ChannelName"] : ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(consoleLaunchArgs[ChannelNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                LaunchArgs["Link"] = LinkIndex is -1 ? LaunchArgs["Link"] : consoleLaunchArgs[LinkIndex + 1];
            }
        }
    }
}
