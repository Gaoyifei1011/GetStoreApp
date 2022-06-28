using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Services.Settings;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LinkFilterViewModel : ObservableRecipient
    {
        private bool _startsWithEFilterValue = LinkFilterService.LinkFilterValue[0];

        public bool StartsWithEFilterValue
        {
            get { return _startsWithEFilterValue; }

            set { SetProperty(ref _startsWithEFilterValue, value); }
        }

        private bool _blockMapFilterValue = LinkFilterService.LinkFilterValue[1];

        public bool BlockMapFilterValue
        {
            get { return _blockMapFilterValue; }

            set { SetProperty(ref _blockMapFilterValue, value); }
        }

        public IAsyncRelayCommand StartsWithECommand { get; set; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://datatypes.net/open-eappx-files#:~:text=EAPPX%20file%20is%20a%20Microsoft%20Windows%20Encrypted%20App,applications%20may%20also%20use%20the%20.eappx%20file%20extension."));
        });

        public IAsyncRelayCommand BlockMapCommand { get; set; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://docs.microsoft.com/en-us/uwp/schemas/blockmapschema/app-package-block-map"));
        });

        public IAsyncRelayCommand StartWithEFilterCommand { get; set; }

        public IAsyncRelayCommand BlockMapFilterCommand { get; set; }

        public LinkFilterViewModel()
        {
            StartWithEFilterCommand = new AsyncRelayCommand(StartWithEFilterAsync);

            BlockMapFilterCommand = new AsyncRelayCommand(BlockMapFilterAsync);
        }

        public async Task StartWithEFilterAsync()
        {
            LinkFilterService.SetStartsWithEFilterValue(StartsWithEFilterValue);
            Messenger.Send(new StartsWithEFilterMessage(StartsWithEFilterValue));
            await Task.CompletedTask;
        }

        private async Task BlockMapFilterAsync()
        {
            LinkFilterService.SetBlockMapFilterValue(BlockMapFilterValue);
            Messenger.Send(new BlockMapFilterMessage(BlockMapFilterValue));
            await Task.CompletedTask;
        }
    }
}
