using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 下载任务创建成功后应用内通知
    /// </summary>
    public sealed partial class DownloadCreateTip : TeachingTip
    {
        public DownloadCreateTip(bool isDownloadCreated = false)
        {
            InitializeComponent();
            InitializeContent(isDownloadCreated);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(bool isDownloadCreated)
        {
            if (isDownloadCreated)
            {
                DownloadCreateSuccess.Text = ResourceService.GetLocalized("Notification/DownloadCreateSuccessfully");
                DownloadCreateSuccess.Visibility = Visibility.Visible;
                DownloadCreateFailed.Visibility = Visibility.Collapsed;
            }
            else
            {
                DownloadCreateFailed.Text = ResourceService.GetLocalized("Notification/DownloadCreateFailed");
                DownloadCreateSuccess.Visibility = Visibility.Collapsed;
                DownloadCreateFailed.Visibility = Visibility.Visible;
            }
        }
    }
}
