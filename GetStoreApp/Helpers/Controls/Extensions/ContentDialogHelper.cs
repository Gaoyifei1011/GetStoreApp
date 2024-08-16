using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Helpers.Controls.Extensions
{
    /// <summary>
    /// 扩展后的内容对话框控件辅助类
    /// </summary>
    public static class ContentDialogHelper
    {
        private static bool isDialogOpening = false;

        /// <summary>
        /// 显示对话框
        /// </summary>
        public static async Task<ContentDialogResult> ShowAsync(ContentDialog dialog, FrameworkElement element)
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            if (!isDialogOpening && dialog is not null && element is not null)
            {
                isDialogOpening = true;
                dialog.XamlRoot = element.XamlRoot;
                dialog.RequestedTheme = element.ActualTheme;
                element.ActualThemeChanged += (sender, args) =>
                {
                    dialog.RequestedTheme = element.ActualTheme;
                };
                dialogResult = await dialog.ShowAsync();
                isDialogOpening = false;
            }
            return dialogResult;
        }
    }
}
