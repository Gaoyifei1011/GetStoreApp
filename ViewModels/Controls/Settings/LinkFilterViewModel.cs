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

            set
            {
                SetProperty(ref _startsWithEFilterValue, value);
                LinkFilterService.SetStartsWithEFilterValue(value);
                Messenger.Send(new StartsWithEFilterMessage(value));
            }
        }

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

        public IAsyncRelayCommand StartsWithECommand { get; set; }

        public IAsyncRelayCommand BlockMapCommand { get; set; }

        public LinkFilterViewModel()
        {
            StartsWithECommand = new AsyncRelayCommand(LaunchStartsWithEDescriptionAsync);

            BlockMapCommand = new AsyncRelayCommand(LaunchBlockMapDescriptionAsync);
        }

        private async Task LaunchStartsWithEDescriptionAsync()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://datatypes.net/open-eappx-files#:~:text=EAPPX%20file%20is%20a%20Microsoft%20Windows%20Encrypted%20App,applications%20may%20also%20use%20the%20.eappx%20file%20extension."));
        }

        private async Task LaunchBlockMapDescriptionAsync()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://docs.microsoft.com/en-us/uwp/schemas/blockmapschema/app-package-block-map"));
        }
    }
}
