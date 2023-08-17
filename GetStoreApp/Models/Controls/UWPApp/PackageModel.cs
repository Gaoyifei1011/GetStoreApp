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
        /// 应用入口个数
        /// </summary>
        public int AppListEntryCount { get; set; }

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
