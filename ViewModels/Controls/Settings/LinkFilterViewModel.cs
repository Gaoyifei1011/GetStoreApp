using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Messages;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LinkFilterViewModel : ObservableRecipient
    {
        private ILinkFilterService LinkFilterService { get; } = App.GetService<ILinkFilterService>();

        private INavigationService NavigationService { get; } = App.GetService<INavigationService>();

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

        public IAsyncRelayCommand LinkFilterInstructionCommand { get; set; }

        public IAsyncRelayCommand StartWithEFilterCommand { get; set; }

        public IAsyncRelayCommand BlockMapFilterCommand { get; set; }

        public LinkFilterViewModel()
        {
            StartsWithEFilterValue = LinkFilterService.StartWithEFilterValue;
            BlockMapFilterValue = LinkFilterService.BlockMapFilterValue;

            LinkFilterInstructionCommand = new AsyncRelayCommand(async () =>
            {
                NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                await Task.CompletedTask;
            });

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
