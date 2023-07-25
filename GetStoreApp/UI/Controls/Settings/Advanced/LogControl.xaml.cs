using GetStoreApp.Services.Root;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    /// <summary>
    /// 日志记录控件
    /// </summary>
    public sealed partial class LogControl : Expander
    {
        public LogControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开日志文件夹
        /// </summary>
        public async void OnOpenLogFolderClicked(object sender, RoutedEventArgs args)
        {
            await LogService.OpenLogFolderAsync();
        }

        /// <summary>
        /// 清除所有日志记录
        /// </summary>
        public void OnClearClicked(object sender, RoutedEventArgs args)
        {
            bool result = LogService.ClearLog();
            new LogCleanNotification(this, result).Show();
        }
    }
}
