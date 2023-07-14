using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    /// <summary>
    /// 设置页面：窗口置顶设置控件
    /// </summary>
    public sealed partial class TopMostControl : Grid, INotifyPropertyChanged
    {
        private bool _topMostValue = TopMostService.TopMostValue;

        public bool TopMostValue
        {
            get { return _topMostValue; }

            set
            {
                _topMostValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TopMostValue)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TopMostControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await TopMostService.SetTopMostValueAsync(toggleSwitch.IsOn);
                TopMostService.SetAppTopMost();
                TopMostValue = toggleSwitch.IsOn;
            }
        }
    }
}
