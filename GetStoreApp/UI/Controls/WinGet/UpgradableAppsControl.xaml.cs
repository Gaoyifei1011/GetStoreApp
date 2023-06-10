using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.WinGet
{
    public sealed partial class UpgradableAppsControl : Grid
    {
        public UpgradableAppsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化应用数量统计信息
        /// </summary>
        public string LocalizeUpgradableAppsCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("WinGet/UpgradableAppsCountEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("WinGet/UpgradableAppsCountInfo"), count);
            }
        }
    }
}
