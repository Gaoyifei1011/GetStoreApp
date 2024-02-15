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
                DownloadCreateSuccess.Visibility = Visibility.Visible;
                DownloadCreateFailed.Visibility = Visibility.Collapsed;
            }
            else
            {
                DownloadCreateSuccess.Visibility = Visibility.Collapsed;
                DownloadCreateFailed.Visibility = Visibility.Visible;
            }
        }
    }
}
