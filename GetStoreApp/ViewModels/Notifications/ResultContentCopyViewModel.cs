using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// 请求结果内容复制应用内通知视图模型
    /// </summary>
    public sealed class ResultContentCopyViewModel : ViewModelBase
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
