using GetStoreApp.Services.Controls.Settings.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：链接过滤设置控件
    /// </summary>
    public sealed partial class LinkFilterControl : Expander, INotifyPropertyChanged
    {
        private bool _startsWithEFilterValue = LinkFilterService.StartWithEFilterValue;

        public bool StartsWithEFilterValue
        {
            get { return _startsWithEFilterValue; }

            set
            {
                _startsWithEFilterValue = value;
                OnPropertyChanged();
            }
        }

        private bool _blockMapFilterValue = LinkFilterService.BlockMapFilterValue;

        public bool BlockMapFilterValue
        {
            get { return _blockMapFilterValue; }

            set
            {
                _blockMapFilterValue = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LinkFilterControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置是否过滤以“.e”开头的文件
        /// </summary>
        public void OnStartWithEToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                LinkFilterService.SetStartsWithEFilterValue(toggleSwitch.IsOn);
                StartsWithEFilterValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 设置是否过滤包块映射文件
        /// </summary>
        public void OnBlockMapToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                LinkFilterService.SetBlockMapFilterValue(toggleSwitch.IsOn);
                BlockMapFilterValue = toggleSwitch.IsOn;
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
