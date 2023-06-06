using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    /// <summary>
    /// 下载页面：未下载完成用户控件视图
    /// </summary>
    public sealed partial class UnfinishedControl : UserControl
    {
        public UnfinishedControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化未下载完成数量统计信息
        /// </summary>
        public string LocalizeUnfinishedCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("Download/UnfinishedEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("Download/UnfinishedCountInfo"), count);
            }
        }
    }
}
