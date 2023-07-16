using GetStoreApp.Services.Controls.Settings.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using Windows.System;
using WinRT;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：应用通知设置控件
    /// </summary>
    public sealed partial class NotificationControl : Grid, INotifyPropertyChanged
    {
        private bool _notification = NotificationService.AppNotification;

        public bool Notification
        {
            get { return _notification; }

            set
            {
                _notification = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notification)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public NotificationControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开系统通知设置
        /// </summary>
        public async void OnSettingsNotificationClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
        }

        /// <summary>
        /// 设置是否开启应用通知
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender.As<ToggleSwitch>();
            if (toggleSwitch is not null)
            {
                await NotificationService.SetNotificationAsync(toggleSwitch.IsOn);
                Notification = toggleSwitch.IsOn;
            }
        }
    }
}
