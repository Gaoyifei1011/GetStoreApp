using CommunityToolkit.Mvvm.ComponentModel;

namespace GetStoreApp.ViewModels.Notifications
{
    public class LanguageChangeViewModel : ObservableRecipient
    {
        private bool _setResult = false;

        public bool SetResult
        {
            get { return _setResult; }

            set { SetProperty(ref _setResult, value); }
        }

        public void Initialize(bool notification)
        {
            SetResult = notification;
        }
    }
}
