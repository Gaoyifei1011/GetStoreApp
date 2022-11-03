using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Experiment;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;

namespace GetStoreApp.ViewModels.Controls.Settings.Experiment
{
    public class NetWorkMonitorViewModel : ObservableRecipient
    {
        private INetWorkMonitorService NetWorkMonitorService { get; } = ContainerHelper.GetInstance<INetWorkMonitorService>();

        private bool _netWorkMonitorValue;

        public bool NetWorkMonitorValue
        {
            get { return _netWorkMonitorValue; }

            set { SetProperty(ref _netWorkMonitorValue, value); }
        }

        // 控件被卸载时，关闭消息服务
        public IRelayCommand UnloadedCommand => new RelayCommand(() =>
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        });

        // 下载文件时“网络状态监控”开启设置
        public IRelayCommand NetWorkMonitorCommand => new RelayCommand<bool>(async (netWorkMonitorValue) =>
        {
            await NetWorkMonitorService.SetNetWorkMonitorValueAsync(netWorkMonitorValue);
            NetWorkMonitorValue = netWorkMonitorValue;
        });

        public NetWorkMonitorViewModel()
        {
            NetWorkMonitorValue = NetWorkMonitorService.NetWorkMonitorValue;

            WeakReferenceMessenger.Default.Register<NetWorkMonitorViewModel, RestoreDefaultMessage>(this, (netWorkMonitorViewModel, restoreDefaultMessage) =>
            {
                if (restoreDefaultMessage.Value)
                {
                    NetWorkMonitorValue = NetWorkMonitorService.NetWorkMonitorValue;
                }
            });
        }
    }
}
