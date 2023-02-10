using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// 快捷操作应用内通知视图模型
    /// </summary>
    public class QuickOperationViewModel : ViewModelBase
    {
        private QuickOperationType _operationType;

        public QuickOperationType OperationType
        {
            get { return _operationType; }

            set
            {
                _operationType = value;
                OnPropertyChanged();
            }
        }

        private bool _isPinnedSuccessfully = false;

        public bool IsPinnedSuccessfully
        {
            get { return _isPinnedSuccessfully; }

            set
            {
                _isPinnedSuccessfully = value;
                OnPropertyChanged();
            }
        }

        public void Initialize(QuickOperationType operationType, bool isPinnedSuccessfully)
        {
            OperationType = operationType;
            IsPinnedSuccessfully = isPinnedSuccessfully;
        }
    }
}
