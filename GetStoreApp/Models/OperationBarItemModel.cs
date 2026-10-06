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
        private OperationBarKind _operationBarKind;

        internal OperationBarKind OperationBarKind
        {
            get { return _operationBarKind; }

            set
            {
                if (!Equals(_operationBarKind, value))
                {
                    _operationBarKind = value;
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
        private bool _isCheckingUpdate;

        internal bool IsCheckingUpdate
        {
            get { return _isCheckingUpdate; }

            set
            {
                if (!Equals(_isCheckingUpdate, value))
                {
                    _isCheckingUpdate = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsCheckingUpdate)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
