using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace GetStoreApp.UI.Dialogs.About
{
    /// <summary>
    /// 如何识别桌面传统应用提示对话框
    /// </summary>
    public sealed partial class DesktopAppsDialog : ContentDialog
    {
        private BitmapImage DesktopAppsImage { get; } = new BitmapImage();

        public DesktopAppsDialog()
        {
            InitializeComponent();
            DesktopAppsImage.UriSource = new Uri(string.Format("ms-appx:///Assets/Images/{0}/DesktopApps.png", ActualTheme.ToString()));
        }
    }
}
