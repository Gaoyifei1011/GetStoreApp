using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class NotificationViewModel : ObservableRecipient
    {
        private INotificationService NotificationService { get; } = IOCHelper.GetService<INotificationService>();

        private bool _notification;

        public bool Notification
        {
            get { return _notification; }

            set { SetProperty(ref _notification, value); }
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
