using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Dialogs
{
    public sealed partial class WinGetAppsVersionDialog : ContentDialog
    {
        public WinGetAppsVersionDialog(WinGetOptionKind winGetOptionKind, WinGetPage winGetPage, SearchAppsModel searchApps)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Hide();
        }
    }
}
