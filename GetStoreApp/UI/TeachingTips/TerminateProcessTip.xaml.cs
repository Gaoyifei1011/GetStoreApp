using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 终止浏览器进程结果应用内通知
    /// </summary>
    public sealed partial class TerminateProcessTip : TeachingTip
    {
        public TerminateProcessTip(bool isTerminated)
        {
            InitializeComponent();
            InitializeContent(isTerminated);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(bool isTerminated)
        {
            if (isTerminated)
            {
                TerminateSuccess.Visibility = Visibility.Visible;
                TerminateFailed.Visibility = Visibility.Collapsed;
            }
            else
            {
                TerminateSuccess.Visibility = Visibility.Collapsed;
                TerminateFailed.Visibility = Visibility.Visible;
            }
        }
    }
}
