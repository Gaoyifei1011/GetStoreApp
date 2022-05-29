using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Services.Settings;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LinkFilterViewModel : ObservableRecipient
    {
        // 扩展名“.e”开头文件过滤的状态
        private bool _startsWithEFilterValue = LinkFilterService.LinkFilterValue[0];

        public bool StartsWithEFilterValue
        {
            get { return _startsWithEFilterValue; }

            set
            {
                SetProperty(ref _startsWithEFilterValue, value);
                LinkFilterService.SetStartsWithEFilterValue(value);
                Messenger.Send(new StartsWithEFilterMessage(value));
            }
        }

        // 扩展名“.blockmap”文件过滤的状态
        private bool _blockMapFilterValue = LinkFilterService.LinkFilterValue[1];

        public bool BlockMapFilterValue
        {
            get { return _blockMapFilterValue; }

            set
            {
                SetProperty(ref _blockMapFilterValue, value);
                LinkFilterService.SetBlockMapFilterValue(value);
                Messenger.Send(new BlockMapFilterMessage(value));
            }
        }

        // 链接过滤说明第一个了解按钮命令
        private ICommand _startsWithECommand;

        public ICommand StartsWithECommand
        {
            get { return _startsWithECommand; }

            set { SetProperty(ref _startsWithECommand, value); }
        }

        // 链接过滤说明第二个了解按钮命令
        private ICommand _blockMapCommand;

        public ICommand BlockMapCommand
        {
            get { return _blockMapCommand; }

            set { SetProperty(ref _blockMapCommand, value); }
        }

        public LinkFilterViewModel()
        {
            StartsWithECommand = new RelayCommand(async () => { await LaunchStartsWithEDescriptionAsync(); });

            BlockMapCommand = new RelayCommand(async () => { await LaunchBlockMapDescriptionAsync(); });
        }

        // 链接过滤说明第一个了解按钮打开网页
        private async Task LaunchStartsWithEDescriptionAsync()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://datatypes.net/open-eappx-files#:~:text=EAPPX%20file%20is%20a%20Microsoft%20Windows%20Encrypted%20App,applications%20may%20also%20use%20the%20.eappx%20file%20extension."));
        }

        // 链接过滤说明第一个了解按钮打开网页
        private async Task LaunchBlockMapDescriptionAsync()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://docs.microsoft.com/en-us/uwp/schemas/blockmapschema/app-package-block-map"));
        }
    }
}