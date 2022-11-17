using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.Settings.Experiment
{
    public interface INetWorkMonitorService
    {
        bool NetWorkMonitorValue { get; set; }

        Task InitializeNetWorkMonitorValueAsync();

        Task SetNetWorkMonitorValueAsync(bool netWorkMonitorValue);

        Task RestoreDefaultValueAsync();
    }
}
