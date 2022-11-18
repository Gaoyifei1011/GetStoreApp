using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.UI.Dialogs.ContentDialogs.Settings;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public sealed class TraceCleanupViewModel
    {
        // 清理应用内使用的所有痕迹
        public IRelayCommand TraceCleanupCommand = new RelayCommand(async () =>
        {
            await new TraceCleanupPromptDialog().ShowAsync();
        });
    }
}
