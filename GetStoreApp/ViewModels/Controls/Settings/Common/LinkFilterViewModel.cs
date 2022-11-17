using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Controls.Settings.Common;
using GetStoreApp.Contracts.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Views.Pages;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public class LinkFilterViewModel : ObservableRecipient
    {
        private ILinkFilterService LinkFilterService { get; } = ContainerHelper.GetInstance<ILinkFilterService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        private bool _startsWithEFilterValue;

        public bool StartsWithEFilterValue
        {
            get { return _startsWithEFilterValue; }

            set { SetProperty(ref _startsWithEFilterValue, value); }
        }

        private bool _blockMapFilterValue;

        public bool BlockMapFilterValue
        {
            get { return _blockMapFilterValue; }

            set { SetProperty(ref _blockMapFilterValue, value); }
        }

        // 链接过滤说明
        public IRelayCommand LinkFilterInstructionCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutPage));
        });

        // 设置是否过滤以“.e”开头的文件
        public IRelayCommand StartWithEFilterCommand => new RelayCommand<bool>(async (startWithEFilterValue) =>
        {
            await LinkFilterService.SetStartsWithEFilterValueAsync(startWithEFilterValue);
            StartsWithEFilterValue = startWithEFilterValue;
        });

        // 设置是否过滤包块映射文件
        public IRelayCommand BlockMapFilterCommand => new RelayCommand<bool>(async (blockMapFilterValue) =>
        {
            await LinkFilterService.SetBlockMapFilterValueAsync(blockMapFilterValue);
            BlockMapFilterValue = blockMapFilterValue;
        });

        public LinkFilterViewModel()
        {
            StartsWithEFilterValue = LinkFilterService.StartWithEFilterValue;
            BlockMapFilterValue = LinkFilterService.BlockMapFilterValue;
        }
    }
}
