using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadCreateNotification(FrameworkElement element, bool isDownloadCreated = false) : base(element)
        {
            InitializeComponent();
            IsDownloadCreated = isDownloadCreated;
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
