using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    /// <summary>
    /// 下载页面：已下载完成用户控件视图
    /// </summary>
    public sealed partial class CompletedControl : Grid
    {
        public CompletedControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化已下载完成数量统计信息
        /// </summary>
        public string LocalizeCompletedCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("Download/CompletedEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("Download/CompletedCountInfo"), count);
            }
        }
    }
}
