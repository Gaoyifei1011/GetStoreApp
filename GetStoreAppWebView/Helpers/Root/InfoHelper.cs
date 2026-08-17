using Windows.Storage;

namespace GetStoreAppWebView.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    internal static class InfoHelper
    {
        // 常见用户数据文件夹的完整路径
        internal static UserDataPaths UserDataPath { get; } = UserDataPaths.GetDefault();
    }
}
