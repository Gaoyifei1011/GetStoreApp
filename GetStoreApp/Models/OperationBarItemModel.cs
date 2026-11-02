using GetStoreApp.Extensions.DataType.Enums;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 操作栏项目数据模型
    /// </summary>
    internal sealed partial class OperationBarItemModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 操作栏类型
        /// </summary>
        internal OperationBarKind OperationBarKind
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(OperationBarKind)));
                }
            }
        }

        /// <summary>
        /// 设置关于页面操作栏标题
        /// </summary>
        internal string Title { get; set; }

        /// <summary>
        /// 设置关于页面操作栏标题
        /// </summary>
        internal string Tag { get; set; }

        /// <summary>
        /// 设置关于页面操作栏图标字符代码
        /// </summary>
        internal string IconGlyph { get; set; }

        /// <summary>
        /// 是否正在检查更新
        /// </summary>
        internal bool IsCheckingUpdate
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsCheckingUpdate)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
