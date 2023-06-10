using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：顶部栏用户控件视图模型
    /// </summary>
    public sealed class HeaderViewModel
    {
        private readonly int MajorVersion = InfoHelper.GetAppVersion().Major;

        private readonly int MinorVersion = InfoHelper.GetAppVersion().Minor;

        private readonly int BuildVersion = InfoHelper.GetAppVersion().Build;

        private readonly int RevisionVersion = InfoHelper.GetAppVersion().Revision;

        public string AppVersion => string.Format("{0}.{1}.{2}.{3}", MajorVersion, MinorVersion, BuildVersion, RevisionVersion);

        /// <summary>
        /// 检查更新
        /// </summary>
        public async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        }

        /// <summary>
        /// 开发者个人信息
        /// </summary>
        public async void OnDeveloperDescriptionClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011"));
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        public async void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        public async void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/issues"));
        }
    }
}
