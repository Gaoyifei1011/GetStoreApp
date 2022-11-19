using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
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
