using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 查询链接结果的数据模型
    /// </summary>
    public sealed partial class QueryLinksResultModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 是否处于多选模式
        /// </summary>
        private ListViewSelectionMode _selectionMode;

        public ListViewSelectionMode SelectionMode
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
        /// 文件SHA256值
        /// </summary>
        public string FileSHA256 { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
