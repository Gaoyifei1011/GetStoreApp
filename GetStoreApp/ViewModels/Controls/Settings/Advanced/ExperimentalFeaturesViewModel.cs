using GetStoreApp.UI.Dialogs.Settings;
using Microsoft.UI.Xaml;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：实验性功能设置用户控件视图模型
    /// </summary>
    public sealed class ExperimentalFeaturesViewModel
    {
        /// <summary>
        /// 实验功能设置
        /// </summary>
        public async void OnConfigClicked(object sender, RoutedEventArgs args)
        {
            await new ExperimentalConfigDialog().ShowAsync();
        }
    }
}
