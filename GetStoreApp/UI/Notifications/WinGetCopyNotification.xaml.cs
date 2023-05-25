using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Views.CustomControls.Notifications;
using System.Runtime.InteropServices;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// WinGet 程序包应用安装、卸载、升级指令复制应用内通知视图
    /// </summary>
    public sealed partial class WinGetCopyNotification : InAppNotification
    {
        public WinGetCopyNotification([Optional, DefaultParameterValue(false)] bool copyState, [Optional, DefaultParameterValue(WinGetOptionArgs.SearchInstall)] WinGetOptionArgs optionArgs)
        {
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            InitializeComponent();
            ViewModel.Initialize(copyState, optionArgs);
        }

        public bool ControlLoaded(bool copyState, WinGetOptionArgs optionArgs, string controlName)
        {
            if (controlName is "SearchInstallCopySuccess" && copyState && optionArgs == WinGetOptionArgs.SearchInstall)
            {
                return true;
            }
            else if (controlName is "SearchInstallCopyFailed" && !copyState && optionArgs == WinGetOptionArgs.SearchInstall)
            {
                return true;
            }
            else if (controlName is "UnInstallCopySuccess" && copyState && optionArgs == WinGetOptionArgs.UnInstall)
            {
                return true;
            }
            else if (controlName is "UnInstallCopyFailed" && !copyState && optionArgs == WinGetOptionArgs.UnInstall)
            {
                return true;
            }
            else if (controlName is "UpgradeInstallCopySuccess" && copyState && optionArgs == WinGetOptionArgs.UpgradeInstall)
            {
                return true;
            }
            else if (controlName is "UpgradeInstallCopyFailed" && !copyState && optionArgs == WinGetOptionArgs.UpgradeInstall)
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
