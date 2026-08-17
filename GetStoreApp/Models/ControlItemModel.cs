using System;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 主页面项目数据模型
    /// </summary>
    internal sealed class ControlItemModel
    {
        /// <summary>
        /// 主页面项目标题
        /// </summary>
        internal string Title { get; set; }

        /// <summary>
        /// 主页面项目描述
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// 主页面项目图片路径
        /// </summary>
        internal string ImagePath { get; set; }

        /// <summary>
        /// 主页面项目标签
        /// </summary>
        internal string Tag { get; set; }

        /// <summary>
        /// 主页面项目对应的页面
        /// </summary>
        internal Type NavigationPage { get; set; }
    }
}
