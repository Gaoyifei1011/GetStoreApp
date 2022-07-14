using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Messages;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LinkFilterViewModel : ObservableRecipient
    {
        private readonly ILinkFilterService LinkFilterService;

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

        public IAsyncRelayCommand StartWithEFilterCommand { get; set; }

        public IAsyncRelayCommand BlockMapFilterCommand { get; set; }

        public IAsyncRelayCommand StartsWithECommand { get; set; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://datatypes.net/open-eappx-files#:~:text=EAPPX%20file%20is%20a%20Microsoft%20Windows%20Encrypted%20App,applications%20may%20also%20use%20the%20.eappx%20file%20extension."));
        });

        public IAsyncRelayCommand BlockMapCommand { get; set; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://docs.microsoft.com/en-us/uwp/schemas/blockmapschema/app-package-block-map"));
        });

        public LinkFilterViewModel(ILinkFilterService linkFilterService)
        {
            LinkFilterService = linkFilterService;

            StartsWithEFilterValue = LinkFilterService.StartWithEFilterValue;
            BlockMapFilterValue = LinkFilterService.BlockMapFilterValue;

            StartWithEFilterCommand = new AsyncRelayCommand<bool>(async (param) =>
            {
                await LinkFilterService.SetStartsWithEFilterValueAsync(param);
                Messenger.Send(new StartsWithEFilterMessage(param));
                StartsWithEFilterValue = param;
            });

            BlockMapFilterCommand = new AsyncRelayCommand<bool>(async (param) =>
            {
                await LinkFilterService.SetBlockMapFilterValueAsync(param);
                Messenger.Send(new BlockMapFilterMessage(param));
                BlockMapFilterValue = param;
            });
        }
    }
}
