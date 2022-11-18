using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.UI.Dialogs.ContentDialogs.Settings;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public sealed class ExperimentalFeaturesViewModel
    {
        // 实验功能设置
        public IRelayCommand ConfigCommand => new RelayCommand(async () =>
        {
            await new ExperimentalConfigDialog().ShowAsync();
        });
    }
}
