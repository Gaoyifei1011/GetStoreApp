using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Behaviors;
using GetStoreApp.Messages;
using GetStoreApp.ViewModels.Pages;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class StatusBarViewModel : ObservableRecipient
    {
        // 初始化设置Main_Status_Image图标：提示状态
        private StateImageMode _statImage = StateImageMode.Notification;

        public StateImageMode StateImage
        {
            get { return _statImage; }

            set { SetProperty(ref _statImage, value); }
        }

        // 初始化设置Main_Status_Info文本：欢迎使用
        private string _stateInfoText = HomeViewModel.StatusInfoWelcome;

        public string StateInfoText
        {
            get { return _stateInfoText; }

            set { SetProperty(ref _stateInfoText, value); }
        }

        // Main_Status_Progressring的激活状态
        private bool _statePrRingActValue = false;

        public bool StatePrRingActValue
        {
            get { return _statePrRingActValue; }

            set { SetProperty(ref _statePrRingActValue, value); }
        }

        // Main_Status_Progressring的显示状态
        private bool _statePrRingVisValue = false;

        public bool StatePrRingVisValue
        {
            get { return _statePrRingVisValue; }

            set { SetProperty(ref _statePrRingVisValue, value); }
        }

        public StatusBarViewModel()
        {
            Messenger.Register(this, (MessageHandler<StatusBarViewModel, StatusBarStateMessage>)((statusbarViewModel, statusBarStateMessage) =>
                        {
                            statusbarViewModel.StateImage = HomeViewModel.StatusBarStateList[statusBarStateMessage.Value].StateImageMode;
                            statusbarViewModel.StateInfoText = HomeViewModel.StatusBarStateList[statusBarStateMessage.Value].StateInfoText;
                            statusbarViewModel.StatePrRingVisValue = HomeViewModel.StatusBarStateList[statusBarStateMessage.Value].StatePrRingVisValue;
                            statusbarViewModel.StatePrRingActValue = HomeViewModel.StatusBarStateList[statusBarStateMessage.Value].StatePrRingActValue;
                        }));
        }
    }
}
