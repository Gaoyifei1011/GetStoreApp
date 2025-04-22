using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs.WinGet
{
    /// <summary>
    /// 重启设备对话框
    /// </summary>
    public sealed partial class RebootDialog : ContentDialog
    {
        public RebootDialog(WinGetOptionKind options, string appName)
        {
            InitializeComponent();
            switch (options)
            {
                case WinGetOptionKind.SearchInstall:
                    {
                        Content = string.Format(ResourceService.GetLocalized("Dialog/InstallNeedReboot"), appName);
                        break;
                    }
                case WinGetOptionKind.Uninstall:
                    {
                        Content = string.Format(ResourceService.GetLocalized("Dialog/UninstallNeedReboot"), appName);
                        break;
                    }
                case WinGetOptionKind.UpgradeInstall:
                    {
                        Content = string.Format(ResourceService.GetLocalized("Dialog/UpgradeNeedReboot"), appName);
                        break;
                    }
            }
        }
    }
}
