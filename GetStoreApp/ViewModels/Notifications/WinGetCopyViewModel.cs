using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// WinGet 程序包应用安装、卸载、升级指令复制应用内通知视图模型
    /// </summary>
    public class WinGetCopyViewModel : ViewModelBase
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

        private WinGetOptionArgs _optionArgs;

        public WinGetOptionArgs OptionArgs
        {
            get { return _optionArgs; }

            set
            {
                _optionArgs = value;
                OnPropertyChanged();
            }
        }

        public void Initialize(bool copyState, WinGetOptionArgs optionArgs)
        {
            CopyState = copyState;
            OptionArgs = optionArgs;
        }
    }
}
