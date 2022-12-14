using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.Download
{
    public class UnfinishedModel : ModelBase
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
        /// 文件SHA1值，用来校验文件
        /// </summary>
        public string FileSHA1 { get; set; }

        /// <summary>
        /// 文件下载标志：0为下载失败，1为等待下载，2为暂停下载，3为正在下载，4为成功下载
        /// </summary>
        public int DownloadFlag { get; set; }

        /// <summary>
        /// 下载文件的总大小
        /// </summary>
        public double TotalSize { get; set; }
    }
}
