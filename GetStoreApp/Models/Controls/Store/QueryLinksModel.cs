using System.ComponentModel;

namespace GetStoreApp.Models.Controls.Store
{
    /// <summary>
    /// 查询链接返回结果的数据模型
    /// </summary>
    public class QueryLinksModel : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
