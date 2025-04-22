using Microsoft.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models.Controls.WinGet
{
    /// <summary>
    /// 搜索应用数据模型
    /// </summary>
    public sealed partial class SearchAppsModel : INotifyPropertyChanged
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
        public string AppVersion { get; set; }

        /// <summary>
        /// 应用是否处于正在下载中状态
        /// </summary>
        private bool _isDownloading;

        public bool IsDownloading
        {
            get { return _isDownloading; }

            set
            {
                if (!Equals(_isDownloading, value))
                {
                    _isDownloading = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDownloading)));
                }
            }
        }

        /// <summary>
        /// 应用是否处于正在安装中状态
        /// </summary>
        public bool _isInstalling;

        public bool IsInstalling
        {
            get { return _isInstalling; }

            set
            {
                if (!Equals(_isInstalling, value))
                {
                    _isInstalling = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalling)));
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
