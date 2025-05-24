using Microsoft.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 可更新应用数据模型
    /// </summary>
    public sealed partial class UpgradableAppsModel : INotifyPropertyChanged
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
        /// 应用的发布者
        /// </summary>
        public string AppPublisher { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        public string AppCurrentVersion { get; set; }

        /// <summary>
        /// 应用可更新的最新版本
        /// </summary>
        public string AppNewestVersion { get; set; }

        /// <summary>
        /// 应用是否处于正在升级中状态
        /// </summary>
        private bool _isUpgrading;

        public bool IsUpgrading
        {
            get { return _isUpgrading; }

            set
            {
                if (!Equals(_isUpgrading, value))
                {
                    _isUpgrading = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpgrading)));
                }
            }
        }

        /// <summary>
        /// 匹配到的应用包
        /// </summary>
        public CatalogPackage CatalogPackage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
