using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class IntroductionViewModel : ObservableRecipient
    {
        // 查看项目后续的更新 / 维护信息
        public IRelayCommand MaintenanceCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        });
    }
}
