using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.System;

namespace GetStoreApp.ViewModels.Dialogs.WinGet
{
    /// <summary>
    /// 安装失败提示对话框视图
    /// </summary>
    public class InstalledFailedViewModel
    {
        public string InstallFailedContent { get; set; }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        public void OnCloseDialogClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            if (button.Tag is not null)
            {
                ((ExtendedContentDialog)button.Tag).Hide();
            }
        }

        /// <summary>
        /// 打开下载目录，让用户手动安装应用
        /// </summary>
        public async void OpenDownloadFolderClicked(object sender, RoutedEventArgs args)
        {
            string wingetTempPath = Path.Combine(Path.GetTempPath(), "WinGet");
            if (Directory.Exists(wingetTempPath))
            {
                await Launcher.LaunchFolderPathAsync(wingetTempPath);
            }
            else
            {
                await Launcher.LaunchFolderPathAsync(Path.GetTempPath());
            }
        }
    }
}
