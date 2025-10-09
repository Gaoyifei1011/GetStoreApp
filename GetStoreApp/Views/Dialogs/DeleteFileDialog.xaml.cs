using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 删除文件对话框
    /// </summary>
    public sealed partial class DeleteFileDialog : ContentDialog, INotifyPropertyChanged
    {
        private bool _deleteFileSameTime;

        public bool DeleteFileSameTime
        {
            get { return _deleteFileSameTime; }

            set
            {
                if (!Equals(_deleteFileSameTime, value))
                {
                    _deleteFileSameTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeleteFileSameTime)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DeleteFileDialog()
        {
            InitializeComponent();
        }
    }
}
