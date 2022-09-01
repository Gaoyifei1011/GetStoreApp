using CommunityToolkit.Mvvm.ComponentModel;

namespace GetStoreApp.ViewModels.Notifications
{
    public class HistoryCopyViewModel : ObservableRecipient
    {
        private bool _copyState = false;

        public bool CopyState
        {
            get { return _copyState; }

            set { SetProperty(ref _copyState, value); }
        }

        private bool _isMultiSelected = false;

        public bool IsMultiSelected
        {
            get { return _isMultiSelected; }

            set { SetProperty(ref _isMultiSelected, value); }
        }

        public void Initialize(bool copyState, bool isMultiSelected)
        {
            CopyState = copyState;
            IsMultiSelected = isMultiSelected;
        }
    }
}
