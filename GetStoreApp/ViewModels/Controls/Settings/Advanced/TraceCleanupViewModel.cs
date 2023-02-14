using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Settings;
using GetStoreApp.Views.Pages;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：痕迹清理设置用户控件视图模型
    /// </summary>
    public sealed class TraceCleanupViewModel
    {
        // 痕迹清理说明
        public IRelayCommand TraceCleanupTipCommand => new RelayCommand(() =>
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        });

        // 清理应用内使用的所有痕迹
        public IRelayCommand TraceCleanupCommand = new RelayCommand(async () =>
        {
            await new TraceCleanupPromptDialog().ShowAsync();
        });
    }
}
