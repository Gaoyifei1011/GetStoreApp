using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// 下载任务创建成功后应用内通知视图模型
    /// </summary>
    public sealed class DownloadCreateViewModel : ViewModelBase
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

        public void Initialize(bool notification)
        {
            IsDownloadCreated = notification;
        }
    }
}
