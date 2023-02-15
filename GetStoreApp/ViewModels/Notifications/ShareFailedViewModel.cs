using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    public class ShareFailedViewModel : ViewModelBase
    {
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

        public void Initialize(bool isMultiSelected)
        {
            IsMultiSelected = isMultiSelected;
        }
    }
}
