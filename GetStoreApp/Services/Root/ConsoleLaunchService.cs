using GetStoreApp.Extensions.Console;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Shell;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using WinRT;

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
        public static async Task InitializeLaunchAsync(AppActivationArguments appActivationArguments)
        {
            Windows.ApplicationModel.Activation.LaunchActivatedEventArgs launchActivatedEventArgs = Windows.ApplicationModel.Activation.LaunchActivatedEventArgs.FromAbi((appActivationArguments.Data as IInspectable).ThisPtr);
            string[] argumentsArray = launchActivatedEventArgs.Arguments.Split(' ');

            foreach (string arguments in argumentsArray)
            {
                if (arguments.Contains("GetStoreApp.exe") || string.IsNullOrEmpty(arguments))
                {
                    continue;
                }
                else
                {
                    consoleLaunchArgs.Add(arguments);
                }
            }

            ConsoleEventDelegate ctrlDelegate = new(OnConsoleCtrlHandler);
            Kernel32Library.SetConsoleCtrlHandler(ctrlDelegate, true);
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
            int typeNameIndex = 1;
            int channelNameIndex = 4;
            string link = string.Empty;

            // 手动输入参数
            if (consoleLaunchArgs.Count is 1)
            {
                // 选择类型
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/TypeInformation"));
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/URLSample"));
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ProductIDSample"));
                ConsoleHelper.Write(ResourceService.GetLocalized("Console/SelectType"));

                try
                {
                    if (int.TryParse(ConsoleHelper.ReadLine(), out typeNameIndex) && (typeNameIndex is < 1 or > 4))
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
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ChannelInformation"));
                ConsoleHelper.Write(ResourceService.GetLocalized("Console/SelectChannel"));

                try
                {
                    if (int.TryParse(ConsoleHelper.ReadLine(), out channelNameIndex) && (channelNameIndex is < 1 or > 4))
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
                ConsoleHelper.Write(ResourceService.GetLocalized("Console/InputLink"));
                link = ConsoleHelper.ReadLine();
            }
            // 只有两个参数：链接
            else if (consoleLaunchArgs.Count is 2)
            {
                link = consoleLaunchArgs[1];
            }
            // 多个参数
            else if (consoleLaunchArgs.Count % 2 is 1)
            {
                int typeNameParameterIndex = consoleLaunchArgs.FindIndex(item => item.Equals("-t", StringComparison.OrdinalIgnoreCase) || item.Equals("--type", StringComparison.OrdinalIgnoreCase));
                int channelNameParameterIndex = consoleLaunchArgs.FindIndex(item => item.Equals("-c", StringComparison.OrdinalIgnoreCase) || item.Equals("--channel", StringComparison.OrdinalIgnoreCase));
                int linkParameterIndex = consoleLaunchArgs.FindIndex(item => item.Equals("-l", StringComparison.OrdinalIgnoreCase) || item.Equals("--link", StringComparison.OrdinalIgnoreCase));

                typeNameIndex = typeNameParameterIndex is -1 ? 1 : ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(consoleLaunchArgs[typeNameParameterIndex + 1], StringComparison.OrdinalIgnoreCase));
                channelNameIndex = channelNameParameterIndex is -1 ? 4 : ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(consoleLaunchArgs[channelNameParameterIndex + 1], StringComparison.OrdinalIgnoreCase));
                link = linkParameterIndex is -1 ? string.Empty : consoleLaunchArgs[linkParameterIndex + 1];
            }
            // 非法参数
            else
            {
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/ParameterError"));
                Environment.Exit(Environment.ExitCode);
            }

            RequestService.InitializeQueryData(typeNameIndex, channelNameIndex, link);
        }
    }
}
