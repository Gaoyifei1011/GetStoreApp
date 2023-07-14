using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;

namespace GetStoreApp.UI.Controls.About
{
    /// <summary>
    /// 关于页面：应用简介控件
    /// </summary>
    public sealed partial class IntroductionControl : Expander
    {
        public IntroductionControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 查看项目后续的维护信息
        /// </summary>
        public async void OnMaintenanceClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        }
    }
}
