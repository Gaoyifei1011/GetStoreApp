using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 重启设备对话框
    /// </summary>
    internal sealed partial class RebootDialog : ContentDialog
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string InstallNeedRebootString = ResourceService.GetLocalized("Dialog/InstallNeedReboot");
        private readonly string UninstallNeedRebootString = ResourceService.GetLocalized("Dialog/UninstallNeedReboot");
        private readonly string UpgradeNeedRebootString = ResourceService.GetLocalized("Dialog/UpgradeNeedReboot");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：构造函数

        internal RebootDialog(WinGetOperationKind winGetOperationKind, string appName)
        {
            InitializeComponent();
            switch (winGetOperationKind)
            {
                case WinGetOperationKind.SearchInstall:
                    {
                        Content = string.Format(InstallNeedRebootString, appName);
                        break;
                    }
                case WinGetOperationKind.Uninstall:
                    {
                        Content = string.Format(UninstallNeedRebootString, appName);
                        break;
                    }
                case WinGetOperationKind.Upgrade:
                    {
                        Content = string.Format(UpgradeNeedRebootString, appName);
                        break;
                    }
            }
        }

        #endregion 第二部分：构造函数
    }
}
