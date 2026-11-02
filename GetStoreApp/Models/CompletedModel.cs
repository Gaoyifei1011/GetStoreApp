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
        internal ImageSource IconImage
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IconImage)));
                }
            }
        }

        /// <summary>
        /// 是否处于多选模式
        /// </summary>
        internal ListViewSelectionMode SelectionMode
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
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
        internal bool IsInstalling
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsInstalling)));
                }
            }
        }

        /// <summary>
        /// 安装文件的进度
        /// </summary>
        internal double InstallProgressValue
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstallProgressValue)));
                }
            }
        }

        /// <summary>
        /// 安装是否处于等待中
        /// </summary>
        internal bool IsInstallWaiting
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsInstallWaiting)));
                }
            }
        }

        internal string InstallStateString
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstallStateString)));
                }
            }
        }

        /// <summary>
        /// 安装是否失败
        /// </summary>
        internal bool InstallFailed
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstallFailed)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
