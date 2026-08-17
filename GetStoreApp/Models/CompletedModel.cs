using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 已下载完成文件信息数据模型
    /// </summary>
    internal sealed partial class CompletedModel : INotifyPropertyChanged
    {
        private ImageSource _iconImage;

        internal ImageSource IconImage
        {
            get { return _iconImage; }

            set
            {
                if (!Equals(_iconImage, value))
                {
                    _iconImage = value;
                    PropertyChanged?.Invoke(this, new(nameof(IconImage)));
                }
            }
        }

        /// <summary>
        /// 是否处于多选模式
        /// </summary>
        private ListViewSelectionMode _selectionMode;

        internal ListViewSelectionMode SelectionMode
        {
            get { return _selectionMode; }

            set
            {
                if (!Equals(_selectionMode, value))
                {
                    _selectionMode = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectionMode)));
                }
            }
        }

        /// <summary>
        /// 下载任务的唯一标识码，该值唯一
        /// </summary>
        internal string DownloadKey { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// 文件下载链接
        /// </summary>
        internal string FileLink { get; set; }

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        internal string FilePath { get; set; }

        /// <summary>
        /// 下载文件的总大小
        /// </summary>
        internal double TotalSize { get; set; }

        /// <summary>
        /// 标志是否处于正在安装状态
        /// </summary>
        private bool _isInstalling;

        internal bool IsInstalling
        {
            get { return _isInstalling; }

            set
            {
                if (!Equals(_isInstalling, value))
                {
                    _isInstalling = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsInstalling)));
                }
            }
        }

        /// <summary>
        /// 安装文件的进度
        /// </summary>
        private double _installProgressValue;

        internal double InstallProgressValue
        {
            get { return _installProgressValue; }

            set
            {
                if (!Equals(_installProgressValue, value))
                {
                    _installProgressValue = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstallProgressValue)));
                }
            }
        }

        /// <summary>
        /// 安装是否处于等待中
        /// </summary>
        private bool _isInstallWaiting;

        internal bool IsInstallWaiting
        {
            get { return _isInstallWaiting; }

            set
            {
                if (!Equals(_isInstallWaiting, value))
                {
                    _isInstallWaiting = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsInstallWaiting)));
                }
            }
        }

        private string _installStateString;

        internal string InstallStateString
        {
            get { return _installStateString; }

            set
            {
                if (!string.Equals(_installStateString, value))
                {
                    _installStateString = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstallStateString)));
                }
            }
        }

        /// <summary>
        /// 安装是否失败
        /// </summary>
        private bool _installFailed;

        internal bool InstallFailed
        {
            get { return _installFailed; }

            set
            {
                if (!Equals(_installFailed, value))
                {
                    _installFailed = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstallFailed)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
