using Microsoft.Windows.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 磁盘卷数据模型
    /// </summary>
    public partial class PackageVolumeModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 是否正在操作中
        /// </summary>
        private bool _isOperating;

        public bool IsOperating
        {
            get { return _isOperating; }

            set
            {
                if (!Equals(_isOperating, value))
                {
                    _isOperating = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOperating)));
                }
            }
        }

        /// <summary>
        /// 存储卷名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 存储卷空间
        /// </summary>
        public string Space { get; set; }

        /// <summary>
        /// 存储卷 ID
        /// </summary>
        public string PackageVolumeId { get; set; }

        /// <summary>
        /// 存储卷路径
        /// </summary>
        public string PackageVolumePath { get; set; }

        /// <summary>
        /// 挂载点路径
        /// </summary>
        public string MountPoint { get; set; }

        /// <summary>
        /// 存储卷已使用空间百分比
        /// </summary>
        public double PackageVolumeUsedPercentage { get; set; }

        /// <summary>
        /// 存储卷
        /// </summary>
        public Windows.Management.Deployment.PackageVolume WinRTPackageVolume { get; set; }

        /// <summary>
        /// 存储卷
        /// </summary>
        public PackageVolume WASDKPackageVolume { get; set; }

        /// <summary>
        /// 存储卷可用空间警告（可用空间在 5% - 10%）
        /// </summary>
        public bool IsAvailableSpaceWarning { get; set; }

        /// <summary>
        /// 存储空间是否不可用（可用空间在 0% - 5%）
        /// </summary>
        public bool IsAvailableSpaceError { get; set; }

        /// <summary>
        /// 是否为默认卷（字符串）
        /// </summary>
        private string _defaultVolume;

        public string DefaultVolume
        {
            get { return _defaultVolume; }

            set
            {
                if (!Equals(_defaultVolume, value))
                {
                    _defaultVolume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultVolume)));
                }
            }
        }

        /// <summary>
        /// 是否为默认卷
        /// </summary>
        private bool _isDefaultVolume;

        public bool IsDefaultVolume
        {
            get { return _isDefaultVolume; }

            set
            {
                if (!Equals(_isDefaultVolume, value))
                {
                    _isDefaultVolume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDefaultVolume)));
                }
            }
        }

        /// <summary>
        /// 是否支持 APPX 安装
        /// </summary>
        private string _isAppxInstallSupported;

        public string IsAppxInstallSupported
        {
            get { return _isAppxInstallSupported; }

            set
            {
                if (!Equals(_isAppxInstallSupported, value))
                {
                    _isAppxInstallSupported = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAppxInstallSupported)));
                }
            }
        }

        /// <summary>
        /// 是否支持完全信任包
        /// </summary>
        private string _isFullTrustPackageSupported;

        public string IsFullTrustPackageSupported
        {
            get { return _isFullTrustPackageSupported; }

            set
            {
                if (!Equals(_isFullTrustPackageSupported, value))
                {
                    _isFullTrustPackageSupported = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFullTrustPackageSupported)));
                }
            }
        }

        /// <summary>
        /// 是否处于脱机状态（字符串）
        /// </summary>
        private string _offline;

        public string Offline
        {
            get { return _offline; }

            set
            {
                if (!Equals(_offline, value))
                {
                    _offline = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Offline)));
                }
            }
        }

        /// <summary>
        /// 是否处于脱机状态
        /// </summary>
        private bool _isOffline;

        public bool IsOffline
        {
            get { return _isOffline; }

            set
            {
                if (!Equals(_isOffline, value))
                {
                    _isOffline = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFullTrustPackageSupported)));
                }
            }
        }

        /// <summary>
        /// 是否为系统卷
        /// </summary>
        private string _isSystemVolume;

        public string IsSystemVolume
        {
            get { return _isSystemVolume; }

            set
            {
                if (!Equals(_isSystemVolume, value))
                {
                    _isSystemVolume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemVolume)));
                }
            }
        }

        /// <summary>
        /// 是否支持在其文件系统中创建硬链接
        /// </summary>
        private string _supportedHardLinks;

        public string SupportedHardLinks
        {
            get { return _supportedHardLinks; }

            set
            {
                if (!Equals(_supportedHardLinks, value))
                {
                    _supportedHardLinks = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemVolume)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
