using GetStoreApp.UI.Dialogs.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：实验性功能设置控件
    /// </summary>
    public sealed partial class ExperimentalFeaturesControl : Grid
    {
        public ExperimentalFeaturesControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 实验功能设置
        /// </summary>
        public async void OnConfigClicked(object sender, RoutedEventArgs args)
        {
            await new ExperimentalConfigDialog().ShowAsync();
        }
    }
}
