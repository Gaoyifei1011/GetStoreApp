using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
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
        /// 检查痕迹清理对话框按钮是否可用
        /// </summary>
        public static bool IsTraceCleanupButtonEnabled(bool isSelected, bool isCleaning)
        {
            return !isCleaning && isSelected;
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
