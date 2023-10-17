using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Store.Preview.InstallControl;

namespace GetStoreApp.Models.Controls.AppUpdate
{
    /// <summary>
    /// 商店应用更新数据模型
    /// </summary>
    public class AppUpdateModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 标志应用是否处于升级状态
        /// </summary>
        private bool _isUpdating;

        public bool IsUpdating
        {
            get { return _isUpdating; }

            set
            {
                _isUpdating = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 应用显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        public string PublisherName { get; set; }

        /// <summary>
        /// 应用的包系列名称
        /// </summary>
        public string PackageFamilyName { get; set; }

        /// <summary>
        /// 安装信息
        /// </summary>
        private string _installInformation;

        public string InstallInformation
        {
            get { return _installInformation; }

            set
            {
                _installInformation = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 详细的安装信息（工具提示中显示）
        /// </summary>
        private string _installDetailInformation;

        public string InstallDetailInformation
        {
            get { return _installDetailInformation; }

            set
            {
                _installDetailInformation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 应用的产品 ID
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 当前应用的安装状态
        /// </summary>
        private AppInstallState _appInstallState;

        public AppInstallState AppInstallState
        {
            get { return _appInstallState; }

            set
            {
                _appInstallState = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 已为当前应用下载的字节数
        /// </summary>
        private ulong _bytesDownload;

        public ulong BytesDownload
        {
            get { return _bytesDownload; }

            set
            {
                _bytesDownload = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前应用的下载大小（以字节为单位）
        /// </summary>
        private ulong _downloadSizeInBytes;

        public ulong DownloadSizeInBytes
        {
            get { return _downloadSizeInBytes; }

            set
            {
                _downloadSizeInBytes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前应用的安装完成百分比
        /// </summary>
        private double _percentComplete;

        public double PercentComplete
        {
            get { return _percentComplete; }

            set
            {
                _percentComplete = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 遇到安装失败的应用的错误代码
        /// </summary>
        public Exception ErrorCode { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
