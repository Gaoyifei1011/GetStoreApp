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
            // 修改StatImage的图标，根据传入的Enum值确定显示的图标状态
            Messenger.Register<StatusBarViewModel, StateImageModeMessage>(this, (statusBarViewModel, stateImageModeMessage) =>
            {
                statusBarViewModel.StateImage = stateImageModeMessage.Value;
            });

            // 修改StatusInfo文本
            Messenger.Register<StatusBarViewModel, StateInfoTextMessage>(this, (statusBarViewModel, stateInfoTextMessage) =>
            {
                statusBarViewModel.StateInfoText = stateInfoTextMessage.Value;
            });

            // 设置StatusProgressring激活和显示状态
            Messenger.Register<StatusBarViewModel, StatePrRingActValueMessage>(this, (statusbarViewModel, statePrRingActValueMessage) =>
            {
                statusbarViewModel.StatePrRingActValue = statePrRingActValueMessage.Value;
            });

            Messenger.Register<StatusBarViewModel, StatePrRingVisValueMessage>(this, (statusbarViewModel, statePrRingVisValueMessage) =>
            {
                statusbarViewModel.StatePrRingVisValue = statePrRingVisValueMessage.Value;
            });
        }
    }
}