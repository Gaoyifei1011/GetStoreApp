using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartsWithEFilterValue)));
            }
        }

        private bool _blockMapFilterValue = LinkFilterService.BlockMapFilterValue;

        public bool BlockMapFilterValue
        {
            get { return _blockMapFilterValue; }

            set
            {
                _blockMapFilterValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlockMapFilterValue)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LinkFilterControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 链接过滤说明
        /// </summary>
        public void OnLinkFilterInstructionClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 设置是否过滤以“.e”开头的文件
        /// </summary>
        public async void OnStartWithEToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await LinkFilterService.SetStartsWithEFilterValueAsync(toggleSwitch.IsOn);
                StartsWithEFilterValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 设置是否过滤包块映射文件
        /// </summary>
        public async void OnBlockMapToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await LinkFilterService.SetBlockMapFilterValueAsync(toggleSwitch.IsOn);
                BlockMapFilterValue = toggleSwitch.IsOn;
            }
        }
    }
}
