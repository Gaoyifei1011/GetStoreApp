using GetStoreApp.Extensions.DataType.Classes;
using Microsoft.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models.Controls.Settings
{
    /// <summary>
    /// WinGet 数据源模型
    /// </summary>
    public partial class WinGetSourceModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// 数据源信息
        /// </summary>
        public PackageCatalogInformation PackageCatalogInformation { get; set; }

        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据源参数
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// 数据源是否是显性
        /// </summary>
        public string Explicit { get; set; }

        /// <summary>
        /// 数据源信任等级
        /// </summary>
        public string TrustLevel { get; set; }

        /// <summary>
        /// 数据源 ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 数据源最后一次更新时间
        /// </summary>
        public string LastUpdateTime { get; set; }

        /// <summary>
        /// 数据源源类型
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 数据源是否可接受参数
        /// </summary>
        public string AcceptSourceAgreements { get; set; }

        /// <summary>
        /// 数据源额外参数
        /// </summary>
        public string AdditionalPackageCatalogArguments { get; set; }

        /// <summary>
        /// 数据源验证类型
        /// </summary>
        public string AuthenticationType { get; set; }

        /// <summary>
        /// 数据源验证账户
        /// </summary>
        public string AuthenticationAccount { get; set; }

        /// <summary>
        /// 数据源后台更新间隔
        /// </summary>
        public string PackageCatalogBackgroundUpdateInterval { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
