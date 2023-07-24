using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using System.ComponentModel;
using System.Text;

namespace GetStoreApp.Models.Controls.WinGet
{
    public class InstallingAppsModel : INotifyPropertyChanged
    {
        private static string AppNameToolTip = ResourceService.GetLocalized("WinGet/AppNameToolTip");

        private static string InstallStateToolTip = ResourceService.GetLocalized("WinGet/InstallStateToolTip");

        private static string DownloadedProgressToolTip = ResourceService.GetLocalized("WinGet/DownloadedProgressToolTip");

        private static string QueuedToolTip = ResourceService.GetLocalized("WinGet/QueuedToolTip");

        private static string DownloadingToolTip = ResourceService.GetLocalized("WinGet/DownloadingToolTip");

        private static string InstallingToolTip = ResourceService.GetLocalized("WinGet/InstallingToolTip");

        private static string PostInstallToolTip = ResourceService.GetLocalized("WinGet/PostInstallToolTip");

        private static string FinishedToolTip = ResourceService.GetLocalized("WinGet/FinishedToolTip");

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
                _downloadProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadProgress)));
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
                _downloadedFileSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadedFileSize)));
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
                _totalFileSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalFileSize)));
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
                _installProgressState = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallProgressState)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取应用是否处于下载状态
        /// </summary>
        public bool IsDownloading(PackageInstallProgressState installProgressState)
        {
            return installProgressState != PackageInstallProgressState.Downloading;
        }

        /// <summary>
        /// 获取下载的进度
        /// </summary>
        public string GetDownloadProgress(double downloadProgress)
        {
            return string.Format("{0}%", downloadProgress);
        }

        /// <summary>
        /// 添加安装任务的详细文字信息提示
        /// </summary>
        public string InstallToolTip(string appName, PackageInstallProgressState installProgressState, double downloadProgress, string downloadedFileSize, string totalFileSize)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format(AppNameToolTip, appName));

            switch (installProgressState)
            {
                case PackageInstallProgressState.Queued:
                    {
                        builder.Append(string.Format(InstallStateToolTip, QueuedToolTip));
                        break;
                    }
                case PackageInstallProgressState.Downloading:
                    {
                        builder.AppendLine(string.Format(InstallStateToolTip, DownloadingToolTip));
                        builder.Append(string.Format(DownloadedProgressToolTip, downloadProgress));
                        builder.Append(string.Format(",{0}/{1}", downloadedFileSize, totalFileSize));
                        break;
                    }
                case PackageInstallProgressState.Installing:
                    {
                        builder.Append(string.Format(InstallStateToolTip, InstallingToolTip));
                        break;
                    }
                case PackageInstallProgressState.PostInstall:
                    {
                        builder.Append(string.Format(InstallStateToolTip, PostInstallToolTip));
                        break;
                    }
                case PackageInstallProgressState.Finished:
                    {
                        builder.Append(string.Format(InstallStateToolTip, FinishedToolTip));
                        break;
                    }
                default: break;
            }
            return builder.ToString();
        }
    }
}
