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
        /// 是否正在进行操作
        /// </summary>
        private bool _isOperating;

        internal bool IsOperating
        {
            get { return _isOperating; }

            set
            {
                if (!Equals(_isOperating, value))
                {
                    _isOperating = value;
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
        private DownloadProgressState _downloadProgressState;

        internal DownloadProgressState DownloadProgressState
        {
            get { return _downloadProgressState; }

            set
            {
                if (!Equals(_downloadProgressState, value))
                {
                    _downloadProgressState = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadProgressState)));
                }
            }
        }

        /// <summary>
        /// 下载文件已完成的进度
        /// </summary>
        private double _completedSize;

        internal double CompletedSize
        {
            get { return _completedSize; }

            set
            {
                if (!Equals(_completedSize, value))
                {
                    _completedSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(CompletedSize)));
                }
            }
        }

        /// <summary>
        /// 下载文件的总大小
        /// </summary>
        private double _totalSize;

        internal double TotalSize
        {
            get { return _totalSize; }

            set
            {
                if (!Equals(_totalSize, value))
                {
                    _totalSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(TotalSize)));
                }
            }
        }

        /// <summary>
        /// 文件下载速度
        /// </summary>
        private double _downloadSpeed;

        internal double DownloadSpeed
        {
            get { return _downloadSpeed; }

            set
            {
                if (!Equals(_downloadSpeed, value))
                {
                    _downloadSpeed = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadSpeed)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
