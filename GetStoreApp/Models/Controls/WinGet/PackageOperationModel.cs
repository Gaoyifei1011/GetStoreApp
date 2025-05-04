using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.Management.Deployment;
using System.ComponentModel;
using Windows.Foundation;

namespace GetStoreApp.Models.Controls.WinGet
{
    /// <summary>
    /// 应用包操作数据模型
    /// </summary>
    public sealed partial class PackageOperationModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 应用包操作数据类型
        /// </summary>
        public PackageOperationKind PackageOperationKind { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// 应用包下载目录
        /// </summary>
        public string PackagePath { get; set; }

        /// <summary>
        /// 下载进度
        /// </summary>
        private double _packageOperationProgress;

        public double PackageOperationProgress
        {
            get { return _packageOperationProgress; }

            set
            {
                if (!Equals(_packageOperationProgress, value))
                {
                    _packageOperationProgress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageOperationProgress)));
                }
            }
        }

        /// <summary>
        /// 已下载文件的大小
        /// </summary>
        public string _downloadedFileSize;

        public string DownloadedFileSize
        {
            get { return _downloadedFileSize; }

            set
            {
                if (!Equals(_downloadedFileSize, value))
                {
                    _downloadedFileSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadedFileSize)));
                }
            }
        }

        /// <summary>
        /// 总文件大小
        /// </summary>
        public string _totalFileSize;

        public string TotalFileSize
        {
            get { return _totalFileSize; }

            set
            {
                if (!Equals(_totalFileSize, value))
                {
                    _totalFileSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalFileSize)));
                }
            }
        }

        private bool _isCanceling;

        public bool IsCanceling
        {
            get { return _isCanceling; }

            set
            {
                if (!Equals(_isCanceling, value))
                {
                    _isCanceling = true;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCanceling)));
                }
            }
        }

        /// <summary>
        /// 应用包下载状态
        /// </summary>
        private PackageDownloadProgressState _packageDownloadProgressState;

        public PackageDownloadProgressState PackageDownloadProgressState
        {
            get { return _packageDownloadProgressState; }

            set
            {
                if (!Equals(_packageDownloadProgressState, value))
                {
                    _packageDownloadProgressState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageDownloadProgressState)));
                }
            }
        }

        /// <summary>
        /// 应用包安装状态
        /// </summary>
        private PackageInstallProgressState _packageInstallProgressState;

        public PackageInstallProgressState PackageInstallProgressState
        {
            get { return _packageInstallProgressState; }

            set
            {
                if (!Equals(_packageInstallProgressState, value))
                {
                    _packageInstallProgressState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageInstallProgressState)));
                }
            }
        }

        private PackageUninstallProgressState _packageUninstallProgressState;

        public PackageUninstallProgressState PackageUninstallProgressState
        {
            get { return _packageUninstallProgressState; }

            set
            {
                if (!Equals(_packageUninstallProgressState, value))
                {
                    _packageUninstallProgressState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageUninstallProgressState)));
                }
            }
        }

        private PackageRepairProgressState _packageRepairProgressState;

        public PackageRepairProgressState PackageRepairProgressState
        {
            get { return _packageRepairProgressState; }

            set
            {
                if (!Equals(_packageRepairProgressState, value))
                {
                    _packageRepairProgressState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageRepairProgressState)));
                }
            }
        }

        public IAsyncOperationWithProgress<DownloadResult, PackageDownloadProgress> PackageDownloadProgress { get; set; }

        public IAsyncOperationWithProgress<InstallResult, InstallProgress> PackageInstallProgress { get; set; }

        public IAsyncOperationWithProgress<UninstallResult, UninstallProgress> PackageUninstallProgress { get; set; }

        public IAsyncOperationWithProgress<RepairResult, RepairProgress> PackageRepairProgress { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
