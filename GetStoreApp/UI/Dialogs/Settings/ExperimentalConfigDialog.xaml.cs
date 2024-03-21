using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// 实验性功能配置对话框
    /// </summary>
    public sealed partial class ExperimentalConfigDialog : ContentDialog, INotifyPropertyChanged
    {
        private int countDown = 0;

        private DispatcherTimer displayTimer = new DispatcherTimer();

        private bool _isMessageVisable = false;

        public bool IsMessageVisable
        {
            get { return _isMessageVisable; }

            set
            {
                if (!Equals(_isMessageVisable, value))
                {
                    _isMessageVisable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMessageVisable)));
                }
            }
        }

        private bool _netWorkMonitorValue = NetWorkMonitorService.NetWorkMonitorValue;

        public bool NetWorkMonitorValue
        {
            get { return _netWorkMonitorValue; }

            set
            {
                if (!Equals(_netWorkMonitorValue, value))
                {
                    _netWorkMonitorValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NetWorkMonitorValue)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ExperimentalConfigDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭对话框时关闭计时器
        /// </summary>
        private void OnClosed(object sender, ContentDialogClosedEventArgs args)
        {
            displayTimer.Tick -= DisplayTimerTick;
            if (displayTimer.IsEnabled)
            {
                displayTimer.Stop();
            }
        }

        /// <summary>
        /// 打开对话框时初始化计时器
        /// </summary>
        private void OnOpened(object sender, ContentDialogOpenedEventArgs args)
        {
            displayTimer.Tick += DisplayTimerTick;
            displayTimer.Interval = new TimeSpan(0, 0, 1);
        }

        /// <summary>
        /// 下载文件时“网络状态监控”开启设置
        /// </summary>
        private void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                NetWorkMonitorService.SetNetWorkMonitorValue(toggleSwitch.IsOn);
                NetWorkMonitorValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 打开配置文件目录
        /// </summary>
        private void OnOpenConfigFileClicked(object sender, RoutedEventArgs args)
        {
            if (Aria2Service.Aria2ConfPath is not null)
            {
                Task.Run(async () =>
                {
                    string filePath = Aria2Service.Aria2ConfPath.Replace(@"\\", @"\");

                    // 定位文件，若定位失败，则仅启动资源管理器并打开桌面目录
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        try
                        {
                            StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                            StorageFolder folder = await file.GetParentAsync();
                            FolderLauncherOptions options = new FolderLauncherOptions();
                            options.ItemsToSelect.Add(file);
                            await Launcher.LaunchFolderAsync(folder, options);
                        }
                        catch (Exception)
                        {
                            await Launcher.LaunchFolderPathAsync(InfoHelper.UserDataPath.Desktop);
                        }
                    }
                    else
                    {
                        await Launcher.LaunchFolderPathAsync(InfoHelper.UserDataPath.Desktop);
                    }
                });
            }
        }

        /// <summary>
        /// 还原默认值
        /// </summary>
        private async void OnRestoreDefaultClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            if (sender.Tag is not null)
            {
                NetWorkMonitorValue = true;
                await Aria2Service.RestoreDefaultAsync();
                NetWorkMonitorService.RestoreDefaultValue();

                if (!displayTimer.IsEnabled)
                {
                    displayTimer.Start();
                    IsMessageVisable = true;
                }
                countDown = 3;
            }
        }

        private void DisplayTimerTick(object sender, object args)
        {
            if (countDown > 0)
            {
                countDown--;
            }
            else
            {
                IsMessageVisable = false;
                displayTimer.Stop();
            }
        }
    }
}
