using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.WinGet
{
    /// <summary>
    /// 已安装应用数据模型
    /// </summary>
    public class InstalledAppsModel : ModelBase
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
        /// 表示应用是否处于卸载状态
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
    }
}
