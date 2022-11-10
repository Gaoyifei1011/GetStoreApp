using CommandLine;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.CommandLine;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.UI.Dialogs.ContentDialogs.Common;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Root
{
    public class StartupService : IStartupService
    {
        private IResourceService ResourceService = ContainerHelper.GetInstance<IResourceService>();

        private readonly string[] CommandLineArgs = Environment.GetCommandLineArgs().Where((source, index) => index != 0).ToArray();

        private ExtendedActivationKind StartupKind;

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
            await RunSingleInstanceAppAsync();
            InitializeStartupArgs();
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
                // 将激活重定向到该实例
                await mainInstance.RedirectActivationToAsync(appArgs);

                // 然后退出实例并停止
                Process.GetCurrentProcess().Kill();
                return;
            }

            // 否则将注册激活重定向
            AppInstance.GetCurrent().Activated += OnAppActivated;
        }

        private void InitializeStartupArgs()
        {
            switch (StartupKind)
            {
                // 正常方式启动（包括命令行）
                case ExtendedActivationKind.Launch:
                    {
                        NormalLaunch();
                        break;
                    }
                // 使用 Protocol协议启动
                case ExtendedActivationKind.Protocol:
                    {
                        break;
                    }
                // ToDo:使用共享任务方式启动
                case ExtendedActivationKind.ShareTarget:
                    {
                        ShareTargetLaunch();
                        break;
                    }
                // 未知方式启动
                default:
                    {
                        NormalLaunch();
                        break;
                    }
            }
        }

        /// <summary>
        /// 关闭其他实例后，按照原来的状态显示已经打开的实例窗口
        /// </summary>
        private void OnAppActivated(object sender, AppActivationArguments args)
        {
            WindowHelper.ShowAppWindow();

            if (!App.IsDialogOpening)
            {
                App.IsDialogOpening = true;
                App.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
                {
                    await new AppRunningDialog().ShowAsync();
                });
                App.IsDialogOpening = false;
            }
        }

        // 正常方式启动应用
        private void NormalLaunch()
        {
            if (CommandLineArgs.Length == 0)
            {
                return;
            }
            else if (CommandLineArgs.Length == 1)
            {
                StartupArgs["Link"] = CommandLineArgs[0];
            }
            else
            {
                Parser.Default.ParseArguments<CommandOptions>(CommandLineArgs).WithParsed((options) =>
                {
                    StartupArgs["TypeName"] = ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(options.TypeName, StringComparison.OrdinalIgnoreCase));
                    StartupArgs["ChannelName"] = ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(options.ChannelName, StringComparison.OrdinalIgnoreCase));
                    StartupArgs["Link"] = options.Link;
                });
            }
        }

        // 使用共享任务方式启动
        private void ShareTargetLaunch()
        {
        }
    }
}
