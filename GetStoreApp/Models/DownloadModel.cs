using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;
using System.IO;

namespace GetStoreApp.Models
{
    public class DownloadModel : ModelBase
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
        private string _gID;

        public string GID
        {
            get { return _gID; }
            set { _gID = value; }
        }

        /// <summary>
        /// 下载任务的唯一标识码，该值唯一
        /// </summary>
        private string _downloadKey;

        public string DownloadKey
        {
            get { return _downloadKey; }
            set { _downloadKey = value; }
        }

        /*
        2.下载文件的基础信息
        */

        /// <summary>
        /// 下载文件名称
        /// </summary>
        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// 文件下载链接
        /// </summary>
        private string _fileLink;

        public string FileLink
        {
            get { return _fileLink; }
            set { _fileLink = value; }
        }

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        /// <summary>
        /// 文件SHA1值，用来校验文件是否正确下载
        /// </summary>
        private string _fileSHA1;

        public string FileSHA1
        {
            get { return _fileSHA1; }
            set { _fileSHA1 = value; }
        }

        /*
        3.下载文件的状态信息
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
        private int _totalSize;

        public int TotalSize
        {
            get { return _totalSize; }

            set { _totalSize = value; }
        }

        /// <summary>
        /// 下载文件已完成的进度
        /// </summary>
        private int _finishedSize;

        public int FinishedSize
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
        private int _currentSpeed;

        public int CurrentSpeed
        {
            get { return _currentSpeed; }

            set
            {
                _currentSpeed = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 计算当前文件的下载进度
        /// </summary>
        public double DownloadProgress(int finishedSize, int totalSize)
        {
            if(totalSize == 0)
            {
                return 0.0;
            }

            return finishedSize / totalSize;
        }

        /// <summary>
        /// 判断已下载完成的文件是否存在
        /// </summary>
        public Visibility IsFileExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }
    }
}
