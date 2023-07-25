using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;
using System.ComponentModel;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 历史记录复制应用内通知
    /// </summary>
    public sealed partial class HistoryCopyNotification : InAppNotification, INotifyPropertyChanged
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

        public HistoryCopyNotification(FrameworkElement element, bool isMultiSelected = false, int count = 0) : base(element)
        {
            InitializeComponent();
            IsMultiSelected = isMultiSelected;
            Count = count;
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("Notification/HistorySelectedCopy"), Count);
        }
    }
}
