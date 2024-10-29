using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Views.Pages;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
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
        private static AppActivationArguments appActivationArguments;
        private static IActivatedEventArgs activatedEventArgs;
        private static ActivationKind activationKind = ActivationKind.Launch;
        private static readonly List<string> desktopLaunchArgs = [];

        // 应用启动时使用的参数
        public static Dictionary<string, object> LaunchArgs { get; set; } = new Dictionary<string, object>()
        {
            {"TypeName", -1 },
            {"ChannelName", -1 },
            {"Link", null},
        };

        public static Type InitializePage { get; set; } = typeof(StorePage);

        /// <summary>
        /// 处理桌面应用启动的方式
        /// </summary>
        public static async Task InitializeLaunchAsync(string[] args)
        {
            foreach (string arg in args)
            {
                desktopLaunchArgs.Add(arg);
            }

            isLaunched = AppInstance.GetInstances().Count > 1;
            appActivationArguments = AppInstance.GetCurrent().GetActivatedEventArgs();
            activatedEventArgs = appActivationArguments.Data as IActivatedEventArgs;
            await ParseStartupKindAsync(activatedEventArgs is null ? ActivationKind.Launch : activatedEventArgs.Kind);
            await DealLaunchArgsAsync();
        }

        /// <summary>
        /// 解析启动命令方式
        /// </summary>
        private static async Task ParseStartupKindAsync(ActivationKind kind)
        {
            // 使用共享目标方式启动
            if (kind is ActivationKind.ShareTarget)
            {
                activationKind = ActivationKind.ShareTarget;
                ShareOperation shareOperation = (activatedEventArgs as ShareTargetActivatedEventArgs).ShareOperation;
                shareOperation.ReportCompleted();
                LaunchArgs["Link"] = Convert.ToString(await shareOperation.Data.GetUriAsync());
            }
            // 系统通知处启动
            else if (kind is ActivationKind.ToastNotification)
            {
                activationKind = ActivationKind.ToastNotification;
            }
            // 其他启动方式
            else
            {
                activationKind = kind;

                // 无参数，正常启动
                if (desktopLaunchArgs.Count is 0)
                {
                    activationKind = ActivationKind.Launch;
                    return;
                }
                // 一个参数，可能为重新启动，或者是只输入了链接
                else if (desktopLaunchArgs.Count is 1)
                {
                    if (desktopLaunchArgs[0] is "Restart")
                    {
                        activationKind = ActivationKind.CommandLineLaunch;
                        return;
                    }
                    else
                    {
                        activationKind = ActivationKind.CommandLineLaunch;
                        LaunchArgs["Link"] = desktopLaunchArgs[0];
                    }
                }
                // 多个参数，可能为跳转列表启动或者控制台输入了参数
                else
                {
                    activationKind = ActivationKind.CommandLineLaunch;

                    // 跳转列表或辅助磁贴启动的参数
                    if (desktopLaunchArgs[0] is "JumpList" || desktopLaunchArgs[0] is "SecondaryTile")
                    {
                        switch (desktopLaunchArgs[1])
                        {
                            case "AppUpdate":
                                {
                                    InitializePage = typeof(AppUpdatePage);
                                    break;
                                }
                            case "WinGet":
                                {
                                    InitializePage = typeof(WinGetPage);
                                    break;
                                }
                            case "AppManager":
                                {
                                    InitializePage = typeof(AppManagerPage);
                                    break;
                                }
                            case "Download":
                                {
                                    InitializePage = typeof(DownloadPage);
                                    break;
                                }
                            case "Web":
                                {
                                    await Launcher.LaunchUriAsync(new Uri("webbrowser:"));
                                    Environment.Exit(Environment.ExitCode);
                                    break;
                                }
                        }
                    }

                    // 命令行启动带参数
                    else
                    {
                        if (desktopLaunchArgs.Count % 2 is not 0) return;

                        int typeNameIndex = desktopLaunchArgs.FindIndex(item => item.Equals("-t", StringComparison.OrdinalIgnoreCase) || item.Equals("--type", StringComparison.OrdinalIgnoreCase));
                        int channelNameIndex = desktopLaunchArgs.FindIndex(item => item.Equals("-c", StringComparison.OrdinalIgnoreCase) || item.Equals("--channel", StringComparison.OrdinalIgnoreCase));
                        int linkIndex = desktopLaunchArgs.FindIndex(item => item.Equals("-l", StringComparison.OrdinalIgnoreCase) || item.Equals("--link", StringComparison.OrdinalIgnoreCase));

                        LaunchArgs["TypeName"] = typeNameIndex is -1 ? LaunchArgs["TypeName"] : ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(desktopLaunchArgs[typeNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                        LaunchArgs["ChannelName"] = channelNameIndex is -1 ? LaunchArgs["ChannelName"] : ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(desktopLaunchArgs[channelNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                        LaunchArgs["Link"] = linkIndex is -1 ? LaunchArgs["Link"] : desktopLaunchArgs[linkIndex + 1];
                    }
                }
            }
        }

        /// <summary>
        /// 处理应用启动参数
        /// </summary>
        private static async Task DealLaunchArgsAsync()
        {
            AppInstance currentInstance = AppInstance.FindOrRegisterForKey("GetStoreApp");

            // 正常启动
            if (activationKind is ActivationKind.Launch)
            {
                string sendData = string.Format("{0} {1} {2}", LaunchArgs["TypeName"], LaunchArgs["ChannelName"], LaunchArgs["Link"] is null ? "PlaceHolderText" : LaunchArgs["Link"]);
                ResultService.SaveResult(StorageDataKind.DesktopLaunch, sendData);

                if (isLaunched)
                {
                    ApplicationData.Current.SignalDataChanged();

                    try
                    {
                        await currentInstance.RedirectActivationToAsync(AppInstance.GetCurrent().GetActivatedEventArgs());
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Redirect to main instance failed", e);
                    }
                    finally
                    {
                        Environment.Exit(0);
                    }
                }
            }
            // 命令参数启动或者共享目标启动
            else if (activationKind is ActivationKind.CommandLineLaunch || activationKind is ActivationKind.ShareTarget)
            {
                // 重新启动
                if (desktopLaunchArgs.Count > 0 && desktopLaunchArgs[0] is "Restart")
                {
                    return;
                }
                // 带命令参数启动
                else
                {
                    string sendData;
                    // 跳转列表启动
                    if (desktopLaunchArgs.Count is 2 && desktopLaunchArgs[0] is "JumpList")
                    {
                        sendData = string.Join(" ", desktopLaunchArgs);
                    }
                    // 辅助磁贴启动
                    else if (desktopLaunchArgs.Count is 4 && desktopLaunchArgs[0] is "SecondaryTile")
                    {
                        sendData = string.Join(' ', new string[] { desktopLaunchArgs[0], desktopLaunchArgs[1] });
                    }
                    else
                    {
                        sendData = string.Format("{0} {1} {2}", LaunchArgs["TypeName"], LaunchArgs["ChannelName"], LaunchArgs["Link"] is null ? "PlaceHolderText" : LaunchArgs["Link"]);
                    }

                    if (activationKind is ActivationKind.CommandLineLaunch)
                    {
                        ResultService.SaveResult(StorageDataKind.CommandLineLaunch, sendData);
                    }
                    else
                    {
                        ResultService.SaveResult(StorageDataKind.ShareTarget, sendData);
                    }

                    if (isLaunched)
                    {
                        ApplicationData.Current.SignalDataChanged();

                        try
                        {
                            await currentInstance.RedirectActivationToAsync(AppInstance.GetCurrent().GetActivatedEventArgs());
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Redirect to main instance failed", e);
                        }
                        finally
                        {
                            Environment.Exit(0);
                        }
                    }
                }
            }
            // 应用通知启动
            else if (activationKind is ActivationKind.ToastNotification)
            {
                string sendData = desktopLaunchArgs[0];

                ResultService.SaveResult(StorageDataKind.ToastNotification, sendData);

                if (!isLaunched)
                {
                    await ToastNotificationService.HandleToastNotificationAsync(desktopLaunchArgs[0], false);
                }
                else
                {
                    ApplicationData.Current.SignalDataChanged();

                    try
                    {
                        await currentInstance.RedirectActivationToAsync(AppInstance.GetCurrent().GetActivatedEventArgs());
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Redirect to main instance failed", e);
                    }
                    finally
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }
    }
}
