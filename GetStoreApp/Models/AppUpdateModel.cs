using System;
using System.ComponentModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 商店应用更新数据模型
    /// </summary>
    internal sealed partial class AppUpdateModel : INotifyPropertyChanged
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
        /// 标志应用是否处于升级状态
        /// </summary>
        internal bool IsUpdating
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUpdating)));
                }
            }
        }

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
        /// 应用显示名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        internal string PublisherDisplayName { get; set; }

        /// <summary>
        /// 应用的包系列名称
        /// </summary>
        internal string PackageFamilyName { get; set; }

        /// <summary>
        /// 安装信息
        /// </summary>
        internal string InstallInformation
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstallInformation)));
                }
            }
        }

        internal string InstallSubInformation
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstallSubInformation)));
                }
            }
        }

        /// <summary>
        /// 应用的产品 ID
        /// </summary>
        internal string ProductId { get; set; }

        /// <summary>
        /// 当前应用的安装状态
        /// </summary>
        internal AppInstallState AppInstallState
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(AppInstallState)));
                }
            }
        }

        /// <summary>
        /// 当前应用的安装完成百分比
        /// </summary>
        internal double PercentComplete
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(PercentComplete)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
