using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 更新检查应用内通知
    /// </summary>
    public sealed partial class CheckUpdateTip : TeachingTip
    {
        public CheckUpdateTip(bool isNewest)
        {
            InitializeComponent();
            InitializeContent(isNewest);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(bool isNewest)
        {
            if (isNewest)
            {
                NewestVersion.Visibility = Visibility.Visible;
                NotNewestVersion.Visibility = Visibility.Collapsed;
            }
            else
            {
                NewestVersion.Visibility = Visibility.Collapsed;
                NotNewestVersion.Visibility = Visibility.Visible;
            }
        }
    }
}
