using Microsoft.UI.Xaml;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：应用简介用户控件视图模型
    /// </summary>
    public sealed class IntroductionViewModel
    {
        /// <summary>
        /// 查看项目后续的维护信息
        /// </summary>
        public async void OnMaintenanceClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        }
    }
}
