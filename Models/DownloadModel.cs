using Microsoft.UI.Xaml;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    public class DownloadModel : DependencyObject, INotifyPropertyChanged
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

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        /// <summary>
        /// 下载文件创建时对应的时间戳，本地存储时使用的是格林尼治标准时间（GMT+0）
        /// </summary>
        public long CreateTimeStamp
        {
            get { return (long)GetValue(DownloadTimeStampProperty); }
            set { SetValue(DownloadTimeStampProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CreateTimeStamp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DownloadTimeStampProperty =
            DependencyProperty.Register("CreateTimeStamp", typeof(string), typeof(DownloadModel), new PropertyMetadata(0));

        public string DownloadKey
        {
            get { return (string)GetValue(DownloadKeyProperty); }
            set { SetValue(DownloadKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DownloadKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DownloadKeyProperty =
            DependencyProperty.Register("DownloadKey", typeof(string), typeof(DownloadModel), new PropertyMetadata(""));

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
            DependencyProperty.Register("FileName", typeof(string), typeof(DownloadModel), new PropertyMetadata(""));

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
            DependencyProperty.Register("FileLink", typeof(string), typeof(DownloadModel), new PropertyMetadata(""));

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
            DependencyProperty.Register("FilePath", typeof(string), typeof(DownloadModel), new PropertyMetadata(""));

        /// <summary>
        /// 文件SHA1值
        /// </summary>
        public string FileSHA1
        {
            get { return (string)GetValue(FileSHA1Property); }
            set { SetValue(FileSHA1Property, value); }
        }

        // Using a DependencyProperty as the backing store for FileSHA1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileSHA1Property =
            DependencyProperty.Register("FileSHA1", typeof(string), typeof(DownloadModel), new PropertyMetadata(""));

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize
        {
            get { return (string)GetValue(FileSizeProperty); }
            set { SetValue(FileSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileSizeProperty =
            DependencyProperty.Register("FileSize", typeof(string), typeof(DownloadModel), new PropertyMetadata(""));

        /// <summary>
        /// 文件下载标志：0为下载失败，1为正在下载中，2为下载暂停，3为下载失败
        /// </summary>
        private int _downloadFlag;

        public int DownloadFlag
        {
            get { return _downloadFlag; }

            set
            {
                _downloadFlag = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFlag)));
            }
        }

        /// <summary>
        /// 文件下载进度
        /// </summary>
        private int _downloadProgress;

        public int DownloadProgress
        {
            get { return _downloadProgress; }

            set
            {
                _downloadProgress = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadProgress)));
            }
        }

        /// <summary>
        /// 文件下载速度
        /// </summary>
        private bool _downloadSpeed;

        public bool DownloadSpeed
        {
            get { return _downloadSpeed; }

            set
            {
                _downloadSpeed = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadSpeed)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
