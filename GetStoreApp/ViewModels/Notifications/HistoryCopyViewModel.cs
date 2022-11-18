using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    public sealed class HistoryCopyViewModel : ViewModelBase
    {
        private bool _copyState = false;

        public bool CopyState
        {
            get { return _copyState; }

            set
            {
                _copyState = value;
                OnPropertyChanged();
            }
        }

        private bool _isMultiSelected = false;

        public bool IsMultiSelected
        {
            get { return _isMultiSelected; }

            set
            {
                _isMultiSelected = value;
                OnPropertyChanged();
            }
        }

        public void Initialize(bool copyState, bool isMultiSelected)
        {
            CopyState = copyState;
            IsMultiSelected = isMultiSelected;
        }
    }
}
