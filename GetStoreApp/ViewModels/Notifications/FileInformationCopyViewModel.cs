using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    public sealed class FileInformationCopyViewModel : ViewModelBase
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

        public void Initialize(bool copyState)
        {
            CopyState = copyState;
        }
    }
}
