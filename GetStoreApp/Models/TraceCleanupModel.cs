using GetStoreApp.Extensions.DataType.Enums;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 清理选项数据模型
    /// </summary>
    public sealed partial class TraceCleanupModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 清理选项是否被选择的标志
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
        /// 标志该清理选项是否清理失败
        /// </summary>
        private bool _isCleanFailed;

        public bool IsCleanFailed
        {
            get { return _isCleanFailed; }

            set
            {
                if (!Equals(_isCleanFailed, value))
                {
                    _isCleanFailed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCleanFailed)));
                }
            }
        }

        /// <summary>
        /// 清理选项显示的名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 清理选项内部的名称
        /// </summary>
        public CleanKind InternalName { get; set; }

        /// <summary>
        /// 清理失败时详细的错误文字信息
        /// </summary>
        public string CleanFailedText { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
