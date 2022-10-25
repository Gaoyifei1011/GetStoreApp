using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Controls.Settings.Experiment
{
    public interface INetWorkMonitorService
    {
        bool NetWorkMonitorValue { get; set; }

        Task InitializeNetWorkMonitorValueAsync();

        Task SetNetWorkMonitorValueAsync(bool netWorkMonitorValue);

        Task RestoreDefaultValueAsync();
    }
}
