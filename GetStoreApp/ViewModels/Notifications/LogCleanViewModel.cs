using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// 日志记录清除通知视图模型
    /// </summary>
    public sealed class LogCleanViewModel : ViewModelBase
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
