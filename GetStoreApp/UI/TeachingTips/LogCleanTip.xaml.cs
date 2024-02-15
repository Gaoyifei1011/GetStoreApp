using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 日志记录清除应用内通知
    /// </summary>
    public sealed partial class LogCleanTip : TeachingTip
    {
        public LogCleanTip(bool cleanResult)
        {
            InitializeComponent();
            InitializeContent(cleanResult);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(bool cleanResult)
        {
            if (cleanResult)
            {
                LogCleanSuccess.Visibility = Visibility.Visible;
                LogCleanFailed.Visibility = Visibility.Collapsed;
            }
            else
            {
                LogCleanSuccess.Visibility = Visibility.Collapsed;
                LogCleanFailed.Visibility = Visibility.Visible;
            }
        }
    }
}
