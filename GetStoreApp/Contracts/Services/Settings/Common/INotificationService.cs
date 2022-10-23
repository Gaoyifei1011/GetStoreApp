using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings.Common
{
    public interface INotificationService
    {
        bool AppNotification { get; set; }

        Task InitializeNotificationAsync();

        Task SetNotificationAsync(bool appNotification);
    }
}
