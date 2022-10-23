using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings.Experiment;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.ViewModels.Dialogs
{
    public class ExperimentalConfigViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        private INetWorkMonitorService NetWorkMonitorService = IOCHelper.GetService<INetWorkMonitorService>();

        // 还原默认值
        public IRelayCommand RestoreDefualtCommand => new RelayCommand(async () =>
        {
            await Aria2Service.ReturnDefaultAsync();
            await NetWorkMonitorService.ReturnDefaultValueAsync();
        });

        // 关闭窗口
        public IRelayCommand CloswWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });
    }
}
