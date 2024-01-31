using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.Models.Controls.Download
{
    /// <summary>
    /// 已下载完成文件信息数据模型
    /// </summary>
    public class CompletedModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 在多选模式下，该行历史记录是否被选择的标志
        /// </summary>
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否处于多选模式
        /// </summary>
        private bool _isSelectMode;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                _isSelectMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 任务在下载状态时，获取的GID码。该值唯一
        /// </summary>
        public string GID { get; set; }

        /// <summary>
        /// 下载任务的唯一标识码，该值唯一
        /// </summary>
        public string DownloadKey { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件下载链接
        /// </summary>
        public string FileLink { get; set; }

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件下载标志：0为下载失败，1为等待下载，2为暂停下载，3为正在下载，4为成功下载
        /// </summary>
        public int DownloadFlag { get; set; }

        /// <summary>
        /// 下载文件的总大小
        /// </summary>
        public double TotalSize { get; set; }

        /// <summary>
        /// 标志是否处于正在安装状态
        /// </summary>
        private bool _isInstalling;

        public bool IsInstalling
        {
            get { return _isInstalling; }

            set
            {
                _isInstalling = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 安装文件的进度
        /// </summary>
        private double _installValue;

        public double InstallValue
        {
            get { return _installValue; }

            set
            {
                _installValue = value;
                OnPropertyChanged();
            }
        }

        // 标记安装是否出现了异常
        private bool _installError;

        public bool InstallError
        {
            get { return _installError; }

            set
            {
                _installError = value;
                OnPropertyChanged();
            }
        }

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
