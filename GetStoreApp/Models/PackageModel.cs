using System;
using System.ComponentModel;
using Windows.ApplicationModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用管理数据模型
    /// </summary>
    internal sealed partial class PackageModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 应用图标
        /// </summary>
        internal Uri LogoImage
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(LogoImage)));
                }
            }
        }

        /// <summary>
        /// 是否为框架包
        /// </summary>
        internal bool IsFramework { get; set; }

        /// <summary>
        /// 应用入口个数
        /// </summary>
        internal int AppListEntryCount { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 应用安装日期
        /// </summary>
        internal string InstallDate { get; set; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        internal string PublisherDisplayName { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        internal string Version { get; set; }

        /// <summary>
        /// 包签名类型
        /// </summary>
        internal PackageSignatureKind SignatureKind { get; set; }

        /// <summary>
        /// 应用安装日期
        /// </summary>
        internal DateTimeOffset InstalledDate { get; set; }

        /// <summary>
        /// 应用是否正在操作中
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
        /// 应用包操作进度
        /// </summary>
        internal double PackageOperationProgress
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageOperationProgress)));
                }
            }
        }

        internal Package Package { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
