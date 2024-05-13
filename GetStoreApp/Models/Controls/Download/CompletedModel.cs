using GetStoreApp.Services.Root;
using System.ComponentModel;

namespace GetStoreApp.Models.Controls.Download
{
    /// <summary>
    /// 已下载完成文件信息数据模型
    /// </summary>
    public class CompletedModel : INotifyPropertyChanged
    {
        public string InstalledValue = ResourceService.GetLocalized("Download/InstallValue");

        /// <summary>
        /// 在多选模式下，该行历史记录是否被选择的标志
        /// </summary>
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
        /// 是否处于多选模式
        /// </summary>
        private bool _isSelectMode;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                if (!Equals(_isSelectMode, value))
                {
                    _isSelectMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectMode)));
                }
            }
        }

        /// <summary>
        /// 是否正在进行操作
        /// </summary>
        private bool _isNotOperated;

        public bool IsNotOperated
        {
            get { return _isNotOperated; }

            set
            {
                if (!Equals(_isNotOperated, value))
                {
                    _isNotOperated = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotOperated)));
                }
            }
        }

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
                if (!Equals(_isInstalling, value))
                {
                    _isInstalling = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalling)));
                }
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
                if (!Equals(_installValue, value))
                {
                    _installValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallValue)));
                }
            }
        }

        // 标记安装是否出现了异常
        private bool _installError;

        public bool InstallError
        {
            get { return _installError; }

            set
            {
                if (!Equals(_installError, value))
                {
                    _installError = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallError)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
