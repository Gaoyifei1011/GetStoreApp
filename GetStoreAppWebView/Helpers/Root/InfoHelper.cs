using Windows.ApplicationModel;

namespace GetStoreAppWebView.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        public static string AppInstalledLocation { get; } = Package.Current.InstalledLocation.Path;
    }
}
