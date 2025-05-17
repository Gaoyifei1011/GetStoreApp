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
        public RebootDialog(WinGetOperationKind options, string appName)
        {
            InitializeComponent();
            switch (options)
            {
                case WinGetOperationKind.SearchInstall:
                    {
                        Content = string.Format(ResourceService.GetLocalized("Dialog/InstallNeedReboot"), appName);
                        break;
                    }
                case WinGetOperationKind.Uninstall:
                    {
                        Content = string.Format(ResourceService.GetLocalized("Dialog/UninstallNeedReboot"), appName);
                        break;
                    }
                case WinGetOperationKind.Upgrade:
                    {
                        Content = string.Format(ResourceService.GetLocalized("Dialog/UpgradeNeedReboot"), appName);
                        break;
                    }
            }
        }
    }
}
