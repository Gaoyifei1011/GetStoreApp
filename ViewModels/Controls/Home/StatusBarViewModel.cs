using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class StatusBarViewModel : ObservableRecipient
    {
        private readonly IResourceService ResourceService;

        private InfoBarSeverity _infoSeverity = InfoBarSeverity.Informational;

        public InfoBarSeverity InfoBarSeverity
        {
            get { return _infoSeverity; }

            set { SetProperty(ref _infoSeverity, value); }
        }

        private string _stateInfoText;

        public string StateInfoText
        {
            get { return _stateInfoText; }

            set { SetProperty(ref _stateInfoText, value); }
        }

        private bool _statePrBarActValue = false;

        public bool StatePrBarActValue
        {
            get { return _statePrBarActValue; }

            set { SetProperty(ref _statePrBarActValue, value); }
        }

        private bool _statePrBarVisValue = false;

        public bool StatePrBarVisValue
        {
            get { return _statePrBarVisValue; }

            set { SetProperty(ref _statePrBarVisValue, value); }
        }

        public List<StatusBarStateModel> StatusBarStateList { get; set; }

        public StatusBarViewModel(IResourceService resourceService)
        {
            ResourceService = resourceService;

            StateInfoText = ResourceService.GetLocalized("/Home/StatusInfoWelcome");

            StatusBarStateList = ResourceService.StatusBarStateList;

            Messenger.Register(this, (MessageHandler<StatusBarViewModel, StatusBarStateMessage>)(async (statusbarViewModel, statusBarStateMessage) =>
                        {
                            statusbarViewModel.InfoBarSeverity = StatusBarStateList[statusBarStateMessage.Value].InfoBarSeverity;
                            statusbarViewModel.StateInfoText = StatusBarStateList[statusBarStateMessage.Value].StateInfoText;
                            statusbarViewModel.StatePrBarVisValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingVisValue;
                            statusbarViewModel.StatePrBarActValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingActValue;
                            await Task.CompletedTask;
                        }));
        }
    }
}
