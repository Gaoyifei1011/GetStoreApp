using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;

namespace GetStoreApp.UI.Controls.About
{
    /// <summary>
    /// 关于页面：顶部栏控件
    /// </summary>
    public sealed partial class HeaderControl : Grid
    {
        private readonly int MajorVersion = InfoHelper.GetAppVersion().Major;

        private readonly int MinorVersion = InfoHelper.GetAppVersion().Minor;

        private readonly int BuildVersion = InfoHelper.GetAppVersion().Build;

        private readonly int RevisionVersion = InfoHelper.GetAppVersion().Revision;

        public string AppVersion => string.Format("{0}.{1}.{2}.{3}", MajorVersion, MinorVersion, BuildVersion, RevisionVersion);

        public HeaderControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化应用版本信息
        /// </summary>
        private string LocalizeAppVersion(string appVersion)
        {
            return string.Format(ResourceService.GetLocalized("About/AppVersion"), appVersion);
        }

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
