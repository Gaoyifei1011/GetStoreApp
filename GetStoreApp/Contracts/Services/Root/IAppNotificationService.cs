namespace GetStoreApp.Contracts.Services.Root
{
    public interface IAppNotificationService
    {
        void Initialize();

        void Show(params string[] notification);

        void Unregister();
    }
}
