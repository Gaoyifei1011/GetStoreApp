using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// WinGet 程序包应用卸载指令复制成功后应用内通知视图模型
    /// </summary>
    public class UnInstallCopyViewModel : ViewModelBase
    {
        private bool _copyState = false;

        public bool CopyState
        {
            get { return _copyState; }

            set
            {
                _copyState = value;
                OnPropertyChanged();
            }
        }

        public void Initialize(bool copyState)
        {
            CopyState = copyState;
        }
    }
}
