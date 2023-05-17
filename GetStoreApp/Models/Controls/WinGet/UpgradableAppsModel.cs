using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.WinGet
{
    /// <summary>
    /// 可升级应用数据模型
    /// </summary>
    public class UpgradableAppsModel : ModelBase
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 应用的发布者
        /// </summary>
        public string AppPublisher { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        public string AppCurrentVersion { get; set; }

        /// <summary>
        /// 应用可升级的最新版本
        /// </summary>
        public string AppNewestVersion { get; set; }
    }
}
