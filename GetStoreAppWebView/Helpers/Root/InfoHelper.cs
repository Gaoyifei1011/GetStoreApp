using System;
using Windows.Storage;

namespace GetStoreAppWebView.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        // 系统版本信息
        public static Version SystemVersion { get; } = Environment.OSVersion.Version;

        // 常见用户数据文件夹的完整路径
        public static UserDataPaths UserDataPath { get; } = UserDataPaths.GetDefault();
    }
}
