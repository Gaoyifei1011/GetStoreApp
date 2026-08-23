using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 历史记录数据模型
    /// </summary>
    internal sealed partial class HistoryModel : INotifyPropertyChanged
    {
        private bool _isQuerying;

        internal bool IsQuerying
        {
            get { return _isQuerying; }

            set
            {
                if (!Equals(_isQuerying, value))
                {
                    _isQuerying = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsQuerying)));
                }
            }
        }

        /// <summary>
        /// 共用：历史记录生成时对应的时间戳，本地存储时使用的是格林尼治标准时间（GMT+0）
        /// </summary>
        internal long CreateTimeStamp { get; set; }

        /// <summary>
        /// 共用：历史记录的索引键值
        /// </summary>
        internal string HistoryKey { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录的应用名称或类别ID
        /// </summary>
        internal string HistoryAppName { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录中包含的类型，数据库存储的原始名称
        /// </summary>
        internal string HistoryType { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录中包含的类型，数据库存储的显示名称
        /// </summary>
        internal string HistoryTypeName { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录中包含的通道
        /// </summary>
        internal string HistoryChannel { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录中包含的通道显示名称
        /// </summary>
        internal string HistoryChannelName { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录包含的链接
        /// </summary>
        internal string HistoryLink { get; set; }

        /// <summary>
        /// 搜索应用控件：历史记录的内容
        /// </summary>
        internal string HistoryContent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
