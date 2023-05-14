using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Dialogs.WinGet
{
    /// <summary>
    /// 卸载失败提示对话框视图模型
    /// </summary>
    public sealed class UnInstallFailedViewModel
    {
        public string UnInstallFailedContent { get; set; }

        /// <summary>
        /// 打开设置
        /// </summary>
        public async void OpenSettings()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        }
    }
}
