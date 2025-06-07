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
        /// 存储卷名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 存储卷路径
        /// </summary>
        public string PackageVolumePath { get; set; }

        /// <summary>
        /// 存储卷可用空间
        /// </summary>
        public ulong PackageVolumeAvailableSpace { get; set; }

        /// <summary>
        /// 存储卷所有空间
        /// </summary>
        public ulong PackageVolumeTotalSpace { get; set; }

        /// <summary>
        /// 存储卷可用空间百分比
        /// </summary>
        public double PackageVolumeAvailablePercentage { get; set; }

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
        private bool _isAvailableSpaceWarning;

        public bool IsAvailableSpaceWarning
        {
            get { return _isAvailableSpaceWarning; }

            set
            {
                if (!Equals(_isAvailableSpaceWarning, value))
                {
                    _isAvailableSpaceWarning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailableSpaceWarning)));
                }
            }
        }

        /// <summary>
        /// 存储空间是否不可用（可用空间在 0% - 5%）
        /// </summary>
        private bool _isAvailableSpaceError;

        public bool IsAvailableSpaceError
        {
            get { return _isAvailableSpaceError; }

            set
            {
                if (!Equals(_isAvailableSpaceError, value))
                {
                    _isAvailableSpaceError = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailableSpaceError)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
