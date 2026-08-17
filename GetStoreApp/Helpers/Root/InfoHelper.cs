using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview;
using Windows.Storage;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 应用信息辅助类
    /// </summary>
    internal static class InfoHelper
    {
        // 应用版本信息
        internal static Version AppVersion { get; } = new(
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Build,
            Package.Current.Id.Version.Revision
            );

        // 常见用户数据文件夹的完整路径
        internal static UserDataPaths UserDataPath { get; } = UserDataPaths.GetDefault();

        // 应用安装根目录
        internal static string AppInstalledLocation { get; } = Package.Current.InstalledLocation.Path;

        // 传递优化是否可用
        internal static bool IsDeliveryOptimizationEnabled { get; }

        static InfoHelper()
        {
            DeliveryOptimizationSettings deliveryOptimizationSettings = DeliveryOptimizationSettings.GetCurrentSettings();
            IsDeliveryOptimizationEnabled = deliveryOptimizationSettings.DownloadMode is DeliveryOptimizationDownloadMode.HttpOnly || deliveryOptimizationSettings.DownloadMode is DeliveryOptimizationDownloadMode.Lan || deliveryOptimizationSettings.DownloadMode is DeliveryOptimizationDownloadMode.Group || deliveryOptimizationSettings.DownloadMode is DeliveryOptimizationDownloadMode.Internet;
        }
    }
}
