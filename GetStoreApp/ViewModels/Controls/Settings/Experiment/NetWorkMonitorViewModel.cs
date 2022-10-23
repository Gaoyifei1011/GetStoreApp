using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings.Experiment;
using GetStoreApp.Helpers;

namespace GetStoreApp.ViewModels.Controls.Settings.Experiment
{
    public class NetWorkMonitorViewModel : ObservableRecipient
    {
        private INetWorkMonitorService NetWorkMonitorService { get; } = IOCHelper.GetService<INetWorkMonitorService>();

        private bool _netWorkMonitorValue;

        public bool NetWorkMonitorValue
        {
            get { return _netWorkMonitorValue; }

            set { SetProperty(ref _netWorkMonitorValue, value); }
        }

        // 下载文件时“网络状态监控”开启设置
        public IRelayCommand NetWorkMonitorCommand => new RelayCommand<bool>(async (netWorkMonitorValue) =>
        {
            await NetWorkMonitorService.SetNetWorkMonitorValueAsync(netWorkMonitorValue);
            NetWorkMonitorValue = netWorkMonitorValue;
        });

        public NetWorkMonitorViewModel()
        {
            NetWorkMonitorValue = NetWorkMonitorService.NetWorkMonitorValue;
        }
    }
}
