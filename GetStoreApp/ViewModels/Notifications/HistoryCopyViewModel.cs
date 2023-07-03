using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// 历史记录复制应用内通知视图模型
    /// </summary>
    public sealed class HistoryCopyViewModel : ViewModelBase
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
