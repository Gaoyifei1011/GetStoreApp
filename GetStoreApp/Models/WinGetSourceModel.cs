using GetStoreApp.Extensions.DataType.Classes;
using Microsoft.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// WinGet 数据源模型
    /// </summary>
    internal partial class WinGetSourceModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        internal bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
                }
            }
        }

        private bool _isOperating;

        internal bool IsOperating
        {
            get { return _isOperating; }

            set
            {
                if (!Equals(_isOperating, value))
                {
                    _isOperating = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsOperating)));
                }
            }
        }

        /// <summary>
        /// 数据源信息
        /// </summary>
        internal PackageCatalogInformation PackageCatalogInformation { get; set; }

        /// <summary>
        /// 数据源名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 数据源参数
        /// </summary>
        internal string Arguments { get; set; }

        /// <summary>
        /// 数据源是否是显性
        /// </summary>
        internal string Explicit { get; set; }

        /// <summary>
        /// 数据源信任等级
        /// </summary>
        internal string TrustLevel { get; set; }

        /// <summary>
        /// 数据源 ID
        /// </summary>
        internal string SourceId { get; set; }

        /// <summary>
        /// 数据源最后一次更新时间
        /// </summary>
        internal string LastUpdateTime { get; set; }

        /// <summary>
        /// 数据源源类型
        /// </summary>
        internal string Origin { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        internal string Type { get; set; }

        /// <summary>
        /// 数据源是否可接受参数
        /// </summary>
        internal string AcceptSourceAgreements { get; set; }

        /// <summary>
        /// 数据源额外参数
        /// </summary>
        internal string AdditionalPackageCatalogArguments { get; set; }

        /// <summary>
        /// 数据源验证类型
        /// </summary>
        internal string AuthenticationType { get; set; }

        /// <summary>
        /// 数据源验证账户
        /// </summary>
        internal string AuthenticationAccount { get; set; }

        /// <summary>
        /// 数据源后台更新间隔
        /// </summary>
        internal string PackageCatalogBackgroundUpdateInterval { get; set; }

        /// <summary>
        /// 是否是内置源
        /// </summary>
        internal bool IsInternal { get; set; }

        /// <summary>
        /// 预定义的数据源类型
        /// </summary>
        internal PredefinedPackageCatalog? PredefinedPackageCatalog { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
