using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        private DispatcherTimer DisplayTimer = new DispatcherTimer();

        private int CountDown = 0;

        private bool _isMessageVisable = false;

        public bool IsMessageVisable
        {
            get { return _isMessageVisable; }

            set
            {
                _isMessageVisable = value;
                OnPropertyChanged();
            }
        }

        private bool _netWorkMonitorValue = NetWorkMonitorService.NetWorkMonitorValue;

        public bool NetWorkMonitorValue
        {
            get { return _netWorkMonitorValue; }

            set
            {
                _netWorkMonitorValue = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ExperimentalConfigDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开对话框时初始化计时器
        /// </summary>
        public void OnOpened(object sender, ContentDialogOpenedEventArgs args)
        {
            DisplayTimer.Tick += DisplayTimerTick;
            DisplayTimer.Interval = new TimeSpan(0, 0, 1);
        }

        /// <summary>
        /// 下载文件时“网络状态监控”开启设置
        /// </summary>
        public void OnToggled(object sender, RoutedEventArgs args)
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
        public void OnOpenConfigFileClicked(object sender, RoutedEventArgs args)
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
        public void OnRestoreDefaultClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            if (sender.Tag is not null)
            {
                NetWorkMonitorValue = true;
                Aria2Service.RestoreDefault();
                NetWorkMonitorService.RestoreDefaultValue();

                if (!DisplayTimer.IsEnabled)
                {
                    DisplayTimer.Start();
                    IsMessageVisable = true;
                }
                CountDown = 3;
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DisplayTimerTick(object sender, object args)
        {
            if (CountDown > 0)
            {
                CountDown--;
            }
            else
            {
                IsMessageVisable = false;
                DisplayTimer.Stop();
            }
        }
    }
}
