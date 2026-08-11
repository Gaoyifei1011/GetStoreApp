using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Store.Preview.InstallControl;

namespace GetStoreApp.Helpers.Converters
{
    /// <summary>
    /// 值检查辅助类
    /// </summary>
    public static class ValueCheckConverterHelper
    {
        /// <summary>
        /// 检查是否支持显示分享面板
        /// </summary>
        public static Visibility CheckShareUIVisibility()
        {
            return DataTransferManager.IsSupported() && !RuntimeHelper.IsElevated ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查痕迹清理对话框按钮是否可用
        /// </summary>
        public static bool IsTraceCleanupButtonEnabled(bool isSelected, bool isCleaning)
        {
            return !isCleaning && isSelected;
        }

        /// <summary>
        /// 检查商店应用安装状态
        /// </summary>
        public static Visibility CheckAppInstallStateVisibility(AppInstallState appInstallState, AppInstallState comparedAppInstallState)
        {
            return Equals(appInstallState, comparedAppInstallState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查商店应用类型
        /// </summary>
        public static Visibility CheckPackageSignatureKindVisibility(PackageSignatureKind packageSignatureKind, PackageSignatureKind comparedPackageSignatureKind, bool needReverse)
        {
            return needReverse ? Equals(packageSignatureKind, comparedPackageSignatureKind) ? Visibility.Visible : Visibility.Collapsed : Equals(packageSignatureKind, comparedPackageSignatureKind) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查下载任务状态
        /// </summary>
        public static Visibility CheckDownloadProgressStateVisibility(DownloadProgressState downloadProgressState, DownloadProgressState comparedDownloadProgressState)
        {
            return Equals(downloadProgressState, comparedDownloadProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查文件是否正在下载中
        /// </summary>
        public static Visibility CheckDownloadingStateVisibility(DownloadProgressState downloadProgressState)
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
        public static Visibility CheckPackageDownloadProgressStateVisibility(PackageDownloadProgressState packageDownloadProgressState, PackageDownloadProgressState comparedPackageDownloadProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageDownloadProgressState, comparedPackageDownloadProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageDownloadProgressState, comparedPackageDownloadProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的安装状态
        /// </summary>
        public static Visibility CheckPackageInstallProgressStateVisibility(PackageInstallProgressState packageInstallProgressState, PackageInstallProgressState comparedPackageInstallProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageInstallProgressState, comparedPackageInstallProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageInstallProgressState, comparedPackageInstallProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的卸载状态
        /// </summary>
        public static Visibility CheckPackageUninstallProgressStateVisibility(PackageUninstallProgressState packageUninstallProgressState, PackageUninstallProgressState comparedPackageUninstallProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageUninstallProgressState, comparedPackageUninstallProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageUninstallProgressState, comparedPackageUninstallProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的修复状态
        /// </summary>
        public static Visibility CheckPackageRepairProgressStateVisibility(PackageRepairProgressState packageRepairProgressState, PackageRepairProgressState comparedPackageRepairProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageRepairProgressState, comparedPackageRepairProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageRepairProgressState, comparedPackageRepairProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的安装状态
        /// </summary>
        public static Visibility CheckPackageUpgradeProgressStateVisibility(PackageInstallProgressState packageInstallProgressState, PackageInstallProgressState comparedPackageInstallProgressState, bool isReverse)
        {
            return isReverse ? Equals(packageInstallProgressState, comparedPackageInstallProgressState) ? Visibility.Collapsed : Visibility.Visible : Equals(packageInstallProgressState, comparedPackageInstallProgressState) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查 WinGet 应用的安装状态
        /// </summary>
        public static Visibility CheckPackageOperationResultKindVisibility(PackageOperationResultKind packageOperationResultKind, PackageOperationResultKind comparedPackageOperationResultKind)
        {
            return Equals(packageOperationResultKind, comparedPackageOperationResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查更新应用状态
        /// </summary>
        public static Visibility CheckUpdateAppResultKindVisibility(UpdateAppResultKind updateAppResultKind, UpdateAppResultKind comparedUpdateAppResultKind)
        {
            return Equals(updateAppResultKind, comparedUpdateAppResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查列表控件选中项样式
        /// </summary>
        public static Visibility CheckListViewSelectionModeVisibility(ListViewSelectionMode listViewSelectionMode, ListViewSelectionMode comparedListViewSelectionMode)
        {
            return Equals(listViewSelectionMode, comparedListViewSelectionMode) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
