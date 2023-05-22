using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：搜索应用控件视图
    /// </summary>
    public sealed partial class SearchAppsControl : UserControl
    {
        public SearchAppsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化应用数量统计信息
        /// </summary>
        public string LocalizeSearchAppsCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("WinGet/SearchedAppsCountEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("WinGet/SearchedAppsCountInfo"), count);
            }
        }
    }
}
