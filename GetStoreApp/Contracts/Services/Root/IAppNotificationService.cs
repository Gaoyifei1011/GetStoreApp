using Microsoft.Windows.AppNotifications;

namespace GetStoreApp.Contracts.Services.Root
{
    public interface IAppNotificationService
    {
        void Initialize();

        void HandleAppNotification(AppNotificationActivatedEventArgs args);

        void Show(string notificationKey, params string[] notificationContent);

        void Unregister();
    }
}
