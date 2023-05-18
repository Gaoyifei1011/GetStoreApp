using GetStoreApp.UI.Dialogs.Settings;
using Microsoft.UI.Xaml;

namespace GetStoreApp.ViewModels.Pages
{
    /// <summary>
    /// 设置页面数据模型
    /// </summary>
    public sealed class SettingsViewModel
    {
        /// <summary>
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        public async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await new RestartAppsDialog().ShowAsync();
        }
    }
}
