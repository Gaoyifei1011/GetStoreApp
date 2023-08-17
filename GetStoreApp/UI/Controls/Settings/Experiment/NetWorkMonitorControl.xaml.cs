using GetStoreApp.Services.Controls.Settings.Experiment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.UI.Controls.Settings.Experiment
{
    /// <summary>
    /// 实验性功能：网络监控状态设置控件
    /// </summary>
    public sealed partial class NetWorkMonitorControl : Grid, INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        public NetWorkMonitorControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 下载文件时“网络状态监控”开启设置
        /// </summary>
        public void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                NetWorkMonitorService.SetNetWorkMonitorValue(toggleSwitch.IsOn);
                NetWorkMonitorValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
