using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 重启设备对话框
    /// </summary>
    public sealed partial class RebootDialog : ContentDialog
    {
        private readonly string InstallNeedRebootString = ResourceService.GetLocalized("Dialog/InstallNeedReboot");
        private readonly string UninstallNeedRebootString = ResourceService.GetLocalized("Dialog/UninstallNeedReboot");
        private readonly string UpgradeNeedRebootString = ResourceService.GetLocalized("Dialog/UpgradeNeedReboot");

        public RebootDialog(WinGetOperationKind winGetOperationKind, string appName)
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
    }
}
