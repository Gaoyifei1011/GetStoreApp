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
        internal double PackageOperationProgress
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageOperationProgress)));
                }
            }
        }

        /// <summary>
        /// 已下载文件的大小
        /// </summary>
        internal string DownloadedFileSize
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadedFileSize)));
                }
            }
        }

        /// <summary>
        /// 总文件大小
        /// </summary>
        internal string TotalFileSize
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(TotalFileSize)));
                }
            }
        }

        internal PackageOperationResultKind PackageOperationResultKind
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageOperationResultKind)));
                }
            }
        }

        /// <summary>
        /// 应用包下载状态
        /// </summary>
        internal PackageDownloadProgressState PackageDownloadProgressState
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageDownloadProgressState)));
                }
            }
        }

        /// <summary>
        /// 应用包安装状态
        /// </summary>
        internal PackageInstallProgressState PackageInstallProgressState
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageInstallProgressState)));
                }
            }
        }

        internal PackageUninstallProgressState PackageUninstallProgressState
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageUninstallProgressState)));
                }
            }
        }

        internal PackageRepairProgressState PackageRepairProgressState
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageRepairProgressState)));
                }
            }
        }

        internal string PackageOperationFailedContent
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
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
