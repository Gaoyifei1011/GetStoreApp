using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace GetStoreApp.ViewModels.Pages
{
    public class DownloadViewModel : ObservableRecipient
    {
        private IUseInstructionService UseInstructionService { get; } = ContainerHelper.GetInstance<IUseInstructionService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set { SetProperty(ref _useInsVisValue, value); }
        }

        // 了解更多下载管理说明
        public IRelayCommand LearnMoreCommand => new RelayCommand<TeachingTip>((downloadTeachingTip) =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            downloadTeachingTip.IsOpen = false;
            NavigationService.NavigateTo(typeof(AboutPage));
        });

        // 打开应用“下载设置”
        public IRelayCommand DownloadSettingsCommand => new RelayCommand<TeachingTip>((downloadTeachingTip) =>
        {
            App.NavigationArgs = AppNaviagtionArgs.DownloadOptions;
            downloadTeachingTip.IsOpen = false;
            NavigationService.NavigateTo(typeof(SettingsPage));
        });

        // DownloadPivot选中项发生变化时，关闭离开页面的事件，开启要导航到的页面的事件，并更新新页面的数据
        public IRelayCommand DownloadContentCommand => new RelayCommand<int>((value) =>
        {
            WeakReferenceMessenger.Default.Send(new PivotSelectionMessage(value));
        });

        // 初次加载页面时，开启下载中页面的所有事件，加载下载中页面的数据
        public void OnNavigatedTo()
        {
            UseInsVisValue = UseInstructionService.UseInsVisValue;
            WeakReferenceMessenger.Default.Send(new PivotSelectionMessage(0));
        }

        // 离开该页面时，关闭所有事件，并通知注销所有事件（防止内存泄露）
        public void OnNavigatedFrom()
        {
            WeakReferenceMessenger.Default.Send(new PivotSelectionMessage(-1));
        }

        public DownloadViewModel()
        {
            UseInsVisValue = UseInstructionService.UseInsVisValue;
        }
    }
}
