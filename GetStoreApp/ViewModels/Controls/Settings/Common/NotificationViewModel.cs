using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Helpers.Root;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public class NotificationViewModel : ObservableRecipient
    {
        private INotificationService NotificationService { get; } = ContainerHelper.GetInstance<INotificationService>();

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
