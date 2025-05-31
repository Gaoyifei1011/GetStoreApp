using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.Rstrtmgr;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.System;
using Windows.UI.Notifications;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置高级选项页面
    /// </summary>
    public sealed partial class SettingsAdvancedPage : Page, INotifyPropertyChanged
    {
        private readonly string WebKernelWebViewString = ResourceService.GetLocalized("SettingsAdvanced/WebKernelWebView");
        private readonly string WebKernelWebView2String = ResourceService.GetLocalized("SettingsAdvanced/WebKernelWebView2");

        private bool _notificationValue = NotificationService.AppNotification;

        public bool NotificationValue
        {
            get { return _notificationValue; }

            set
            {
                if (!Equals(_notificationValue, value))
                {
                    _notificationValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NotificationValue)));
                }
            }
        }

        private bool _notificationEnabled = NotificationService.NotificationSetting is NotificationSetting.Enabled;

        public bool NotificationEnabled
        {
            get { return _notificationEnabled; }

            private set
            {
                if (!Equals(_notificationEnabled, value))
                {
                    _notificationEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NotificationEnabled)));
                }
            }
        }

        private KeyValuePair<string, string> _webKernel;

        public KeyValuePair<string, string> WebKernel
        {
            get { return _webKernel; }

            set
            {
                if (!Equals(_webKernel, value))
                {
                    _webKernel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WebKernel)));
                }
            }
        }

        private bool _isRestarting = false;

        public bool IsRestarting
        {
            get { return _isRestarting; }

            set
            {
                if (!Equals(_isRestarting, value))
                {
                    _isRestarting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRestarting)));
                }
            }
        }

        private bool _shellMenuValue = ShellMenuService.ShellMenuValue;

        public bool ShellMenuValue
        {
            get { return _shellMenuValue; }

            set
            {
                if (!Equals(_shellMenuValue, value))
                {
                    _shellMenuValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShellMenuValue)));
                }
            }
        }

        private List<KeyValuePair<string, string>> WebKernelList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsAdvancedPage()
        {
            InitializeComponent();
            WebKernelList.Add(KeyValuePair.Create(WebKernelService.WebKernelList[0], WebKernelWebViewString));
            WebKernelList.Add(KeyValuePair.Create(WebKernelService.WebKernelList[1], WebKernelWebView2String));
            WebKernel = WebKernelList.Find(item => string.Equals(item.Key, WebKernelService.WebKernel, StringComparison.OrdinalIgnoreCase));
            NotificationService.PropertyChanged += OnServicePropertyChanged;
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
        }

        #region 第一部分：设置高级页面——挂载的事件

        /// <summary>
        /// 打开系统通知设置
        /// </summary>
        private void OnSystemNotificationSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 选择网页浏览器渲染网页使用的内核
        /// </summary>
        private void OnWebKernelSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                WebKernel = WebKernelList[Convert.ToInt32(tag)];
                WebKernelService.SetWebKernel(WebKernel.Key);
            }
        }

        /// <summary>
        /// 重新启动资源管理器
        /// </summary>
        private async void OnRestartExplorerClicked(object sender, RoutedEventArgs args)
        {
            IsRestarting = true;

            await Task.Run(() =>
            {
                try
                {
                    int dwRmStatus = RstrtmgrLibrary.RmStartSession(out uint dwSessionHandle, 0, GuidHelper.Empty.ToString());

                    if (dwRmStatus is 0)
                    {
                        List<uint> processPIDList = ProcessHelper.GetProcessPIDByName("explorer.exe");
                        RM_UNIQUE_PROCESS[] lpRmProcList = new RM_UNIQUE_PROCESS[processPIDList.Count];

                        for (int index = 0; index < processPIDList.Count; index++)
                        {
                            lpRmProcList[index].dwProcessId = (int)processPIDList[index];
                            IntPtr hProcess = Kernel32Library.OpenProcess(EDesiredAccess.PROCESS_QUERY_LIMITED_INFORMATION, false, (int)processPIDList[index]);
                            lpRmProcList[index].ProcessStartTime = hProcess != IntPtr.Zero && Kernel32Library.GetProcessTimes(hProcess, out FILETIME creationTime, out FILETIME exitTime, out FILETIME kernelTime, out FILETIME userTime) ? creationTime : new();
                        }

                        dwRmStatus = RstrtmgrLibrary.RmRegisterResources(dwSessionHandle, 0, null, (uint)processPIDList.Count, lpRmProcList, 0, null);

                        if (dwRmStatus is 0)
                        {
                            dwRmStatus = RstrtmgrLibrary.RmShutdown(dwSessionHandle, RM_SHUTDOWN_TYPE.RmForceShutdown, null);

                            if (dwRmStatus is 0)
                            {
                                dwRmStatus = RstrtmgrLibrary.RmRestart(dwSessionHandle, 0, null);

                                if (dwRmStatus is 0)
                                {
                                    dwRmStatus = RstrtmgrLibrary.RmEndSession(dwSessionHandle);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAdvancedPage), nameof(OnRestartExplorerClicked), 1, e);
                }
            });

            IsRestarting = false;
        }

        /// <summary>
        /// 是否开启显示文件右键菜单
        /// </summary>
        private void OnShellMenuToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                ShellMenuService.SetShellMenuValue(toggleSwitch.IsOn);
                ShellMenuValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 设置是否开启应用通知
        /// </summary>
        private void OnNotificationToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                NotificationService.SetNotification(toggleSwitch.IsOn);
                NotificationValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 清理应用内使用的所有痕迹
        /// </summary>
        private async void OnTraceCleanupClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new TraceCleanupPromptDialog());
        }

        /// <summary>
        /// 打开日志文件夹
        /// </summary>
        private async void OnOpenLogFolderClicked(object sender, RoutedEventArgs args)
        {
            await LogService.OpenLogFolderAsync();
        }

        /// <summary>
        /// 清除所有日志记录
        /// </summary>
        private async void OnClearClicked(object sender, RoutedEventArgs args)
        {
            bool result = await LogService.ClearLogAsync();
            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.LogClean, result));
        }

        #endregion 第一部分：设置高级页面——挂载的事件

        #region 第二部分：设置高级页面——自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                NotificationService.PropertyChanged -= OnServicePropertyChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAdvancedPage), nameof(OnApplicationExit), 1, e);
            }
        }

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (Equals(args.PropertyName, nameof(NotificationService.NotificationSetting)))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    NotificationEnabled = NotificationService.NotificationSetting is NotificationSetting.Enabled;
                });
            }
        }

        #endregion 第二部分：设置高级页面——自定义事件
    }
}
