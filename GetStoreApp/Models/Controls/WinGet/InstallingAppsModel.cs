using GetStoreApp.Models.Base;
using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using System.Text;

namespace GetStoreApp.Models.Controls.WinGet
{
    public class InstallingAppsModel : ModelBase
    {
        public static string AppNameToolTip = ResourceService.GetLocalized("WinGet/AppNameToolTip");

        public static string InstallStateToolTip = ResourceService.GetLocalized("WinGet/InstallStateToolTip");

        public static string DownloadedProgressToolTip = ResourceService.GetLocalized("WinGet/DownloadedProgressToolTip");

        public static string QueuedToolTip = ResourceService.GetLocalized("WinGet/QueuedToolTip");

        public static string DownloadingToolTip = ResourceService.GetLocalized("WinGet/DownloadingToolTip");

        public static string InstallingToolTip = ResourceService.GetLocalized("WinGet/InstallingToolTip");

        public static string PostInstallToolTip = ResourceService.GetLocalized("WinGet/PostInstallToolTip");

        public static string FinishedToolTip = ResourceService.GetLocalized("WinGet/FinishedToolTip");

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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 获取应用是否处于下载状态
        /// </summary>
        public bool IsDownloading(PackageInstallProgressState installProgressState)
        {
            return installProgressState != PackageInstallProgressState.Downloading;
        }

        /// <summary>
        /// 获取应用是否处于安装状态
        /// </summary>
        public bool IsInstalling(PackageInstallProgressState installProgressState)
        {
            return installProgressState != PackageInstallProgressState.Installing;
        }

        /// <summary>
        /// 添加安装任务的详细文字信息提示
        /// </summary>
        public string InstallToolTip(string appName, PackageInstallProgressState installProgressState, double downloadProgress)
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
