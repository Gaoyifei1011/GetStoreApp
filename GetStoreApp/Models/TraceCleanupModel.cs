using GetStoreApp.Extensions.DataType.Enums;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 清理选项数据模型
    /// </summary>
    internal sealed partial class TraceCleanupModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 标志该清理选项是否清理失败
        /// </summary>
        private bool _isCleanFailed;

        internal bool IsCleanFailed
        {
            get { return _isCleanFailed; }

            set
            {
                if (!Equals(_isCleanFailed, value))
                {
                    _isCleanFailed = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsCleanFailed)));
                }
            }
        }

        /// <summary>
        /// 清理选项显示的名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 清理选项内部的名称
        /// </summary>
        internal CleanKind InternalName { get; set; }

        /// <summary>
        /// 清理失败时详细的错误文字信息
        /// </summary>
        internal string CleanFailedText { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
