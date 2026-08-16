using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 删除文件对话框
    /// </summary>
    internal sealed partial class DeleteFileDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：属性、集合与事件

        private bool _deleteFileSameTime;

        internal bool DeleteFileSameTime
        {
            get { return _deleteFileSameTime; }

            set
            {
                if (!Equals(_deleteFileSameTime, value))
                {
                    _deleteFileSameTime = value;
                    PropertyChanged?.Invoke(this, new(nameof(DeleteFileSameTime)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第一部分：属性、集合与事件

        #region 第二部分：构造函数

        internal DeleteFileDialog()
        {
            InitializeComponent();
        }

        #endregion 第二部分：构造函数
    }
}
