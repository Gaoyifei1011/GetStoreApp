using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 分享失败信息提示应用内通知
    /// </summary>
    public sealed partial class ShareFailedTip : TeachingTip
    {
        public ShareFailedTip(bool isMultiSelected = false, int count = 0)
        {
            InitializeComponent();
            InitializeContent(isMultiSelected, count);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(bool isMultiSelected, int count)
        {
            if (isMultiSelected)
            {
                ShareSelectedFailed.Text = string.Format(ResourceService.GetLocalized("Notification/ShareSelectedFailed"), count);
                ShareFailed.Visibility = Visibility.Collapsed;
                ShareSelectedFailed.Visibility = Visibility.Visible;
            }
            else
            {
                ShareFailed.Visibility = Visibility.Visible;
                ShareSelectedFailed.Visibility = Visibility.Collapsed;
            }
        }
    }
}
