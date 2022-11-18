using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public sealed class NotificationViewModel : ViewModelBase
    {
        private bool _notification;

        public bool Notification
        {
            get { return _notification; }

            set
            {
                _notification = value;
                OnPropertyChanged();
            }
        }

        // 设置是否开启应用通知
        public IRelayCommand NotificationCommand => new RelayCommand<bool>(async (notification) =>
        {
            await NotificationService.SetNotificationAsync(notification);
            Notification = notification;
        });

        public NotificationViewModel()
        {
            Notification = NotificationService.AppNotification;
        }
    }
}
