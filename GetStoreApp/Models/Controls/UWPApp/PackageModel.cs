using System;
using System.ComponentModel;
using Windows.ApplicationModel;

namespace GetStoreApp.Models.Controls.UWPApp
{
    /// <summary>
    /// 应用管理数据模型
    /// </summary>
    public partial class PackageModel : INotifyPropertyChanged
    {
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
        public string PublisherName { get; set; }

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
        /// 应用是否正在卸载
        /// </summary>
        private bool _isUnInstalling;

        public bool IsUnInstalling
        {
            get { return _isUnInstalling; }

            set
            {
                if (!Equals(_isUnInstalling, value))
                {
                    _isUnInstalling = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUnInstalling)));
                }
            }
        }

        public Package Package { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
