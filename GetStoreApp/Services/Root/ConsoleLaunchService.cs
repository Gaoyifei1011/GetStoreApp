using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Shell;
using GetStoreApp.WindowsAPI.PInvoke.Comctl32;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Collections.Generic;
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
        public static async Task InitializeConsoleStartupAsync()
        {
            Console.CancelKeyPress += OnConsoleCancelKeyPress;
            InitializeIntroduction();
            InitializeRequestContent();
            await RequestService.GetLinksAsync();

            Console.WriteLine(LineBreaks + ResourceService.GetLocalized("Console/ApplicationExit"));
        }

        /// <summary>
        /// 控制台程序捕捉键盘 Ctrl + C 退出事件并询问用户是否退出
        /// </summary>
        private static void OnConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            if (args.SpecialKey is ConsoleSpecialKey.ControlC)
            {
                IntPtr ConsoleHandle = Kernel32Library.GetConsoleWindow();

                Comctl32Library.TaskDialog(
                    ConsoleHandle,
                    IntPtr.Zero,
                    ResourceService.GetLocalized("Resources/AppDisplayName"),
                    ResourceService.GetLocalized("Console/ExitPrompt"),
                    string.Empty,
                    TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON | TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CANCEL_BUTTON,
                    TASKDIALOGICON.TD_SHIELD_WARNING_YELLOW_BAR,
                    out TaskDialogResult Result
                    );

                if (Result is TaskDialogResult.IDOK)
                {
                    Console.WriteLine(LineBreaks + ResourceService.GetLocalized("Console/ApplicationExit"));
                    IsAppRunning = false;
                    DownloadService.StopDownloadFile();
                    Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
                }
            }
        }

        /// <summary>
        /// 控制台程序运行时初始化介绍信息
        /// </summary>
        private static void InitializeIntroduction()
        {
            Console.Title = ResourceService.GetLocalized("Console/Title");

            Console.WriteLine(string.Format(ResourceService.GetLocalized("Console/HeaderDescription1"),
                InfoHelper.GetAppVersion().Major,
                InfoHelper.GetAppVersion().Minor,
                InfoHelper.GetAppVersion().Build,
                InfoHelper.GetAppVersion().Revision
                ) + LineBreaks);
            Console.WriteLine(ResourceService.GetLocalized("Console/HeaderDescription2"));
            Console.WriteLine(ResourceService.GetLocalized("Console/HeaderDescription3") + LineBreaks);
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
                Console.WriteLine(ResourceService.GetLocalized("Console/TypeInformation"));
                Console.Write(ResourceService.GetLocalized("Console/SelectType"));
                int typeIndex;
                try
                {
                    typeIndex = int.Parse(Console.ReadLine());
                    if (typeIndex < 1 || typeIndex > 4)
                    {
                        typeIndex = 1;
                    }
                }
                catch (Exception)
                {
                    typeIndex = 1;
                }

                // 选择通道
                Console.WriteLine(ResourceService.GetLocalized("Console/ChannelInformation"));
                Console.Write(ResourceService.GetLocalized("Console/SelectChannel"));
                int channelIndex;
                try
                {
                    channelIndex = int.Parse(Console.ReadLine());
                    if (channelIndex < 1 || channelIndex > 4)
                    {
                        channelIndex = 4;
                    }
                }
                catch (Exception)
                {
                    channelIndex = 4;
                }

                // 输入链接
                Console.Write(ResourceService.GetLocalized("Console/InputLink"));
                string link = Console.ReadLine();

                RequestService.InitializeQueryData(typeIndex, channelIndex, link);
            }
        }

        /// <summary>
        /// 获取控制台应用的启动方式
        /// </summary>
        private static ConsoleLaunchModeArgs GetLaunchMode()
        {
            if (Program.CommandLineArgs.Count is 1)
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
            if (Program.CommandLineArgs.Count is 2)
            {
                LaunchArgs["Link"] = Program.CommandLineArgs[1];
            }
            else
            {
                if (Program.CommandLineArgs.Count % 2 is not 1)
                {
                    Console.WriteLine(ResourceService.GetLocalized("Console/ParameterError"));
                    return;
                }

                int TypeNameIndex = Program.CommandLineArgs.FindIndex(item => item.Equals("-t", StringComparison.OrdinalIgnoreCase) || item.Equals("--type", StringComparison.OrdinalIgnoreCase));
                int ChannelNameIndex = Program.CommandLineArgs.FindIndex(item => item.Equals("-c", StringComparison.OrdinalIgnoreCase) || item.Equals("--channel", StringComparison.OrdinalIgnoreCase));
                int LinkIndex = Program.CommandLineArgs.FindIndex(item => item.Equals("-l", StringComparison.OrdinalIgnoreCase) || item.Equals("--link", StringComparison.OrdinalIgnoreCase));

                LaunchArgs["TypeName"] = TypeNameIndex is -1 ? LaunchArgs["TypeName"] : ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(Program.CommandLineArgs[TypeNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                LaunchArgs["ChannelName"] = ChannelNameIndex is -1 ? LaunchArgs["ChannelName"] : ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(Program.CommandLineArgs[ChannelNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                LaunchArgs["Link"] = LinkIndex is -1 ? LaunchArgs["Link"] : Program.CommandLineArgs[LinkIndex + 1];
            }
        }
    }
}
