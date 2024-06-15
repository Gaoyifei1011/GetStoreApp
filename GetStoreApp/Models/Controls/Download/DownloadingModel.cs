using GetStoreApp.Extensions.DataType.Enums;
using System;
using System.ComponentModel;

namespace GetStoreApp.Models.Controls.Download
{
    /// <summary>
    /// 正在下载文件信息数据模型
    /// </summary>
    public partial class DownloadingModel : INotifyPropertyChanged
    {
        /*
        1.下载的通用信息
        */

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
        /// 任务下载时创建下载 ID
        /// </summary>
        public Guid DownloadID { get; set; }

        /*
        2.下载文件的基础信息
        */

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
        /// 文件SHA1值，用来校验文件
        /// </summary>
        public string FileSHA1 { get; set; }

        /*
        3.下载文件的状态信息
        */

        /// <summary>
        /// 文件下载状态
        /// </summary>
        private DownloadStatus _downloadStatus;

        public DownloadStatus DownloadStatus
        {
            get { return _downloadStatus; }

            set
            {
                if (!Equals(_downloadStatus, value))
                {
                    _downloadStatus = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadStatus)));
                }
            }
        }

        /// <summary>
        /// 下载文件的总大小
        /// </summary>
        private double _totalSize;

        public double TotalSize
        {
            get { return _totalSize; }

            set
            {
                if (!Equals(_totalSize, value))
                {
                    _totalSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalSize)));
                }
            }
        }

        /// <summary>
        /// 下载文件已完成的进度
        /// </summary>
        private double _finishedSize;

        public double FinishedSize
        {
            get { return _finishedSize; }

            set
            {
                if (!Equals(_finishedSize, value))
                {
                    _finishedSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FinishedSize)));
                }
            }
        }

        /// <summary>
        /// 文件下载速度
        /// </summary>
        private double _currentSpeed;

        public double CurrentSpeed
        {
            get { return _currentSpeed; }

            set
            {
                if (!Equals(_currentSpeed, value))
                {
                    _currentSpeed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSpeed)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
