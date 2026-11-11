using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 查询链接结果的数据模型
    /// </summary>
    internal sealed partial class QueryLinksResultModel : INotifyPropertyChanged
    {
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
        /// 文件名称
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// 文件下载链接
        /// </summary>
        internal string FileLink { get; set; }

        /// <summary>
        /// 文件SHA256值
        /// </summary>
        internal string FileSHA256 { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        internal string FileSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
