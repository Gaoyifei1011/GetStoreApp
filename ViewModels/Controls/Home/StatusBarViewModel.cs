using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class StatusBarViewModel : ObservableRecipient
    {
        private InfoBarSeverity _infoSeverity = InfoBarSeverity.Informational;

        public InfoBarSeverity InfoBarSeverity
        {
            get { return _infoSeverity; }

            set { SetProperty(ref _infoSeverity, value); }
        }

        private string _stateInfoText = LanguageService.GetResources("/Home/StatusInfoWelcome");

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

        public static IReadOnlyList<StatusBarState> StatusBarStateList { get; } = new List<StatusBarState>()
        {
            new StatusBarState(){
                InfoBarSeverity=InfoBarSeverity.Informational,
                StateInfoText=LanguageService.GetResources("/Home/StatusInfoGetting"),
                StatePrRingActValue=true,
                StatePrRingVisValue=true
            },
            new StatusBarState()
            {
                InfoBarSeverity=InfoBarSeverity.Success,
                StateInfoText=LanguageService.GetResources("/Home/StatusInfoSuccess"),
                StatePrRingActValue=false,
                StatePrRingVisValue=false
            },
            new StatusBarState()
            {
                InfoBarSeverity=InfoBarSeverity.Warning,
                StateInfoText=LanguageService.GetResources("/Home/StatusInfoWarning"),
                StatePrRingActValue=false,
                StatePrRingVisValue=false
            },
            new StatusBarState()
            {
                InfoBarSeverity=InfoBarSeverity.Error,
                StateInfoText=LanguageService.GetResources("/Home/StatusInfoError"),
                StatePrRingActValue=false,
                StatePrRingVisValue=false
            }
        };

        public StatusBarViewModel()
        {
            Messenger.Register(this, (MessageHandler<StatusBarViewModel, StatusBarStateMessage>)((statusbarViewModel, statusBarStateMessage) =>
                        {
                            statusbarViewModel.InfoBarSeverity = StatusBarStateList[statusBarStateMessage.Value].InfoBarSeverity;
                            statusbarViewModel.StateInfoText = StatusBarStateList[statusBarStateMessage.Value].StateInfoText;
                            statusbarViewModel.StatePrBarVisValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingVisValue;
                            statusbarViewModel.StatePrBarActValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingActValue;
                        }));
        }
    }
}
