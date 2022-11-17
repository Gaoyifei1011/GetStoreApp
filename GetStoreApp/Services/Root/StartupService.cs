using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreAppWindowsAPI.PInvoke.User32;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer.ShareTarget;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用启动服务
    /// </summary>
    public class StartupService : IStartupService
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private IAppNotificationService AppNotificationService { get; } = ContainerHelper.GetInstance<IAppNotificationService>();

        private readonly List<string> CommandLineArgs = Environment.GetCommandLineArgs().Where((source, index) => index != 0).ToList();

        private ExtendedActivationKind StartupKind;

        private int NeedToSendMesage;

        // 应用启动时使用的参数
        public Dictionary<string, object> StartupArgs { get; set; } = new Dictionary<string, object>
        {
            {"TypeName",-1 },
            {"ChannelName",-1 },
            {"Link",null},
        };

        /// <summary>
        /// 处理应用启动的方式
        /// </summary>
        public async Task InitializeStartupAsync()
        {
            StartupKind = AppInstance.GetCurrent().GetActivatedEventArgs().Kind;
            await InitializeStartupKindAsync();
            await RunSingleInstanceAppAsync();
        }

        /// <summary>
        /// 初始化启动命令方式
        /// </summary>
        private async Task InitializeStartupKindAsync()
        {
            switch (StartupKind)
            {
                // 正常方式启动（包括命令行）
                case ExtendedActivationKind.Launch:
                    {
                        ParseStartupArgs();
                        break;
                    }
                // 使用 Protocol协议启动
                case ExtendedActivationKind.Protocol:
                    {
                        NeedToSendMesage = 0;
                        break;
                    }
                // ToDo:使用共享目标方式启动
                case ExtendedActivationKind.ShareTarget:
                    {
                        NeedToSendMesage = 1;
                        ShareOperation shareOperation = (AppInstance.GetCurrent().GetActivatedEventArgs().Data as ShareTargetActivatedEventArgs).ShareOperation;
                        shareOperation.ReportCompleted();
                        StartupArgs["Link"] = Convert.ToString(await shareOperation.Data.GetUriAsync());
                        break;
                    }
                // 从系统通知处启动
                case ExtendedActivationKind.AppNotification:
                    {
                        AppNotificationService.HandleAppNotification(AppInstance.GetCurrent().GetActivatedEventArgs().Data as AppNotificationActivatedEventArgs);
                        break;
                    }
                // 其他方式
                default:
                    {
                        ParseStartupArgs();
                        break;
                    }
            }
        }

        /// <summary>
        /// 初始化启动命令参数
        /// </summary>
        private void ParseStartupArgs()
        {
            if (CommandLineArgs.Count == 0)
            {
                NeedToSendMesage = 0;
                return;
            }
            else if (CommandLineArgs.Count == 1)
            {
                NeedToSendMesage = 1;
                StartupArgs["Link"] = CommandLineArgs[0];
            }
            else
            {
                NeedToSendMesage = 1;

                if (CommandLineArgs.Count % 2 != 0)
                {
                    return;
                }

                int TypeNameIndex = CommandLineArgs.FindIndex(item => item.Equals("-t", StringComparison.OrdinalIgnoreCase) || item.Equals("--type", StringComparison.OrdinalIgnoreCase));
                int ChannelNameIndex = CommandLineArgs.FindIndex(item => item.Equals("-c", StringComparison.OrdinalIgnoreCase) || item.Equals("--channel", StringComparison.OrdinalIgnoreCase));
                int LinkIndex = CommandLineArgs.FindIndex(item => item.Equals("-l", StringComparison.OrdinalIgnoreCase) || item.Equals("--link", StringComparison.OrdinalIgnoreCase));

                StartupArgs["TypeName"] = TypeNameIndex == -1 ? StartupArgs["TypeName"] : ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(CommandLineArgs[TypeNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                StartupArgs["ChannelName"] = ChannelNameIndex == -1 ? StartupArgs["ChannelName"] : ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(CommandLineArgs[ChannelNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                StartupArgs["Link"] = LinkIndex == -1 ? StartupArgs["Link"] : CommandLineArgs[LinkIndex + 1];
            }
        }

        /// <summary>
        /// 应用程序只运行单个实例
        /// </summary>
        private async Task RunSingleInstanceAppAsync()
        {
            // 获取已经激活的参数
            AppActivationArguments appArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            // 获取或注册主实例
            AppInstance mainInstance = AppInstance.FindOrRegisterForKey("Main");

            // 如果主实例不是此当前实例
            if (!mainInstance.IsCurrent)
            {
                // 将激活重定向到主实例
                await mainInstance.RedirectActivationToAsync(appArgs);

                App.MainWindow.Title = "WinUI Desktop";

                // 向主实例发送数据
                CopyDataStruct copyDataStruct;
                copyDataStruct.dwData = NeedToSendMesage;
                copyDataStruct.lpData = string.Format("{0} {1} {2}", StartupArgs["TypeName"], StartupArgs["ChannelName"], StartupArgs["Link"] is null ? "PlaceHolderText" : StartupArgs["Link"]);
                copyDataStruct.cbData = System.Text.Encoding.Default.GetBytes(copyDataStruct.lpData).Length + 1;

                // 向主进程发送消息
                User32Library.SendMessage(User32Library.FindWindow(null, ResourceService.GetLocalized("AppDisplayName")), WindowMessage.WM_COPYDATA, 0, ref copyDataStruct);

                // 然后退出实例并停止
                Process.GetCurrentProcess().Kill();
                return;
            }
        }
    }
}
