using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.Models.Controls.Store
{
    /// <summary>
    /// 历史记录数据模型
    /// </summary>
    public sealed partial class HistoryModel : INotifyPropertyChanged
    {
        private bool _isQuerying;

        public bool IsQuerying
        {
            get { return _isQuerying; }

            set
            {
                if (!Equals(_isQuerying, value))
                {
                    _isQuerying = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsQuerying)));
                }
            }
        }

        /// <summary>
        /// 共用：历史记录生成时对应的时间戳，本地存储时使用的是格林尼治标准时间（GMT+0）
        /// </summary>
        public long CreateTimeStamp { get; set; }

        /// <summary>
        /// 共用：历史记录的索引键值
        /// </summary>
        public string HistoryKey { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录的应用名称或类别ID
        /// </summary>
        public string HistoryAppName { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录中包含的类型，数据库存储的原始名称
        /// </summary>
        public KeyValuePair<string, string> HistoryType { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录中包含的通道
        /// </summary>
        public KeyValuePair<string, string> HistoryChannel { get; set; }

        /// <summary>
        /// 查询链接控件：历史记录包含的链接
        /// </summary>
        public string HistoryLink { get; set; }

        /// <summary>
        /// 搜索应用控件：历史记录的内容
        /// </summary>
        public string HistoryContent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
