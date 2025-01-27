using GetStoreAppInstaller.Services.Root;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace GetStoreAppInstaller.UI.TeachingTips
{
    /// <summary>
    /// 数据复制应用内通知
    /// </summary>
    public sealed partial class InstallerDataCopyTip : TeachingTip
    {
        public InstallerDataCopyTip(bool isSuccessfully = false, bool isMultiSelected = false, int count = 0)
        {
            InitializeComponent();
            InitializeContent(isSuccessfully, isMultiSelected, count);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(bool isSuccessfully, bool isMultiSelected, int count)
        {
            if (isSuccessfully)
            {
                CopySuccess.Visibility = Visibility.Visible;
                CopyFailed.Visibility = Visibility.Collapsed;
                CopySuccess.Text = ResourceService.GetLocalized("Notification/ErrorInformationSuccessfully");
            }
            else
            {
                CopySuccess.Visibility = Visibility.Collapsed;
                CopyFailed.Visibility = Visibility.Visible;
            }
        }
    }
}
