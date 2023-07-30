using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.UI.Dialogs.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：痕迹清理设置控件
    /// </summary>
    public sealed partial class TraceCleanupControl : Grid
    {
        public TraceCleanupControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 清理应用内使用的所有痕迹
        /// </summary>
        public async void OnTraceCleanupClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new TraceCleanupPromptDialog(), this);
        }
    }
}
