using Windows.ApplicationModel.Core;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用程序入口数据模型
    /// </summary>
    internal sealed class AppListEntryModel
    {
        /// <summary>
        /// 应用程序入口的显示名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 应用程序入口的说明
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        ///应用程序入口的应用程序用户模型 ID
        /// </summary>
        internal string AppUserModelId { get; set; }

        /// <summary>
        /// 应用程序入口
        /// </summary>
        internal AppListEntry AppListEntry { get; set; }

        /// <summary>
        /// 包的全名
        /// </summary>
        internal string PackageFullName { get; set; }
    }
}
