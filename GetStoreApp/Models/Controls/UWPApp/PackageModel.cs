using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;

namespace GetStoreApp.Models.Controls.UWPApp
{
    /// <summary>
    /// 应用管理数据模型
    /// </summary>
    public class PackageModel : INotifyPropertyChanged
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
                _isUnInstalling = value;
                OnPropertyChanged();
            }
        }

        public Package Package { get; set; }

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
