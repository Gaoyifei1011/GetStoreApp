using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public class LinkFilterViewModel : ObservableRecipient
    {
        private ILinkFilterService LinkFilterService { get; } = IOCHelper.GetService<ILinkFilterService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

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
            NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
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
