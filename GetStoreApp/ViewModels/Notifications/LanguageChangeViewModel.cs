using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// 语言设置修改成功时应用内通知视图模型
    /// </summary>
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
