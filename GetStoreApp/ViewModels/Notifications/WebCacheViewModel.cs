using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// 网页缓存清理成功后应用内通知视图模型
    /// </summary>
    public sealed class WebCacheViewModel : ViewModelBase
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
