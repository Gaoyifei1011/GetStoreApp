using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// WinGet 数据源编辑对话框
    /// </summary>
    public sealed partial class WinGetSourceEditDialog : ContentDialog, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetSourceEditDialog(WinGetSourceEditKind winGetSourceEditKind, WinGetSourceModel winGetSourceItem)
        {
            InitializeComponent();
            Title = winGetSourceEditKind is WinGetSourceEditKind.Add ? ResourceService.GetLocalized("Dialog/WinGetDataSourceAdd") : ResourceService.GetLocalized("Dialog/WinGetDataSourceEdit");
        }
    }
}
