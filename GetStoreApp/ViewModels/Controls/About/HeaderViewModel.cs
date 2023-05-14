using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：顶部栏用户控件视图模型
    /// </summary>
    public sealed class HeaderViewModel : ViewModelBase
    {
        private readonly ushort MajorVersion = InfoHelper.GetAppVersion().MajorVersion;

        private readonly ushort MinorVersion = InfoHelper.GetAppVersion().MinorVersion;

        private readonly ushort BuildVersion = InfoHelper.GetAppVersion().BuildVersion;

        private readonly ushort RevisionVersion = InfoHelper.GetAppVersion().RevisionVersion;

        private string _appVersion;

        public string AppVersion
        {
            get { return _appVersion; }

            set
            {
                _appVersion = value;
                OnPropertyChanged();
            }
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
        /// 初始化应用版本信息
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            AppVersion = string.Format("{0}.{1}.{2}.{3}", MajorVersion, MinorVersion, BuildVersion, RevisionVersion);
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
