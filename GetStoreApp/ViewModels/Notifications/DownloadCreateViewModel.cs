using CommunityToolkit.Mvvm.ComponentModel;

namespace GetStoreApp.ViewModels.Notifications
{
    public class DownloadCreateViewModel : ObservableRecipient
    {
        private bool _isDownloadCreated = false;

        public bool IsDownloadCreated
        {
            get { return _isDownloadCreated; }

            set { SetProperty(ref _isDownloadCreated, value); }
        }

        public void Initialize(bool notification)
        {
            IsDownloadCreated = notification;
        }
    }
}
