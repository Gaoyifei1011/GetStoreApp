using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    public sealed class LanguageChangeViewModel : ViewModelBase
    {
        private bool _setResult = false;

        public bool SetResult
        {
            get { return _setResult; }

            set
            {
                _setResult = value;
                OnPropertyChanged();
            }
        }

        public void Initialize(bool setResult)
        {
            SetResult = setResult;
        }
    }
}
