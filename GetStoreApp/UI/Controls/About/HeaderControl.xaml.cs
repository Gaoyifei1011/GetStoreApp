using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    /// <summary>
    /// 关于页面：顶部栏用户控件视图
    /// </summary>
    public sealed partial class HeaderControl : UserControl
    {
        public HeaderControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化应用版本信息
        /// </summary>
        private string LocalizeAppVersion(string appVersion)
        {
            return string.Format(ResourceService.GetLocalized("About/AppVersion"), appVersion);
        }
    }
}
