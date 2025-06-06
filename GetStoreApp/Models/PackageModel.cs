using System;
using System.ComponentModel;
using Windows.ApplicationModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用管理数据模型
    /// </summary>
    public sealed partial class PackageModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 应用图标
        /// </summary>
        private Uri _logoImage;

        public Uri LogoImage
        {
            get { return _logoImage; }

            set
            {
                if (!Equals(_logoImage, value))
                {
                    _logoImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogoImage)));
                }
            }
        }

        /// <summary>
        /// 是否为框架包
        /// </summary>
        public bool IsFramework { get; set; }

        /// <summary>
        /// 应用入口个数
        /// </summary>
        public int AppListEntryCount { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 应用安装日期
        /// </summary>
        public string InstallDate { get; set; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        public string PublisherDisplayName { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 包签名类型
        /// </summary>
        public PackageSignatureKind SignatureKind { get; set; }

        /// <summary>
        /// 应用安装日期
        /// </summary>
        public DateTimeOffset InstalledDate { get; set; }

        /// <summary>
        /// 应用是否正在操作中
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

        public Package Package { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
