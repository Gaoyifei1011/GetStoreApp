using System.Collections.Specialized;

namespace GetStoreApp.Contracts.Services.App
{
    public interface IAppNotificationService
    {
        void Initialize();

        bool Show(string payload);

        NameValueCollection ParseArguments(string arguments);

        void Unregister();
    }
}
