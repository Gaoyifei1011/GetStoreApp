using Windows.ApplicationModel;

namespace GetStoreApp.Models.Controls.PackageManager
{
    /// <summary>
    /// 应用管理数据模型
    /// </summary>
    public class PackageModel
    {
        /// <summary>
        /// 应用是否正在卸载
        /// </summary>
        public bool IsUnInstalling { get; set; }

        public Package Package { get; set; }
    }
}
