using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.UI.Dialogs.Settings;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：实验性功能设置用户控件视图模型
    /// </summary>
    public sealed class ExperimentalFeaturesViewModel
    {
        // 实验功能设置
        public IRelayCommand ConfigCommand => new RelayCommand(async () =>
        {
            await new ExperimentalConfigDialog().ShowAsync();
        });
    }
}
