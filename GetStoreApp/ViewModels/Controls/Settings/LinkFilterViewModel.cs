using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
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

        public IAsyncRelayCommand LinkFilterInstructionCommand => new AsyncRelayCommand(async () =>
        {
            App.NavigationArgs = "SettingsHelp";
            NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand StartWithEFilterCommand => new AsyncRelayCommand<bool>(async (param) =>
        {
            await LinkFilterService.SetStartsWithEFilterValueAsync(param);
            WeakReferenceMessenger.Default.Send(new StartsWithEFilterMessage(param));
            StartsWithEFilterValue = param;
        });

        public IAsyncRelayCommand BlockMapFilterCommand => new AsyncRelayCommand<bool>(async (param) =>
        {
            await LinkFilterService.SetBlockMapFilterValueAsync(param);
            WeakReferenceMessenger.Default.Send(new BlockMapFilterMessage(param));
            BlockMapFilterValue = param;
        });

        public LinkFilterViewModel()
        {
            StartsWithEFilterValue = LinkFilterService.StartWithEFilterValue;
            BlockMapFilterValue = LinkFilterService.BlockMapFilterValue;
        }
    }
}
