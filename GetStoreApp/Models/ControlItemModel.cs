namespace GetStoreApp.Models
{
    /// <summary>
    /// 主页面项目数据模型
    /// </summary>
    public sealed class ControlItemModel
    {
        /// <summary>
        /// 主页面项目标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 主页面项目描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 主页面项目图片路径
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 主页面项目标签
        /// </summary>
        public string Tag { get; set; }
    }
}
