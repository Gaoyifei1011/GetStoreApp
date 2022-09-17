using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Contracts.ViewModels;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Pages
{
    public class DownloadViewModel : ObservableRecipient, INavigationAware
    {
        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        // 了解更多下载管理说明
        public IAsyncRelayCommand LearnMoreCommand => new AsyncRelayCommand<TeachingTip>(async (param) =>
        {
            App.NavigationArgs = "SettingsHelp";
            param.IsOpen = false;
            NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        });

        // 打开应用“下载设置”
        public IAsyncRelayCommand DownloadSettingsCommand => new AsyncRelayCommand<TeachingTip>(async (param) =>
        {
            App.NavigationArgs = "DownloadOptions";
            param.IsOpen = false;
            NavigationService.NavigateTo(typeof(SettingsViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        });

        // DownloadPivot选中项发生变化时，关闭离开页面的事件，开启要导航到的页面的事件，并更新新页面的数据
        public IAsyncRelayCommand DownloadContentCommand => new AsyncRelayCommand<int>(async (param) =>
        {
            WeakReferenceMessenger.Default.Send(new PivotSelectionMessage(param));
            await Task.CompletedTask;
        });

        // 初次加载页面时，开启下载中页面的所有事件，加载下载中页面的数据
        public void OnNavigatedTo(object parameter)
        {
            WeakReferenceMessenger.Default.Send(new PivotSelectionMessage(0));
        }

        // 离开该页面时，关闭所有事件，并通知注销所有事件（防止内存泄露）
        public void OnNavigatedFrom()
        {
            WeakReferenceMessenger.Default.Send(new PivotSelectionMessage(-1));
        }
    }
}
