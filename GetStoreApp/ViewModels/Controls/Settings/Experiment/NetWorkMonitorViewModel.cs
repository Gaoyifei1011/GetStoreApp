using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.ViewModels.Controls.Settings.Experiment
{
    /// <summary>
    /// 实验性功能：网络监控状态设置用户控件视图模型
    /// </summary>
    public sealed class NetWorkMonitorViewModel : ViewModelBase
    {
        private bool _netWorkMonitorValue = NetWorkMonitorService.NetWorkMonitorValue;

        public bool NetWorkMonitorValue
        {
            get { return _netWorkMonitorValue; }

            set
            {
                _netWorkMonitorValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 控件被卸载时，关闭消息服务
        /// </summary>
        public void OnUnloaded(object sender, RoutedEventArgs args)
        {
            Messenger.Default.Unregister(this);
        }

        /// <summary>
        /// 下载文件时“网络状态监控”开启设置
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await NetWorkMonitorService.SetNetWorkMonitorValueAsync(toggleSwitch.IsOn);
                NetWorkMonitorValue = toggleSwitch.IsOn;
            }
        }

        public NetWorkMonitorViewModel()
        {
            Messenger.Default.Register<bool>(this, MessageToken.RestoreDefault, (restoreDefaultMessage) =>
            {
                if (restoreDefaultMessage)
                {
                    NetWorkMonitorValue = NetWorkMonitorService.NetWorkMonitorValue;
                }
            });
        }
    }
}
