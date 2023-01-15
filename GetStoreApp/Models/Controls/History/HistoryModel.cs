using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.History
{
    /// <summary>
    /// 历史记录数据模型
    /// </summary>
    public class HistoryModel : ModelBase
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
        /// 历史记录生成时对应的时间戳，本地存储时使用的是格林尼治标准时间（GMT+0）
        /// </summary>
        public long CreateTimeStamp { get; set; }

        /// <summary>
        /// 历史记录的索引键值
        /// </summary>
        public string HistoryKey { get; set; }

        /// <summary>
        /// 历史记录中包含的类型，数据库存储的原始名称
        /// </summary>
        public string HistoryType { get; set; }

        /// <summary>
        /// 历史记录中包含的通道
        /// </summary>
        public string HistoryChannel { get; set; }

        /// <summary>
        /// 历史记录包含的链接
        /// </summary>
        public string HistoryLink { get; set; }
    }
}
