using Windows.ApplicationModel.Core;

namespace GetStoreApp.Models.Controls.AppManager
{
    /// <summary>
    /// 应用程序入口数据模型
    /// </summary>
    public class AppListEntryModel
    {
        /// <summary>
        /// 应用程序入口的显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 应用程序入口的说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///应用程序入口的应用程序用户模型 ID
        /// </summary>
        public string AppUserModelId { get; set; }

        /// <summary>
        /// 应用程序入口
        /// </summary>
        public AppListEntry AppListEntry { get; set; }

        /// <summary>
        /// 包的全名
        /// </summary>
        public string PackageFullName { get; set; }
    }
}
