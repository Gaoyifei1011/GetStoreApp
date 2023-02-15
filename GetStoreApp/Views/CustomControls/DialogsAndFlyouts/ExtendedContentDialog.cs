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
                XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
                dialogResult = await base.ShowAsync();
                IsDialogOpening = false;
            }
            return dialogResult;
        }
    }
}
