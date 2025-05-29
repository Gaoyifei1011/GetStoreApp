using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 删除提示对话框
    /// </summary>
    public sealed partial class DeletePromptDialog : ContentDialog, INotifyPropertyChanged
    {
        private DeleteKind _deleteKind;

        public DeleteKind DeleteKind
        {
            get { return _deleteKind; }

            set
            {
                if (!Equals(_deleteKind, value))
                {
                    _deleteKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeleteKind)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DeletePromptDialog(DeleteKind deleteKind)
        {
            InitializeComponent();
            DeleteKind = deleteKind;
        }

        private Visibility GetDeleteKindState(DeleteKind deleteKind, DeleteKind comparedDeleteKind)
        {
            return Equals(deleteKind, comparedDeleteKind) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
