using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.Rstrtmgr;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
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
using Windows.UI.ViewManagement;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置高级选项页面
    /// </summary>
    internal sealed partial class SettingsAdvancedPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private bool isInitialized;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private bool _notification;

        private bool Notification
        {
            get { return _notification; }

            set
            {
                if (!Equals(_notification, value))
                {
                    _notification = value;
                    PropertyChanged?.Invoke(this, new(nameof(Notification)));
                }
            }
        }

        private bool _notificationEnabled;

        private bool NotificationEnabled
        {
            get { return _notificationEnabled; }

            set
            {
                if (!Equals(_notificationEnabled, value))
                {
                    _notificationEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(NotificationEnabled)));
                }
            }
        }

        private bool _isRestarting = false;

        private bool IsRestarting
        {
            get { return _isRestarting; }

            set
            {
                if (!Equals(_isRestarting, value))
                {
                    _isRestarting = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsRestarting)));
                }
            }
        }

        private bool _shellMenu;

        private bool ShellMenu
        {
            get { return _shellMenu; }

            set
            {
                if (!Equals(_shellMenu, value))
                {
                    _shellMenu = value;
                    PropertyChanged?.Invoke(this, new(nameof(ShellMenu)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第二部分：构造函数

        internal SettingsAdvancedPage()
        {
            InitializeComponent();
        }

        #endregion 第二部分：构造函数

        #region 第三部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面后触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            Notification = NotificationService.AppNotification;
            NotificationEnabled = NotificationService.NotificationSetting is NotificationSetting.Enabled;
            ShellMenu = ShellMenuService.ShellMenu;

            if (!isInitialized)
            {
                isInitialized = true;
                MountSettingsEvent();
            }
        }

        #endregion 第三部分：父类虚方法重写

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 打开系统通知设置
        /// </summary>
        private void OnSystemNotificationSettingsClicked(object sender, RoutedEventArgs args)
        {
            OpenSystemNotificationSettings();
        }

        /// <summary>
        /// 重新启动资源管理器
        /// </summary>
        private async void OnRestartExplorerClicked(object sender, RoutedEventArgs args)
        {
            IsRestarting = true;
            await RestartExplorerAsync();
            IsRestarting = false;
        }

        /// <summary>
        /// 是否开启显示文件右键菜单
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnShellMenuToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(ShellMenu, toggleSwitch.IsOn))
            {
                ShellMenu = toggleSwitch.IsOn;
                ShellMenuService.SetShellMenu(toggleSwitch.IsOn);
                ShellMenu = ShellMenuService.ShellMenu;
            }
        }

        /// <summary>
        /// 设置是否开启应用通知
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnNotificationToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(Notification, toggleSwitch.IsOn))
            {
                Notification = toggleSwitch.IsOn;
                NotificationService.SetNotification(toggleSwitch.IsOn);
                Notification = NotificationService.AppNotification;
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
            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.LogClean, result));
        }

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            DismountSettingsEvent();
        }

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (string.Equals(args.PropertyName, nameof(NotificationService.NotificationSetting)))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    NotificationEnabled = NotificationService.NotificationSetting is NotificationSetting.Enabled;
                });
            }
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 挂载设置事件
        /// </summary>
        private void MountSettingsEvent()
        {
            NotificationService.PropertyChanged += OnServicePropertyChanged;
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
        }

        /// <summary>
        /// 卸载设置事件
        /// </summary>
        private void DismountSettingsEvent()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                NotificationService.PropertyChanged -= OnServicePropertyChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAdvancedPage), nameof(DismountSettingsEvent), 1, e);
            }
        }

        /// <summary>
        /// 打开系统通知设置
        /// </summary>
        private void OpenSystemNotificationSettings()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-settings:notifications"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 重启资源管理器
        /// </summary>
        private async Task RestartExplorerAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    int dwRmStatus = RstrtmgrLibrary.RmStartSession(out uint dwSessionHandle, 0, Convert.ToString(GuidHelper.Empty));

                    if (dwRmStatus is 0)
                    {
                        List<uint> processPIDList = ProcessHelper.GetProcessPIDByName("explorer.exe");
                        RM_UNIQUE_PROCESS[] lpRmProcList = new RM_UNIQUE_PROCESS[processPIDList.Count];

                        for (int index = 0; index < processPIDList.Count; index++)
                        {
                            lpRmProcList[index].dwProcessId = (int)processPIDList[index];
                            nint hProcess = Kernel32Library.OpenProcess(EDesiredAccess.PROCESS_QUERY_LIMITED_INFORMATION, false, (int)processPIDList[index]);
                            lpRmProcList[index].ProcessStartTime = hProcess != nint.Zero && Kernel32Library.GetProcessTimes(hProcess, out FILETIME creationTime, out FILETIME exitTime, out FILETIME kernelTime, out FILETIME userTime) ? creationTime : new();
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAdvancedPage), nameof(RestartExplorerAsync), 1, e);
                }
            });
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
