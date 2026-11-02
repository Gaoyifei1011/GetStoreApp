using Microsoft.Windows.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 磁盘卷数据模型
    /// </summary>
    internal sealed partial class PackageVolumeModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 是否正在操作中
        /// </summary>
        internal bool IsOperating
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsOperating)));
                }
            }
        }

        /// <summary>
        /// 存储卷名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 存储卷空间
        /// </summary>
        internal string Space { get; set; }

        /// <summary>
        /// 存储卷 ID
        /// </summary>
        internal string PackageVolumeId { get; set; }

        /// <summary>
        /// 存储卷路径
        /// </summary>
        internal string PackageVolumePath { get; set; }

        /// <summary>
        /// 挂载点路径
        /// </summary>
        internal string MountPoint { get; set; }

        /// <summary>
        /// 存储卷已使用空间百分比
        /// </summary>
        internal double PackageVolumeUsedPercentage { get; set; }

        /// <summary>
        /// 存储卷
        /// </summary>
        internal PackageVolume PackageVolume { get; set; }

        /// <summary>
        /// 存储卷
        /// </summary>
        internal Windows.Management.Deployment.PackageVolume WinRTPackageVolume { get; set; }

        /// <summary>
        /// 存储卷可用空间警告（可用空间在 5% - 10%）
        /// </summary>
        internal bool IsAvailableSpaceWarning { get; set; }

        /// <summary>
        /// 存储空间是否不可用（可用空间在 0% - 5%）
        /// </summary>
        internal bool IsAvailableSpaceError { get; set; }

        /// <summary>
        /// 是否为默认卷（字符串）
        /// </summary>
        internal string DefaultVolume
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(DefaultVolume)));
                }
            }
        }

        /// <summary>
        /// 是否为默认卷
        /// </summary>
        internal bool IsDefaultVolume
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsDefaultVolume)));
                }
            }
        }

        /// <summary>
        /// 是否支持 APPX 安装
        /// </summary>
        internal string IsAppxInstallSupported
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsAppxInstallSupported)));
                }
            }
        }

        /// <summary>
        /// 是否支持完全信任包
        /// </summary>
        internal string IsFullTrustPackageSupported
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsFullTrustPackageSupported)));
                }
            }
        }

        /// <summary>
        /// 是否处于脱机状态（字符串）
        /// </summary>
        internal string Offline
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(Offline)));
                }
            }
        }

        /// <summary>
        /// 是否处于脱机状态
        /// </summary>
        internal bool IsOffline
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsFullTrustPackageSupported)));
                }
            }
        }

        /// <summary>
        /// 是否为系统卷
        /// </summary>
        internal string IsSystemVolume
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSystemVolume)));
                }
            }
        }

        /// <summary>
        /// 是否支持在其文件系统中创建硬链接
        /// </summary>
        internal string SupportedHardLinks
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSystemVolume)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
