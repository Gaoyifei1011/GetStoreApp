using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 应用重启对话框
    /// </summary>
    internal sealed partial class RestartAppsDialog : ContentDialog
    {
        #region 第一部分：构造函数

        internal RestartAppsDialog()
        {
            InitializeComponent();
        }

        #endregion 第一部分：构造函数

        #region 第二部分：挂载事件处理

        /// <summary>
        /// 重启应用
        /// </summary>
        private void OnRestartAppsClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            AppInstance.Restart("Restart");
        }

        #endregion 第二部分：挂载事件处理
    }
}
