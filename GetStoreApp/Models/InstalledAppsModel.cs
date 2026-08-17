using Microsoft.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 已安装应用数据模型
    /// </summary>
    internal sealed partial class InstalledAppsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        internal string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        internal string AppName { get; set; }

        /// <summary>
        /// 应用的发布者
        /// </summary>
        internal string AppPublisher { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        internal string AppVersion { get; set; }

        /// <summary>
        /// 是否正在卸载应用
        /// </summary>
        private bool _isUninstalling;

        internal bool IsUninstalling
        {
            get { return _isUninstalling; }

            set
            {
                if (!Equals(_isUninstalling, value))
                {
                    _isUninstalling = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUninstalling)));
                }
            }
        }

        internal CatalogPackage CatalogPackage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
