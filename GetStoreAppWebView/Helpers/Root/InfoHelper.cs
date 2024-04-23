using Windows.ApplicationModel;
using Windows.Storage;

namespace GetStoreAppWebView.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        // 系统范围文件夹位置
        public static SystemDataPaths SystemDataPath { get; } = SystemDataPaths.GetDefault();

        // 常见用户数据文件夹的完整路径
        public static UserDataPaths UserDataPath { get; } = UserDataPaths.GetDefault();

        /// <summary>
        /// 获取应用安装根目录
        /// </summary>
        public static string GetAppInstalledLocation()
        {
            return Package.Current.InstalledLocation.Path;
        }
    }
}
