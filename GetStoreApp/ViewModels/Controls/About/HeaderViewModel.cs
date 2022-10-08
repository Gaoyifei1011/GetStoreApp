using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Helpers;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class HeaderViewModel : ObservableRecipient
    {
        private readonly ushort MajorVersion = InfoHelper.GetAppVersion()["MajorVersion"];

        private readonly ushort MinorVersion = InfoHelper.GetAppVersion()["MinorVersion"];

        private readonly ushort BuildVersion = InfoHelper.GetAppVersion()["BuildVersion"];

        private readonly ushort RevisionVersion = InfoHelper.GetAppVersion()["RevisionVersion"];

        private string _appVersion;

        public string AppVersion
        {
            get { return _appVersion; }

            set { SetProperty(ref _appVersion, value); }
        }

        // 开发者个人信息
        public IRelayCommand DeveloperDescriptionCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011"));
        });

        // 项目主页
        public IRelayCommand ProjectDescriptionCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        });

        // 发送反馈
        public IRelayCommand SendFeedbackCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/issues"));
        });

        // 检查更新
        public IRelayCommand CheckUpdateCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        });

        public HeaderViewModel()
        {
            AppVersion = string.Format("{0}.{1}.{2}.{3}", MajorVersion, MinorVersion, BuildVersion, RevisionVersion);
        }
    }
}
