using Windows.ApplicationModel;

namespace GetStoreAppShellExtension.Helpers.Root
{
    /// <summary>
    /// 应用信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        // 应用安装根目录
        public static string AppInstalledLocation { get; } = Package.Current.InstalledLocation.Path;
    }
}
