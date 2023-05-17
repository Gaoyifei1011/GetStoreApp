using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Settings;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：痕迹清理设置用户控件视图模型
    /// </summary>
    public sealed class TraceCleanupViewModel
    {
        /// <summary>
        /// 痕迹清理说明
        /// </summary>
        public void OnTraceCleanupTipClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 清理应用内使用的所有痕迹
        /// </summary>
        public async void OnTraceCleanupClicked(object sender, RoutedEventArgs args)
        {
            await new TraceCleanupPromptDialog().ShowAsync();
        }
    }
}
