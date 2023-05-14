using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：已安装应用控件视图
    /// </summary>
    public sealed partial class InstalledAppsControl : UserControl
    {
        public InstalledAppsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化应用数量统计信息
        /// </summary>
        public string LocalizeInstalledAppsCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("WinGet/AppsCountEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("WinGet/AppsCountInfo"), count);
            }
        }
    }
}
