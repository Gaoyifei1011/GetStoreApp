using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs.WinGet
{
    /// <summary>
    /// <summary>
    /// 重启设备对话框
    /// </summary>
    public sealed partial class RebootDialog : ContentDialog
    {
        public string RebootContent { get; set; }

        public RebootDialog(WinGetOptionArgs options, string appName)
        {
            InitializeComponent();
            switch (options)
            {
                case WinGetOptionArgs.SearchInstall:
                    {
                        RebootContent = string.Format(ResourceService.GetLocalized("Dialog/InstallNeedReboot"), appName);
                        break;
                    }
                case WinGetOptionArgs.UnInstall:
                    {
                        RebootContent = string.Format(ResourceService.GetLocalized("Dialog/UnInstallNeedReboot"), appName);
                        break;
                    }
                case WinGetOptionArgs.UpgradeInstall:
                    {
                        RebootContent = string.Format(ResourceService.GetLocalized("Dialog/UpgradeNeedReboot"), appName);
                        break;
                    }
            }
        }
    }
}
