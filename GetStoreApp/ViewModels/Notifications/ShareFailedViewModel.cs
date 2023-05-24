using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// 分享失败信息提示通知视图模型
    /// </summary>
    public class ShareFailedViewModel : ViewModelBase
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
