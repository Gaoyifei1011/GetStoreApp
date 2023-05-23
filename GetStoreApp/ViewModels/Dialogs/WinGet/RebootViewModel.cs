using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;

namespace GetStoreApp.ViewModels.Dialogs.WinGet
{
    /// <summary>
    /// 重启设备对话框视图模型
    /// </summary>
    public sealed class RebootViewModel
    {
        public string RebootContent { get; set; }

        /// <summary>
        /// 初始化重启文字提示内容
        /// </summary>
        public void InitializeRebootContent(WinGetOptionArgs optionArgs, string appName)
        {
            switch (optionArgs)
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
