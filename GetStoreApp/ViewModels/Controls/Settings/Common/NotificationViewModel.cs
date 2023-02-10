using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：应用通知设置用户控件视图模型
    /// </summary>
    public sealed class NotificationViewModel : ViewModelBase
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

        public IRelayCommand SettingsNotificationCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
        });

        /// <summary>
        /// 设置是否开启应用通知
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await NotificationService.SetNotificationAsync(toggleSwitch.IsOn);
                Notification = toggleSwitch.IsOn;
            }
        }
    }
}
