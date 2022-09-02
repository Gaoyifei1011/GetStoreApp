using CommunityToolkit.Mvvm.ComponentModel;

namespace GetStoreApp.ViewModels.Notifications
{
    public class DownloadCreateViewModel : ObservableRecipient
    {
        private bool _createState = false;

        public bool CreateState
        {
            get { return _createState; }

            set { SetProperty(ref _createState, value); }
        }

        private bool _isMultiSelected = false;

        public bool IsMultiSelected
        {
            get { return _isMultiSelected; }

            set { SetProperty(ref _isMultiSelected, value); }
        }

        public void Initialize(bool copyState, bool isMultiSelected)
        {
            CreateState = copyState;
            IsMultiSelected = isMultiSelected;
        }
    }
}
