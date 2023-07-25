using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;
using System.ComponentModel;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 分享失败信息提示通知
    /// </summary>
    public sealed partial class ShareFailedNotification : InAppNotification, INotifyPropertyChanged
    {
        private int Count = 0;

        private bool _isMultiSelected = false;

        public bool IsMultiSelected
        {
            get { return _isMultiSelected; }

            set
            {
                _isMultiSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMultiSelected)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ShareFailedNotification(FrameworkElement element, bool isMultiSelected = false, int count = 0) : base(element)
        {
            InitializeComponent();
            IsMultiSelected = isMultiSelected;
            Count = count;
        }

        public void ShareSelectedFailedLoaded(object sender, RoutedEventArgs args)
        {
            ShareSelectedFailed.Text = string.Format(ResourceService.GetLocalized("Notification/ShareSelectedFailed"), Count);
        }
    }
}
