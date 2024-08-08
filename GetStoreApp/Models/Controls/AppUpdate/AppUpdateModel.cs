using System.ComponentModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;

namespace GetStoreApp.Models.Controls.AppUpdate
{
    /// <summary>
    /// 商店应用更新数据模型
    /// </summary>
    public sealed partial class AppUpdateModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 标志应用是否处于升级状态
        /// </summary>
        private bool _isUpdating;

        public bool IsUpdating
        {
            get { return _isUpdating; }

            set
            {
                if (!Equals(_isUpdating, value))
                {
                    _isUpdating = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdating)));
                }
            }
        }

        /// <summary>
        /// 应用显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        public string PublisherName { get; set; }

        /// <summary>
        /// 应用的包系列名称
        /// </summary>
        public string PackageFamilyName { get; set; }

        /// <summary>
        /// 安装信息
        /// </summary>
        private string _installInformation;

        public string InstallInformation
        {
            get { return _installInformation; }

            set
            {
                if (!Equals(_installInformation, value))
                {
                    _installInformation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallInformation)));
                }
            }
        }

        private string _installSubInformation;

        public string InstallSubInformation
        {
            get { return _installSubInformation; }

            set
            {
                if (!Equals(_installSubInformation, value))
                {
                    _installSubInformation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallSubInformation)));
                }
            }
        }

        /// <summary>
        /// 应用的产品 ID
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 当前应用的安装状态
        /// </summary>
        private AppInstallState _appInstallState;

        public AppInstallState AppInstallState
        {
            get { return _appInstallState; }

            set
            {
                if (!Equals(_appInstallState, value))
                {
                    _appInstallState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppInstallState)));
                }
            }
        }

        /// <summary>
        /// 当前应用的安装完成百分比
        /// </summary>
        private double _percentComplete;

        public double PercentComplete
        {
            get { return _percentComplete; }

            set
            {
                if (!Equals(_percentComplete, value))
                {
                    _percentComplete = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PercentComplete)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
