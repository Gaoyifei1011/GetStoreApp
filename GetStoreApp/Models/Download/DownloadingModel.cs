using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Download
{
    public class DownloadingModel : ModelBase
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
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 任务在下载状态时，获取的GID码。该值唯一
        /// </summary>
        public string GID
        {
            get { return (string)GetValue(GIDProperty); }
            set { SetValue(GIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GIDProperty =
            DependencyProperty.Register("GID", typeof(string), typeof(DownloadingModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 下载任务的唯一标识码，该值唯一
        /// </summary>
        public string DownloadKey
        {
            get { return (string)GetValue(DownloadKeyProperty); }
            set { SetValue(DownloadKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DownloadKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DownloadKeyProperty =
            DependencyProperty.Register("DownloadKey", typeof(string), typeof(DownloadingModel), new PropertyMetadata(string.Empty));

        /*
        2.下载文件的基础信息
        */

        /// <summary>
        /// 下载文件名称
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(DownloadingModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 文件下载链接
        /// </summary>
        public string FileLink
        {
            get { return (string)GetValue(FileLinkProperty); }
            set { SetValue(FileLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileLink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileLinkProperty =
            DependencyProperty.Register("FileLink", typeof(string), typeof(DownloadingModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(DownloadingModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 文件SHA1值，用来校验文件
        /// </summary>
        public string FileSHA1
        {
            get { return (string)GetValue(FileSHA1Property); }
            set { SetValue(FileSHA1Property, value); }
        }

        // Using a DependencyProperty as the backing store for FileSHA1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileSHA1Property =
            DependencyProperty.Register("FileSHA1", typeof(string), typeof(DownloadingModel), new PropertyMetadata(string.Empty));

        /*
        3.下载文件的状态信息（动态呈现在UI界面上）
        */

        /// <summary>
        /// 文件下载标志：0为下载失败，1为等待下载，2为暂停下载，3为正在下载，4为成功下载
        /// </summary>
        private int _downloadFlag;

        public int DownloadFlag
        {
            get { return _downloadFlag; }

            set
            {
                _downloadFlag = value;
                OnPropertyChanged();
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
                _totalSize = value;
                OnPropertyChanged();
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
                _finishedSize = value;
                OnPropertyChanged();
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
                _currentSpeed = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 用来标志文件是否处于正在下载状态还是未确定状态
        /// </summary>
        private bool _isFileDownloading;

        public bool IsFileDownloading
        {
            get { return _isFileDownloading; }

            set
            {
                _isFileDownloading = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 计算当前文件的下载进度（如果文件暂未下载，修改下载文件的进度显示状态为未确定）
        /// </summary>
        public double DownloadProgress(double finishedSize, double totalSize)
        {
            if (totalSize == default)
            {
                IsFileDownloading = false;
                return default;
            }
            else
            {
                IsFileDownloading = true;
                return finishedSize / totalSize;
            }
        }
    }
}
