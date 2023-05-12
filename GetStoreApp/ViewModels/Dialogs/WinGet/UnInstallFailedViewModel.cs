using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Windows.System;

namespace GetStoreApp.ViewModels.Dialogs.WinGet
{
    /// <summary>
    /// 卸载失败提示对话框视图模型
    /// </summary>
    public sealed class UnInstallFailedViewModel
    {
        public string UnInstallFailedContent { get; set; }

        // 打开应用和功能卸载应用
        public IRelayCommand OpenSettingsCommand => new RelayCommand<ExtendedContentDialog>(async (dialog) =>
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
            dialog.Hide();
        });

        // 关闭对话框
        public IRelayCommand CloseDialogCommand => new RelayCommand<ExtendedContentDialog>((dialog) =>
        {
            dialog.Hide();
        });
    }
}
