using GetStoreApp.Views.CustomControls.Notifications;
using System.ComponentModel;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 下载任务创建成功后应用内通知
    /// </summary>
    public sealed partial class DownloadCreateNotification : InAppNotification, INotifyPropertyChanged
    {
        private bool _isDownloadCreated = false;

        public bool IsDownloadCreated
        {
            get { return _isDownloadCreated; }

            set
            {
                _isDownloadCreated = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDownloadCreated)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadCreateNotification(bool isDownloadCreated = false)
        {
            InitializeComponent();
            IsDownloadCreated = isDownloadCreated;
        }
    }
}
