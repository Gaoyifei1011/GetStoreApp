using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    /// <summary>
    /// 主页面：请求结果用户控件视图
    /// </summary>
    public sealed partial class ResultControl : UserControl
    {
        public string Copy { get; } = ResourceService.GetLocalized("Home/Copy");

        public string CopyOptionsToolTip { get; } = ResourceService.GetLocalized("Home/CopyOptionsToolTip");

        public string CopyLink { get; } = ResourceService.GetLocalized("Home/CopyLink");

        public string CopyLinkToolTip { get; } = ResourceService.GetLocalized("Home/CopyLinkToolTip");

        public string CopyContent { get; } = ResourceService.GetLocalized("Home/CopyContent");

        public string CopyContentToolTip { get; } = ResourceService.GetLocalized("Home/CopyContentToolTip");

        public ResultControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化CategoryId信息
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public string LocalizeCategoryId(string categoryId)
        {
            return string.Format(ResourceService.GetLocalized("Home/categoryId"), categoryId);
        }

        /// <summary>
        /// 本地化获取结果数量统计信息
        /// </summary>
        public string LocalizeResultCountInfo(int count)
        {
            return string.Format(ResourceService.GetLocalized("Home/ResultCountInfo"), count);
        }
    }
}
