using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.Home
{
    public class ResultModel : ModelBase
    {
        /// <summary>
        /// 在多选模式下，该行信息是否被选择的标志
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
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件下载链接
        /// </summary>
        public string FileLink { get; set; }

        /// <summary>
        /// 文件下载链接过期时间
        /// </summary>
        public string FileLinkExpireTime { get; set; }

        /// <summary>
        /// 文件SHA1值
        /// </summary>
        public string FileSHA1 { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }
    }
}
