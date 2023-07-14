using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Views.CustomControls.Notifications;
using System.ComponentModel;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// WinGet 程序包应用安装、卸载、升级指令复制应用内通知
    /// </summary>
    public sealed partial class WinGetCopyNotification : InAppNotification, INotifyPropertyChanged
    {
        private WinGetOptionArgs _optionArgs;

        public WinGetOptionArgs OptionArgs
        {
            get { return _optionArgs; }

            set
            {
                _optionArgs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OptionArgs)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetCopyNotification(WinGetOptionArgs optionArgs)
        {
            InitializeComponent();
            OptionArgs = optionArgs;
        }

        public bool ControlLoaded(WinGetOptionArgs optionArgs, string controlName)
        {
            if (controlName is "SearchInstallCopySuccess" && optionArgs == WinGetOptionArgs.SearchInstall)
            {
                return true;
            }
            else if (controlName is "UnInstallCopySuccess" && optionArgs == WinGetOptionArgs.UnInstall)
            {
                return true;
            }
            else if (controlName is "UpgradeInstallCopySuccess" && optionArgs == WinGetOptionArgs.UpgradeInstall)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
