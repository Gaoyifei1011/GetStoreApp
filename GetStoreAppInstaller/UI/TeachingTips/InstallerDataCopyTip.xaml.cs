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
        public InstallerDataCopyTip(bool isSuccessfully = false)
        {
            InitializeComponent();
            InitializeContent(isSuccessfully);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(bool isSuccessfully)
        {
            if (isSuccessfully)
            {
                CopySuccess.Visibility = Visibility.Visible;
                CopyFailed.Visibility = Visibility.Collapsed;
                CopySuccess.Text = ResourceService.GetLocalized("Notification/ErrorInformationSuccessfully");
            }
        }
    }
}
