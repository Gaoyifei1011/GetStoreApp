using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 请求结果链接复制成功后应用内通知视图
    /// </summary>
    public sealed partial class ResultLinkCopyNotification : InAppNotification
    {
        private int Count = 0;

        private bool IsMultiSelected = false;

        public ResultLinkCopyNotification(bool copyState = false, bool isMultiSelected = false, int count = 0)
        {
            IsMultiSelected = isMultiSelected;
            Count = count;

            InitializeComponent();
            ViewModel.Initialize(copyState, isMultiSelected);
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            if (IsMultiSelected)
            {
                CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("Notification/ResultLinkSelectedCopySuccessfully"), Count);
            }
        }

        public void CopySelectedFailedLoaded(object sender, RoutedEventArgs args)
        {
            if (IsMultiSelected)
            {
                CopySelectedFailed.Text = string.Format(ResourceService.GetLocalized("Notification/ResultLinkSelectedCopyFailed"), Count);
            }
        }

        public bool ControlLoad(bool copyState, bool isMultiSelected, string controlName)
        {
            if (controlName is "CopySuccess" && copyState && !isMultiSelected)
            {
                return true;
            }
            else if (controlName is "CopyFailed" && !copyState && !isMultiSelected)
            {
                return true;
            }
            else if (controlName is "CopySelectedSuccess" && copyState && isMultiSelected)
            {
                return true;
            }
            else if (controlName is "CopySelectedFailed" && !copyState && isMultiSelected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
