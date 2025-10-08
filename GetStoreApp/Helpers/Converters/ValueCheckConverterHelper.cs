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
            return Equals(selectedIndex, index) ? Visibility.Visible : Visibility.Collapsed;
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
        public static Visibility CheckDownloadProgressState(DownloadProgressState downloadProgressState, DownloadProgressState comparedDownloadProgressState)
        {
            return Equals(downloadProgressState, comparedDownloadProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查文件是否正在下载中
        /// </summary>

        public static Visibility CheckDownloadingState(DownloadProgressState downloadProgressState)
        {
            return downloadProgressState is DownloadProgressState.Queued || downloadProgressState is DownloadProgressState.Downloading ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用是否是下载状态
        /// </summary>
        public static bool IsPackageDownloading(PackageDownloadProgressState packageDownloadProgressState)
        {
            return packageDownloadProgressState is PackageDownloadProgressState.Queued;
        }

        /// <summary>
        /// 检查 WinGet 应用是否是安装状态
        /// </summary>
        public static bool IsPackageInstalling(PackageInstallProgressState packageInstallProgressState)
        {
            return packageInstallProgressState is PackageInstallProgressState.Queued || packageInstallProgressState is PackageInstallProgressState.Installing || packageInstallProgressState is PackageInstallProgressState.PostInstall;
        }

        /// <summary>
        /// 检查 WinGet 应用是否是卸载状态
        /// </summary>
        public static bool IsPackageUninstalling(PackageUninstallProgressState packageUninstallProgressState)
        {
            return packageUninstallProgressState is PackageUninstallProgressState.Queued || packageUninstallProgressState is PackageUninstallProgressState.Uninstalling || packageUninstallProgressState is PackageUninstallProgressState.PostUninstall;
        }

        /// <summary>
        /// 检查 WinGet 应用是否是修复状态
        /// </summary>
        public static bool IsPackageRepairing(PackageRepairProgressState packageRepairProgressState)
        {
            return packageRepairProgressState is PackageRepairProgressState.Queued || packageRepairProgressState is PackageRepairProgressState.Repairing || packageRepairProgressState is PackageRepairProgressState.PostRepair;
        }

        /// <summary>
        /// 检查 WinGet 应用是否是更新状态
        /// </summary>
        public static bool IsPackageUpgrading(PackageInstallProgressState packageInstallProgressState)
        {
            return packageInstallProgressState is PackageInstallProgressState.Queued || packageInstallProgressState is PackageInstallProgressState.Installing || packageInstallProgressState is PackageInstallProgressState.PostInstall;
        }

        /// <summary>
        /// 检查 WinGet 应用的下载状态
        /// </summary>
        public static Visibility CheckPackageDownloadProgressState(PackageDownloadProgressState packageDownloadProgressState, PackageDownloadProgressState comparedPackageDownloadProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageDownloadProgressState, comparedPackageDownloadProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageDownloadProgressState, comparedPackageDownloadProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的安装状态
        /// </summary>
        public static Visibility CheckPackageInstallProgressState(PackageInstallProgressState packageInstallProgressState, PackageInstallProgressState comparedPackageInstallProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageInstallProgressState, comparedPackageInstallProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageInstallProgressState, comparedPackageInstallProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的卸载状态
        /// </summary>
        public static Visibility CheckPackageUninstallProgressState(PackageUninstallProgressState packageUninstallProgressState, PackageUninstallProgressState comparedPackageUninstallProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageUninstallProgressState, comparedPackageUninstallProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageUninstallProgressState, comparedPackageUninstallProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的修复状态
        /// </summary>
        public static Visibility CheckPackageRepairProgressState(PackageRepairProgressState packageRepairProgressState, PackageRepairProgressState comparedPackageRepairProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageRepairProgressState, comparedPackageRepairProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageRepairProgressState, comparedPackageRepairProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的安装状态
        /// </summary>
        public static Visibility CheckPackageUpgradeProgressState(PackageInstallProgressState packageInstallProgressState, PackageInstallProgressState comparedPackageInstallProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageInstallProgressState, comparedPackageInstallProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageInstallProgressState, comparedPackageInstallProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的安装状态
        /// </summary>
        public static Visibility CheckPackageOperationResultKind(PackageOperationResultKind packageOperationResultKind, PackageOperationResultKind comparedPackageOperationResultKind)
        {
            return Equals(packageOperationResultKind, comparedPackageOperationResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查更新应用状态
        /// </summary>
        public static Visibility CheckUpdateAppResultKind(UpdateAppResultKind updateAppReusltKind, UpdateAppResultKind comparedUpdateAppReusltKind)
        {
            return Equals(updateAppReusltKind, comparedUpdateAppReusltKind) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
