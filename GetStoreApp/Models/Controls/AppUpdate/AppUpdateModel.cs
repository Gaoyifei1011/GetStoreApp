using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Store.Preview.InstallControl;

namespace GetStoreApp.Models.Controls.AppUpdate
{
    /// <summary>
    /// 商店应用更新数据模型
    /// </summary>
    public class AppUpdateModel : INotifyPropertyChanged
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
                _isUpdating = value;
                OnPropertyChanged();
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
                _installInformation = value;

                OnPropertyChanged();
            }
        }

        private string _installSubInformation;

        public string InstallSubInformation
        {
            get { return _installSubInformation; }

            set
            {
                _installSubInformation = value;
                OnPropertyChanged();
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
                _appInstallState = value;
                OnPropertyChanged();
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
                _percentComplete = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Visibility IsDownloading(AppInstallState appInstallState)
        {
            if (appInstallState is AppInstallState.Downloading)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
