using Microsoft.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models.Controls.WinGet
{
    /// <summary>
    /// 正在安装中应用的数据模型
    /// </summary>
    public sealed partial class InstallingAppsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 下载进度
        /// </summary>
        private double _downloadProgress;

        public double DownloadProgress
        {
            get { return _downloadProgress; }

            set
            {
                if (!Equals(_downloadProgress, value))
                {
                    _downloadProgress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadProgress)));
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
        /// 安装状态
        /// </summary>
        private PackageInstallProgressState _installProgressState;

        public PackageInstallProgressState InstallProgressState
        {
            get { return _installProgressState; }

            set
            {
                if (!Equals(_installProgressState, value))
                {
                    _installProgressState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallProgressState)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
