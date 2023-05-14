using GetStoreApp.UI.Dialogs.About;
using Microsoft.UI.Xaml;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：注意事项用户控件视图模型
    /// </summary>
    public sealed class PrecautionViewModel
    {
        /// <summary>
        /// 区分传统桌面应用
        /// </summary>
        public async void OnRecognizeClicked(object sender,RoutedEventArgs args)
        {
            await new DesktopAppsDialog().ShowAsync();
        }
    }
}
