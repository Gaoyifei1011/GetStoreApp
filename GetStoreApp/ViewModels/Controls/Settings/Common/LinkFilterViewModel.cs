using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public sealed class LinkFilterViewModel : ViewModelBase
    {
        private bool _startsWithEFilterValue;

        public bool StartsWithEFilterValue
        {
            get { return _startsWithEFilterValue; }

            set
            {
                _startsWithEFilterValue = value;
                OnPropertyChanged();
            }
        }

        private bool _blockMapFilterValue;

        public bool BlockMapFilterValue
        {
            get { return _blockMapFilterValue; }

            set
            {
                _blockMapFilterValue = value;
                OnPropertyChanged();
            }
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
