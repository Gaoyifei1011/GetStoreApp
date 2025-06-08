using Microsoft.Windows.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 磁盘卷数据模型
    /// </summary>
    public class PackageVolumeModel
    {
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
    }
}
