using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
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
        private static IActivatedEventArgs ActivatedEventArgs;

        // 0 表示不需要发送消息，1表示发送的是应用启动时重定向的消息
        private static int SendMessageDataType;

        // 应用启动时使用的参数
        public static Dictionary<string, object> LaunchArgs { get; set; } = new Dictionary<string, object>()
        {
            {"TypeName",-1 },
            {"ChannelName",-1 },
            {"Link",null},
        };

        /// <summary>
        /// 处理桌面应用启动的方式
        /// </summary>
        public static async Task InitializeLaunchAsync()
        {
            ActivatedEventArgs = AppInstance.GetActivatedEventArgs();
            await ParseStartupKindAsync(ActivatedEventArgs.Kind);
            await DealLaunchArgsAsync();
        }

        /// <summary>
        /// 解析启动命令方式
        /// </summary>
        private static async Task ParseStartupKindAsync(ActivationKind kind)
        {
            switch (kind)
            {
                // 正常方式启动（包括命令行）
                case ActivationKind.Launch:
                    {
                        ParseLaunchArgs();
                        break;
                    }
                // 使用共享目标方式启动
                case ActivationKind.ShareTarget:
                    {
                        SendMessageDataType = 1;
                        ShareOperation shareOperation = (ActivatedEventArgs as ShareTargetActivatedEventArgs).ShareOperation;
                        shareOperation.ReportCompleted();
                        LaunchArgs["Link"] = Convert.ToString(await shareOperation.Data.GetUriAsync());
                        break;
                    }
                // 从系统通知处启动
                case ActivationKind.ToastNotification:
                    {
                        SendMessageDataType = 2;
                        break;
                    }
                // 其他方式
                default:
                    {
                        ParseLaunchArgs();
                        break;
                    }
            }
        }

        /// <summary>
        /// 解析启动命令参数
        /// </summary>
        private static void ParseLaunchArgs()
        {
            if (Program.CommandLineArgs.Count is 0)
            {
                SendMessageDataType = 0;
                return;
            }
            else if (Program.CommandLineArgs.Count is 1)
            {
                SendMessageDataType = 1;
                LaunchArgs["Link"] = Program.CommandLineArgs[0];
            }
            else
            {
                SendMessageDataType = 1;

                // 跳转列表启动的参数
                if (Program.CommandLineArgs[0] is "JumpList")
                {
                    LaunchArgs["TypeName"] = ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(Program.CommandLineArgs[1], StringComparison.OrdinalIgnoreCase));
                    LaunchArgs["ChannelName"] = ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(Program.CommandLineArgs[2], StringComparison.OrdinalIgnoreCase));
                    LaunchArgs["Link"] = Program.CommandLineArgs[3];
                }

                // 正常启动的参数
                else
                {
                    if (Program.CommandLineArgs.Count % 2 is not 0) return;

                    int TypeNameIndex = Program.CommandLineArgs.FindIndex(item => item.Equals("-t", StringComparison.OrdinalIgnoreCase) || item.Equals("--type", StringComparison.OrdinalIgnoreCase));
                    int ChannelNameIndex = Program.CommandLineArgs.FindIndex(item => item.Equals("-c", StringComparison.OrdinalIgnoreCase) || item.Equals("--channel", StringComparison.OrdinalIgnoreCase));
                    int LinkIndex = Program.CommandLineArgs.FindIndex(item => item.Equals("-l", StringComparison.OrdinalIgnoreCase) || item.Equals("--link", StringComparison.OrdinalIgnoreCase));

                    LaunchArgs["TypeName"] = TypeNameIndex is -1 ? LaunchArgs["TypeName"] : ResourceService.TypeList.FindIndex(item => item.ShortName.Equals(Program.CommandLineArgs[TypeNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                    LaunchArgs["ChannelName"] = ChannelNameIndex is -1 ? LaunchArgs["ChannelName"] : ResourceService.ChannelList.FindIndex(item => item.ShortName.Equals(Program.CommandLineArgs[ChannelNameIndex + 1], StringComparison.OrdinalIgnoreCase));
                    LaunchArgs["Link"] = LinkIndex is -1 ? LaunchArgs["Link"] : Program.CommandLineArgs[LinkIndex + 1];
                }
            }
        }

        /// <summary>
        /// 处理应用启动参数
        /// </summary>
        private static async Task DealLaunchArgsAsync()
        {
            if (SendMessageDataType == 0 || SendMessageDataType == 1)
            {
                bool isExisted = false;

                string sendData = string.Format("{0} {1} {2}", LaunchArgs["TypeName"], LaunchArgs["ChannelName"], LaunchArgs["Link"] is null ? "PlaceHolderText" : LaunchArgs["Link"]);

                // 向主实例发送数据
                COPYDATASTRUCT copyDataStruct = new COPYDATASTRUCT();
                copyDataStruct.dwData = SendMessageDataType;
                copyDataStruct.cbData = Encoding.Default.GetBytes(sendData).Length + 1;
                copyDataStruct.lpData = sendData;

                List<IntPtr> hwndList = FindExistedWindowHandle("GetStoreApp.exe");

                foreach (IntPtr hwnd in hwndList)
                {
                    isExisted = true;
                    IntPtr ptrCopyDataStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(COPYDATASTRUCT)));
                    Marshal.StructureToPtr(copyDataStruct, ptrCopyDataStruct, false);
                    User32Library.SendMessage(hwnd, WindowMessage.WM_COPYDATA, 0, ptrCopyDataStruct);
                    Marshal.FreeHGlobal(ptrCopyDataStruct);
                    User32Library.SetForegroundWindow(hwnd);
                }

                // 然后退出实例并停止
                if (isExisted)
                {
                    Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
                }
            }
            else if (SendMessageDataType == 2)
            {
                bool isExisted = false;
                string sendData = Program.CommandLineArgs[0];

                // 向主实例发送数据
                COPYDATASTRUCT copyDataStruct = new COPYDATASTRUCT();
                copyDataStruct.dwData = SendMessageDataType;
                copyDataStruct.cbData = Encoding.Default.GetBytes(sendData).Length + 1;
                copyDataStruct.lpData = sendData;

                List<IntPtr> hwndList = FindExistedWindowHandle("GetStoreApp.exe");

                foreach (IntPtr hwnd in hwndList)
                {
                    isExisted = true;
                    IntPtr ptrCopyDataStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(COPYDATASTRUCT)));
                    Marshal.StructureToPtr(copyDataStruct, ptrCopyDataStruct, false);
                    User32Library.SendMessage(hwnd, WindowMessage.WM_COPYDATA, 0, ptrCopyDataStruct);
                    Marshal.FreeHGlobal(ptrCopyDataStruct);
                    User32Library.SetForegroundWindow(hwnd);
                }

                // 然后退出实例并停止
                if (isExisted)
                {
                    Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
                }
                else
                {
                    await AppNotificationService.HandleAppNotificationAsync(Program.CommandLineArgs[0]);
                }
            }
        }

        /// <summary>
        /// 寻找当前进程的所有窗口句柄
        /// </summary>
        private static List<IntPtr> FindExistedWindowHandle(string processName)
        {
            List<IntPtr> hwndList = new List<IntPtr>();
            List<uint> GetStoreAppProcessPIDList = ProcessHelper.GetProcessPIDByName(processName);

            if (GetStoreAppProcessPIDList.Count > 0)
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
                            foreach (uint ProcessID in GetStoreAppProcessPIDList)
                            {
                                if (ProcessID == processId)
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
