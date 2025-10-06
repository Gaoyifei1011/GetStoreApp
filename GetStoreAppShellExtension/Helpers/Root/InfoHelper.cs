using Windows.ApplicationModel;
using Windows.Storage;

namespace GetStoreAppShellExtension.Helpers.Root
{
    /// <summary>
    /// 应用信息辅助类
    /// </summary>
    public static partial class InfoHelper
    {
        // 应用安装根目录
        public static string AppInstalledLocation { get; } = Package.Current.InstalledPath;

        // 常见用户数据文件夹的完整路径
        public static UserDataPaths UserDataPath { get; } = UserDataPaths.GetDefault();
    }
}
