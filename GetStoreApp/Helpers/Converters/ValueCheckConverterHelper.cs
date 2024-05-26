using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;

namespace GetStoreApp.Helpers.Converters
{
    /// <summary>
    /// 值检查辅助类
    /// </summary>
    public static class ValueCheckConverterHelper
    {
        /// <summary>
        /// 检查枚举值中是否设置了一个或多个位域
        /// </summary>
        public static bool IsEnumHasFlag(Enum value, Enum comparedValue)
        {
            return value.HasFlag(comparedValue);
        }

        /// <summary>
        /// 判断值是否相同（取反）
        /// </summary>
        public static bool ObjectCompareReverseConvert(object value, object comparedValue)
        {
            return !Equals(value, comparedValue);
        }

        /// <summary>
        /// 判断两个版本是否共同存在
        /// </summary>
        public static bool IsVersionExisted(bool isOfficialVersionExisted, bool isDevVersionExisted)
        {
            return isOfficialVersionExisted && isDevVersionExisted;
        }

        /// <summary>
        /// 判断 WinGet 程序包是否存在
        /// </summary>
        public static bool IsWinGetExisted(bool isOfficialVersionExisted, bool isDevVersionExisted, bool needReverseValue)
        {
            bool result = isOfficialVersionExisted || isDevVersionExisted;
            return needReverseValue ? !result : result;
        }

        /// <summary>
        /// 检查痕迹清理对话框按钮是否可用
        /// </summary>
        public static bool IsTraceCleanupButtonEnabled(bool isSelected, bool isCleaning)
        {
            return !isCleaning && isSelected;
        }

        /// <summary>
        /// 检查搜索框是否可用
        /// </summary>
        public static bool IsSearchBoxEnabled(bool notSearched, bool isSearchCompleted)
        {
            return notSearched || isSearchCompleted;
        }

        /// <summary>
        /// 确定当前选择的索引是否为目标控件
        /// </summary>
        public static Visibility IsCurrentControl(int selectedIndex, int index)
        {
            return selectedIndex.Equals(index) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查商店应用是否在下载
        /// </summary>
        public static Visibility IsAppInstallDownloading(AppInstallState appInstallState)
        {
            return appInstallState is AppInstallState.Downloading ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检测当前应用是否为商店应用
        /// </summary>
        public static Visibility IsStorePackage(PackageSignatureKind packageSignatureKind)
        {
            return packageSignatureKind is PackageSignatureKind.Store ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检测当前应用是否为系统应用（系统应用无法卸载）
        /// </summary>
        public static Visibility IsNotSystemPackage(PackageSignatureKind packageSignatureKind)
        {
            return packageSignatureKind is PackageSignatureKind.System ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查下载任务状态
        /// </summary>
        public static Visibility CheckDownloadStatus(DownloadStatus downloadStatus, DownloadStatus comparedDownloadStatus)
        {
            return downloadStatus.Equals(comparedDownloadStatus) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用是否是下载状态
        /// </summary>
        public static bool IsPackageInstallDownloading(PackageInstallProgressState installProgressState)
        {
            return installProgressState is not PackageInstallProgressState.Downloading;
        }

        /// <summary>
        /// 检查 WinGet 应用的下载状态
        /// </summary>
        public static Visibility CheckPackageInstallProgressState(PackageInstallProgressState packageInstallProgressState, PackageInstallProgressState comparedPackageInstallProgressState)
        {
            return packageInstallProgressState.Equals(comparedPackageInstallProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
