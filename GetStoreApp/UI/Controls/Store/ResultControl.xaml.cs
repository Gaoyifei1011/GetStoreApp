using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 微软商店页面：请求结果用户控件视图
    /// </summary>
    public sealed partial class ResultControl : Grid
    {
        public ResultControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化CategoryId信息
        /// </summary>
        public string LocalizeCategoryId(string categoryId)
        {
            return string.Format(ResourceService.GetLocalized("Store/categoryId"), categoryId);
        }

        /// <summary>
        /// 本地化获取结果数量统计信息
        /// </summary>
        public string LocalizeResultCountInfo(int count)
        {
            return string.Format(ResourceService.GetLocalized("Store/ResultCountInfo"), count);
        }
    }
}
