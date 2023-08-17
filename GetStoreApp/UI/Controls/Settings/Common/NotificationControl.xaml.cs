using GetStoreApp.Services.Controls.Settings.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;

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
                OnPropertyChanged();
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
        public async void OnSystemNotificationSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
        }

        /// <summary>
        /// 设置是否开启应用通知
        /// </summary>
        public void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                NotificationService.SetNotification(toggleSwitch.IsOn);
                Notification = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
