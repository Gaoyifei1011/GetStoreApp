using GetStoreApp.Helpers.Root;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer.ShareTarget;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 桌面应用启动服务
    /// </summary>
    public static class DesktopLaunchService
    {
        private static IActivatedEventArgs activatedEventArgs;
        private static ActivationKind activationKind = ActivationKind.Launch;
        private static List<string> desktopLaunchArgs = new List<string>();

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
            activatedEventArgs = AppInstance.GetActivatedEventArgs();
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
                            case "UWPApp":
                                {
                                    InitializePage = typeof(UWPAppPage);
                                    break;
                                }
                            case "Download":
                                {
                                    InitializePage = typeof(DownloadPage);
                                    break;
                                }
                            case "Web":
                                {
                                    ProcessHelper.StartProcess(Path.Combine(InfoHelper.AppInstalledLocation, "GetStoreAppWebView.exe"), " ", out _);
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
            // 正常启动
            if (activationKind is ActivationKind.Launch)
            {
                bool isExisted = false;

                string sendData = string.Format("{0} {1} {2}", LaunchArgs["TypeName"], LaunchArgs["ChannelName"], LaunchArgs["Link"] is null ? "PlaceHolderText" : LaunchArgs["Link"]);

                // 向主实例发送数据
                COPYDATASTRUCT copyDataStruct = new COPYDATASTRUCT();
                copyDataStruct.dwData = (IntPtr)activationKind;
                copyDataStruct.cbData = Encoding.Default.GetBytes(sendData).Length + 1;
                copyDataStruct.lpData = sendData;

                List<IntPtr> hwndList = FindExistedWindowHandle("GetStoreApp.exe");

                foreach (IntPtr hwnd in hwndList)
                {
                    isExisted = true;
                    IntPtr ptrCopyDataStruct = Marshal.AllocHGlobal(Marshal.SizeOf<COPYDATASTRUCT>());
                    Marshal.StructureToPtr(copyDataStruct, ptrCopyDataStruct, false);
                    User32Library.SendMessage(hwnd, WindowMessage.WM_COPYDATA, 0, ptrCopyDataStruct);
                    Marshal.FreeHGlobal(ptrCopyDataStruct);
                }

                // 然后退出实例并停止
                Program.IsNeedAppLaunch = !isExisted;
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
                    bool isExisted = false;

                    string sendData;
                    // 跳转列表启动
                    if (desktopLaunchArgs.Count is 2 && desktopLaunchArgs[0] is "JumpList")
                    {
                        sendData = string.Join(" ", desktopLaunchArgs);
                    }
                    // 辅助磁贴启动
                    else if (desktopLaunchArgs.Count is 4 && desktopLaunchArgs[0] is "SecondaryTile")
                    {
                        sendData = string.Join(' ', desktopLaunchArgs[0], desktopLaunchArgs[1]);
                    }
                    else
                    {
                        sendData = string.Format("{0} {1} {2}", LaunchArgs["TypeName"], LaunchArgs["ChannelName"], LaunchArgs["Link"] is null ? "PlaceHolderText" : LaunchArgs["Link"]);
                    }

                    // 向主实例发送数据
                    COPYDATASTRUCT copyDataStruct = new COPYDATASTRUCT();
                    copyDataStruct.dwData = (IntPtr)activationKind;
                    copyDataStruct.cbData = Encoding.Default.GetBytes(sendData).Length + 1;
                    copyDataStruct.lpData = sendData;

                    List<IntPtr> hwndList = FindExistedWindowHandle("GetStoreApp.exe");

                    foreach (IntPtr hwnd in hwndList)
                    {
                        isExisted = true;
                        IntPtr ptrCopyDataStruct = Marshal.AllocHGlobal(Marshal.SizeOf<COPYDATASTRUCT>());
                        Marshal.StructureToPtr(copyDataStruct, ptrCopyDataStruct, false);
                        User32Library.SendMessage(hwnd, WindowMessage.WM_COPYDATA, 0, ptrCopyDataStruct);
                        Marshal.FreeHGlobal(ptrCopyDataStruct);
                    }

                    // 然后退出实例并停止
                    Program.IsNeedAppLaunch = !isExisted;
                }
            }
            // 应用通知启动
            else if (activationKind is ActivationKind.ToastNotification)
            {
                bool isExisted = false;
                string sendData = desktopLaunchArgs[0];

                // 向主实例发送数据
                COPYDATASTRUCT copyDataStruct = new COPYDATASTRUCT();
                copyDataStruct.dwData = (IntPtr)activationKind;
                copyDataStruct.cbData = Encoding.Default.GetBytes(sendData).Length + 1;
                copyDataStruct.lpData = sendData;

                List<IntPtr> hwndList = FindExistedWindowHandle("GetStoreApp.exe");

                foreach (IntPtr hwnd in hwndList)
                {
                    isExisted = true;
                    IntPtr ptrCopyDataStruct = Marshal.AllocHGlobal(Marshal.SizeOf<COPYDATASTRUCT>());
                    Marshal.StructureToPtr(copyDataStruct, ptrCopyDataStruct, false);
                    User32Library.SendMessage(hwnd, WindowMessage.WM_COPYDATA, 0, ptrCopyDataStruct);
                    Marshal.FreeHGlobal(ptrCopyDataStruct);
                }

                // 然后退出实例并停止
                if (isExisted)
                {
                    Program.IsNeedAppLaunch = !isExisted;
                    return;
                }
                else
                {
                    await ToastNotificationService.HandleToastNotificationAsync(desktopLaunchArgs[0]);
                }
            }
        }

        /// <summary>
        /// 寻找当前进程的所有窗口句柄
        /// </summary>
        private static List<IntPtr> FindExistedWindowHandle(string processName)
        {
            List<IntPtr> hwndList = new List<IntPtr>();
            List<uint> processPidList = ProcessHelper.GetProcessPidByName(processName);

            if (processPidList.Count > 0)
            {
                IntPtr hwnd = IntPtr.Zero;
                do
                {
                    hwnd = User32Library.FindWindowEx(IntPtr.Zero, hwnd, "WinUIDesktopWin32WindowClass", null);

                    if (hwnd != IntPtr.Zero)
                    {
                        User32Library.GetWindowThreadProcessId(hwnd, out uint processId);

                        if (processId is not 0)
                        {
                            foreach (uint processPid in processPidList)
                            {
                                if (processPid.Equals(processId))
                                {
                                    hwndList.Add(hwnd);
                                }
                            }
                        }
                    }
                }
                while (hwnd != IntPtr.Zero);
            }

            return hwndList;
        }
    }
}
