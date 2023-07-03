using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 请求结果内容复制应用内通知视图
    /// </summary>
    public sealed partial class ResultContentCopyNotification : InAppNotification
    {
        private int Count = 0;

        public ResultContentCopyNotification(bool isMultiSelected = false, int count = 0)
        {
            Count = count;

            InitializeComponent();
            ViewModel.Initialize(isMultiSelected);
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("Notification/ResultContentSelectedCopy"), Count);
        }
    }
}
