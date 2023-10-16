using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.Models.Controls.AppUpdate
{
    /// <summary>
    /// 商店应用更新数据模型
    /// </summary>
    public class AppUpdateModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 标志是否正在初始化下载任务
        /// </summary>
        private bool _isInitializeTasks;

        public bool IsInitializeTasks
        {
            get { return _isInitializeTasks; }

            set
            {
                _isInitializeTasks = value;
                OnPropertyChanged();
            }
        }

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
        /// 安装状态
        /// </summary>
        public string InstallState { get; set; }

        /// <summary>
        /// 应用的产品 ID
        /// </summary>
        public string ProductId { get; set; }

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
