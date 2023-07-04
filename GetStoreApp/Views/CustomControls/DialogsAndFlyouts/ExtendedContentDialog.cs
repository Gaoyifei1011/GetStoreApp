using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Views.CustomControls.DialogsAndFlyouts
{
    /// <summary>
    /// 扩展后的内容对话框，只允许在同一时间段内打开一个内容对话框
    /// </summary>
    public class ExtendedContentDialog : ContentDialog
    {
        private static bool IsDialogOpening { get; set; } = false;

        public new async Task<ContentDialogResult> ShowAsync()
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            if (!IsDialogOpening)
            {
                IsDialogOpening = true;
                RequestedTheme = Program.ApplicationRoot.MainWindow.ViewModel.WindowTheme;
                XamlRoot = Program.ApplicationRoot.MainWindow.Content.XamlRoot;
                Program.ApplicationRoot.MainWindow.ViewModel.PropertyChanged += OnPropertyChanged;
                dialogResult = await base.ShowAsync();
                Program.ApplicationRoot.MainWindow.ViewModel.PropertyChanged -= OnPropertyChanged;
                IsDialogOpening = false;
            }
            return dialogResult;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            RequestedTheme = Program.ApplicationRoot.MainWindow.ViewModel.WindowTheme;
        }
    }
}
