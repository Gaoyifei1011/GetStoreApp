namespace GetStoreApp.Contracts.Services.Root
{
    public interface IAppNotificationService
    {
        void Initialize();

        void Show(string notificationKey, params string[] notificationContent);

        void Unregister();
    }
}
