using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 正在下载文件信息数据模型
    /// </summary>
    internal sealed partial class DownloadingModel : INotifyPropertyChanged
    {
        /*
        1.下载的通用信息
        */

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
        /// 是否正在进行操作
        /// </summary>
        internal bool IsOperating
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsOperating)));
                }
            }
        }

        /// <summary>
        /// 任务下载时创建下载 ID
        /// </summary>
        internal string DownloadID { get; set; }

        /*
        2.下载文件的基础信息
        */

        /// <summary>
        /// 下载文件名称
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        internal string FilePath { get; set; }

        /*
        3.下载文件的状态信息
        */

        /// <summary>
        /// 文件下载状态
        /// </summary>
        internal DownloadProgressState DownloadProgressState
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadProgressState)));
                }
            }
        }

        /// <summary>
        /// 下载文件已完成的进度
        /// </summary>
        internal double CompletedSize
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(CompletedSize)));
                }
            }
        }

        /// <summary>
        /// 下载文件的总大小
        /// </summary>
        internal double TotalSize
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(TotalSize)));
                }
            }
        }

        /// <summary>
        /// 文件下载速度
        /// </summary>
        internal double DownloadSpeed
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadSpeed)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
