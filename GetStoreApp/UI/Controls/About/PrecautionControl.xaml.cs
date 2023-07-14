using GetStoreApp.UI.Dialogs.About;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    /// <summary>
    /// 关于页面：注意事项控件
    /// </summary>
    public sealed partial class PrecautionControl : Expander
    {
        public PrecautionControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 区分传统桌面应用
        /// </summary>
        public async void OnRecognizeClicked(object sender, RoutedEventArgs args)
        {
            await new DesktopAppsDialog().ShowAsync();
        }
    }
}
