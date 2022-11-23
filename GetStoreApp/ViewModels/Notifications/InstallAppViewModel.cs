using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    public class InstallAppViewModel : ViewModelBase
    {
        private bool _installAppState;

        public bool InstallAppState
        {
            get { return _installAppState; }

            set
            {
                _installAppState = value;
                OnPropertyChanged();
            }
        }

        public void Initialize(bool installAppState,string errorMessage = null)
        {
            InstallAppState = installAppState;
        }
    }
}
