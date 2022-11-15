using CommunityToolkit.Mvvm.ComponentModel;

namespace GetStoreApp.ViewModels.Notifications
{
    public class ResultIDCopyViewModel : ObservableRecipient
    {
        private bool _copyState = false;

        public bool CopyState
        {
            get { return _copyState; }

            set { SetProperty(ref _copyState, value); }
        }

        public void Initialize(bool copyState)
        {
            CopyState = copyState;
        }
    }
}
