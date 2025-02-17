using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 桌面应用启动服务
    /// </summary>
    public static class DesktopLaunchService
    {
        private static bool isLaunched = false;
        private static AppInstance currentInstance;

        /// <summary>
        /// 处理桌面应用启动的方式
        /// </summary>
        public static async Task InitializeLaunchAsync(AppActivationArguments appActivationArguments)
        {
            isLaunched = AppInstance.GetInstances().Count > 1;
            currentInstance = AppInstance.FindOrRegisterForKey("GetStoreApp");

            // 正常参数启动
            if (appActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                List<string> argumentsList = [];
                string sendData = string.Empty;

                string[] argumentsArray = Environment.GetCommandLineArgs();
                string executableFileName = Path.GetFileName(Environment.ProcessPath);

                foreach (string arguments in argumentsArray)
                {
                    if (arguments.Contains(executableFileName) || string.IsNullOrEmpty(arguments))
                    {
                        continue;
                    }
                    else
                    {
                        argumentsList.Add(arguments);
                    }
                }

                // 正常启动
                if (argumentsList.Count is 0)
                {
                    sendData = isLaunched ? string.Join(' ', "Launch", "IsRunning") : "Launch";
                }
                else if (argumentsList.Count is 1)
                {
                    // 应用程序重新启动
                    if (argumentsList[0] is "Restart")
                    {
                        return;
                    }
                    else if (argumentsList[0] is "Settings")
                    {
                        sendData = argumentsList[0];
                    }
                    // 带参数启动：只有一个参数，直接输入链接
                    else
                    {
                        sendData = string.Join(' ', "Console", -1, -1, argumentsList[0]);
                    }
                }
                else if (argumentsList.Count is 2)
                {
                    // 从跳转列表启动或从辅助磁贴启动
                    if (argumentsList[0] is "JumpList" || argumentsList[0] is "SecondaryTile")
                    {
                        if (argumentsList[1] is "Web")
                        {
                            await Launcher.LaunchUriAsync(new Uri("webbrowser:"));
                            Environment.Exit(Environment.ExitCode);
                        }
                        else
                        {
                            sendData = string.Join(' ', argumentsList);
                        }
                    }
                    else
                    {
                        Environment.Exit(Environment.ExitCode);
                    }
                }
                else
                {
                    // 带参数启动：包含多个参数
                    if (argumentsList.Count % 2 is 0)
                    {
                        int typeNameParameterIndex = argumentsList.FindIndex(item => item.Equals("-t", StringComparison.OrdinalIgnoreCase) || item.Equals("--type", StringComparison.OrdinalIgnoreCase));
                        int channelNameParameterIndex = argumentsList.FindIndex(item => item.Equals("-c", StringComparison.OrdinalIgnoreCase) || item.Equals("--channel", StringComparison.OrdinalIgnoreCase));
                        int linkParameterIndex = argumentsList.FindIndex(item => item.Equals("-l", StringComparison.OrdinalIgnoreCase) || item.Equals("--link", StringComparison.OrdinalIgnoreCase));

                        int typeNameIndex = typeNameParameterIndex is -1 ? -1 : ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(argumentsList[typeNameParameterIndex + 1], StringComparison.OrdinalIgnoreCase));
                        int channelNameIndex = channelNameParameterIndex is -1 ? -1 : ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(argumentsList[channelNameParameterIndex + 1], StringComparison.OrdinalIgnoreCase));
                        string link = linkParameterIndex is -1 ? null : argumentsList[linkParameterIndex + 1];

                        sendData = string.Join(' ', "Console", typeNameIndex, channelNameIndex, string.IsNullOrEmpty(link) ? "PlaceHolderText" : argumentsList[5]);
                    }
                    else
                    {
                        Environment.Exit(Environment.ExitCode);
                    }
                }

                ResultService.SaveResult(StorageDataKind.Launch, sendData);
            }
            // 通过共享目标启动
            else if (appActivationArguments.Kind is ExtendedActivationKind.ShareTarget)
            {
                ShareTargetActivatedEventArgs shareTargetActivatedEventArgs = appActivationArguments.Data as ShareTargetActivatedEventArgs;
                ShareOperation shareOperation = shareTargetActivatedEventArgs.ShareOperation;
                shareOperation.ReportCompleted();

                if (shareOperation.Data.Contains(StandardDataFormats.Uri))
                {
                    string sendData = string.Join(' ', -1, -1, Convert.ToString(await shareOperation.Data.GetUriAsync()));
                    ResultService.SaveResult(StorageDataKind.ShareTarget, sendData);
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            // 应用通知启动
            else if (appActivationArguments.Kind is ExtendedActivationKind.ToastNotification)
            {
                ToastNotificationActivatedEventArgs toastNotificationActivatedEventArgs = appActivationArguments.Data as ToastNotificationActivatedEventArgs;
                await ToastNotificationService.HandleToastNotificationAsync(toastNotificationActivatedEventArgs.Argument);
            }
            // 暂不支持以其他方式启动应用
            else
            {
                Environment.Exit(Environment.ExitCode);
            }

            // 若应用程序已启动，将启动信息重定向到已启动的应用中
            if (isLaunched)
            {
                ApplicationData.Current.SignalDataChanged();

                try
                {
                    await currentInstance.RedirectActivationToAsync(appActivationArguments);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Redirect to main instance failed", e);
                }
                finally
                {
                    Environment.Exit(Environment.ExitCode);
                }
            }
        }
    }
}
