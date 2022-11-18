using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public sealed class IntroductionViewModel
    {
        // 查看项目后续的更新 / 维护信息
        public IRelayCommand MaintenanceCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        });
    }
}
