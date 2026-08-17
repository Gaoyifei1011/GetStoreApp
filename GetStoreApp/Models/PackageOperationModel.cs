using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.Management.Deployment;
using System.ComponentModel;
using Windows.Foundation;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用包操作数据模型
    /// </summary>
    internal sealed partial class PackageOperationModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 应用包操作数据类型
        /// </summary>
        internal PackageOperationKind PackageOperationKind { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        internal string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        internal string AppName { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        internal string AppVersion { get; set; }

        /// <summary>
        /// 应用包下载目录
        /// </summary>
        internal string PackagePath { get; set; }

        /// <summary>
        /// 版本所有信息
        /// </summary>
        internal PackageVersionId PackageVersionId { get; set; }

        internal SearchAppsModel SearchApps { get; set; }

        internal InstalledAppsModel InstalledApps { get; set; }

        internal UpgradableAppsModel UpgradableApps { get; set; }

        /// <summary>
        /// 应用包操作进度
        /// </summary>
        private double _packageOperationProgress;

        internal double PackageOperationProgress
        {
            get { return _packageOperationProgress; }

            set
            {
                if (!Equals(_packageOperationProgress, value))
                {
                    _packageOperationProgress = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageOperationProgress)));
                }
            }
        }

        /// <summary>
        /// 已下载文件的大小
        /// </summary>
        internal string _downloadedFileSize;

        internal string DownloadedFileSize
        {
            get { return _downloadedFileSize; }

            set
            {
                if (!string.Equals(_downloadedFileSize, value))
                {
                    _downloadedFileSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadedFileSize)));
                }
            }
        }

        /// <summary>
        /// 总文件大小
        /// </summary>
        internal string _totalFileSize;

        internal string TotalFileSize
        {
            get { return _totalFileSize; }

            set
            {
                if (!Equals(_totalFileSize, value))
                {
                    _totalFileSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(TotalFileSize)));
                }
            }
        }

        private PackageOperationResultKind _packageOperationResultKind;

        internal PackageOperationResultKind PackageOperationResultKind
        {
            get { return _packageOperationResultKind; }

            set
            {
                if (!Equals(_packageOperationResultKind, value))
                {
                    _packageOperationResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageOperationResultKind)));
                }
            }
        }

        /// <summary>
        /// 应用包下载状态
        /// </summary>
        private PackageDownloadProgressState _packageDownloadProgressState;

        internal PackageDownloadProgressState PackageDownloadProgressState
        {
            get { return _packageDownloadProgressState; }

            set
            {
                if (!Equals(_packageDownloadProgressState, value))
                {
                    _packageDownloadProgressState = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageDownloadProgressState)));
                }
            }
        }

        /// <summary>
        /// 应用包安装状态
        /// </summary>
        private PackageInstallProgressState _packageInstallProgressState;

        internal PackageInstallProgressState PackageInstallProgressState
        {
            get { return _packageInstallProgressState; }

            set
            {
                if (!Equals(_packageInstallProgressState, value))
                {
                    _packageInstallProgressState = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageInstallProgressState)));
                }
            }
        }

        private PackageUninstallProgressState _packageUninstallProgressState;

        internal PackageUninstallProgressState PackageUninstallProgressState
        {
            get { return _packageUninstallProgressState; }

            set
            {
                if (!Equals(_packageUninstallProgressState, value))
                {
                    _packageUninstallProgressState = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageUninstallProgressState)));
                }
            }
        }

        private PackageRepairProgressState _packageRepairProgressState;

        internal PackageRepairProgressState PackageRepairProgressState
        {
            get { return _packageRepairProgressState; }

            set
            {
                if (!Equals(_packageRepairProgressState, value))
                {
                    _packageRepairProgressState = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageRepairProgressState)));
                }
            }
        }

        private string _packageOperationFailedContent;

        internal string PackageOperationFailedContent
        {
            get { return _packageOperationFailedContent; }

            set
            {
                if (!string.Equals(_packageOperationFailedContent, value))
                {
                    _packageOperationFailedContent = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageOperationFailedContent)));
                }
            }
        }

        internal DownloadOptions DownloadOptions { get; set; }

        internal InstallOptions InstallOptions { get; set; }

        internal UninstallOptions UninstallOptions { get; set; }

        internal RepairOptions RepairOptions { get; set; }

        internal IAsyncOperationWithProgress<DownloadResult, PackageDownloadProgress> PackageDownloadProgress { get; set; }

        internal IAsyncOperationWithProgress<InstallResult, InstallProgress> PackageInstallProgress { get; set; }

        internal IAsyncOperationWithProgress<UninstallResult, UninstallProgress> PackageUninstallProgress { get; set; }

        internal IAsyncOperationWithProgress<RepairResult, RepairProgress> PackageRepairProgress { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
