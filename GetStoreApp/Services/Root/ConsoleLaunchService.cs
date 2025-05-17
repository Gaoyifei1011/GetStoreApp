using GetStoreApp.Extensions.Console;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Shell;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.IO;
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

        private static readonly List<string> consoleLaunchArgsList = [];

        /// <summary>
        /// 处理控制台应用启动的方式
        /// </summary>
        public static async Task InitializeLaunchAsync(AppActivationArguments appActivationArguments)
        {
            // 正常参数启动
            if (appActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                string[] argumentsArray = Environment.GetCommandLineArgs();
                string executableFileName = Path.GetFileName(Environment.ProcessPath);

                if (argumentsArray.Length > 0 && string.Equals(Path.GetExtension(argumentsArray[0]), ".dll", StringComparison.OrdinalIgnoreCase))
                {
                    argumentsArray[0] = argumentsArray[0].Replace(".dll", ".exe");
                }

                foreach (string arguments in argumentsArray)
                {
                    if (arguments.Contains(executableFileName) || string.IsNullOrEmpty(arguments))
                    {
                        continue;
                    }
                    else
                    {
                        consoleLaunchArgsList.Add(arguments);
                    }
                }

                Console.CancelKeyPress += OnCancelKeyPress;
                DownloadSchedulerService.InitializeDownloadScheduler(false);
                DownloadSchedulerService.DownloadCreated += DownloadService.OnDownloadCreated;
                DownloadSchedulerService.DownloadProgressing += DownloadService.OnDownloadProgressing;
                DownloadSchedulerService.DownloadCompleted += DownloadService.OnDownloadCompleted;

                InitializeIntroduction();
                InitializeRequestContent();
                CharExtension.Initialize();
                await RequestService.GetLinksAsync();

                DownloadSchedulerService.CloseDownloadScheduler(false);
                DownloadSchedulerService.DownloadCreated -= DownloadService.OnDownloadCreated;
                DownloadSchedulerService.DownloadProgressing -= DownloadService.OnDownloadProgressing;
                DownloadSchedulerService.DownloadCompleted -= DownloadService.OnDownloadCompleted;
                Console.WriteLine(Environment.NewLine + ResourceService.GetLocalized("Console/ApplicationExit"));
            }
        }

        /// <summary>
        /// 控制台程序捕捉键盘 Ctrl + C/Break 退出事件并询问用户是否退出
        /// </summary>
        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine(Environment.NewLine + ResourceService.GetLocalized("Console/ApplicationExit"));
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
        }

        /// <summary>
        /// 控制台程序运行时初始化介绍信息
        /// </summary>
        private static void InitializeIntroduction()
        {
            Console.Title = ResourceService.GetLocalized("Console/Title");
            Console.WriteLine(string.Format(ResourceService.GetLocalized("Console/HeaderDescription1"), InfoHelper.AppVersion.ToString()));
            Console.Write(Environment.NewLine);
            Console.WriteLine(ResourceService.GetLocalized("Console/HeaderDescription2"));
            Console.WriteLine(ResourceService.GetLocalized("Console/HeaderDescription3") + Environment.NewLine);
        }

        /// <summary>
        /// 初始化请求链接前要输入的信息
        /// </summary>
        private static void InitializeRequestContent()
        {
            int typeNameIndex = 1;
            int channelNameIndex = 4;
            string link = string.Empty;

            // 手动输入参数
            if (consoleLaunchArgsList.Count is 1)
            {
                // 选择类型
                Console.WriteLine(ResourceService.GetLocalized("Console/TypeInformation"));
                Console.WriteLine(ResourceService.GetLocalized("Console/URLSample"));
                Console.WriteLine(ResourceService.GetLocalized("Console/ProductIDSample"));
                Console.Write(ResourceService.GetLocalized("Console/SelectType"));

                try
                {
                    if (int.TryParse(Console.ReadLine(), out typeNameIndex) && (typeNameIndex is < 1 or > 4))
                    {
                        typeNameIndex = 1;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Parse console params(type) failed.", e);
                    typeNameIndex = 1;
                }

                // 选择通道
                Console.WriteLine(ResourceService.GetLocalized("Console/ChannelInformation"));
                Console.Write(ResourceService.GetLocalized("Console/SelectChannel"));

                try
                {
                    if (int.TryParse(Console.ReadLine(), out channelNameIndex) && (channelNameIndex is < 1 or > 4))
                    {
                        channelNameIndex = 4;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Parse console params(channel) failed.", e);
                    channelNameIndex = 4;
                }

                // 输入链接
                Console.Write(ResourceService.GetLocalized("Console/InputLink"));
                link = Console.ReadLine();
            }
            // 只有两个参数：链接
            else if (consoleLaunchArgsList.Count is 2)
            {
                link = consoleLaunchArgsList[1];
            }
            // 多个参数
            else if (consoleLaunchArgsList.Count % 2 is 1)
            {
                int typeNameParameterIndex = consoleLaunchArgsList.FindIndex(item => string.Equals(item, "-t", StringComparison.OrdinalIgnoreCase) || string.Equals(item, "--type", StringComparison.OrdinalIgnoreCase));
                int channelNameParameterIndex = consoleLaunchArgsList.FindIndex(item => string.Equals(item, "-c", StringComparison.OrdinalIgnoreCase) || string.Equals(item, "--channel", StringComparison.OrdinalIgnoreCase));
                int linkParameterIndex = consoleLaunchArgsList.FindIndex(item => string.Equals(item, "-l", StringComparison.OrdinalIgnoreCase) || string.Equals(item, "--link", StringComparison.OrdinalIgnoreCase));

                typeNameIndex = typeNameParameterIndex is -1 ? 1 : ResourceService.TypeList.FindIndex(item => string.Equals(item.ShortName, consoleLaunchArgsList[typeNameParameterIndex + 1], StringComparison.OrdinalIgnoreCase));
                channelNameIndex = channelNameParameterIndex is -1 ? 4 : ResourceService.ChannelList.FindIndex(item => string.Equals(item.ShortName, consoleLaunchArgsList[channelNameParameterIndex + 1], StringComparison.OrdinalIgnoreCase));
                link = linkParameterIndex is -1 ? string.Empty : consoleLaunchArgsList[linkParameterIndex + 1];
            }
            // 非法参数
            else
            {
                Console.WriteLine(ResourceService.GetLocalized("Console/ParameterError"));
                Environment.Exit(Environment.ExitCode);
            }

            RequestService.InitializeQueryData(typeNameIndex, channelNameIndex, link);
        }
    }
}
