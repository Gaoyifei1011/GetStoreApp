using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
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
