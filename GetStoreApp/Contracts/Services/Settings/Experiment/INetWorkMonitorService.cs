using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings.Experiment
{
    public interface INetWorkMonitorService
    {
        bool NetWorkMonitorValue { get; set; }

        Task InitializeNetWorkMonitorValueAsync();

        Task SetNetWorkMonitorValueAsync(bool netWorkMonitorValue);

        Task ReturnDefaultValueAsync();
    }
}
