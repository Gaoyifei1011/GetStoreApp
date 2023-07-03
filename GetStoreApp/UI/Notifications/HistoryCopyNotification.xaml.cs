using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 历史记录复制应用内通知视图
    /// </summary>
    public sealed partial class HistoryCopyNotification : InAppNotification
    {
        private int Count = 0;

        public HistoryCopyNotification(bool isMultiSelected = false, int count = 0)
        {
            Count = count;

            InitializeComponent();
            ViewModel.Initialize(isMultiSelected);
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("Notification/HistorySelectedCopy"), Count);
        }
    }
}
