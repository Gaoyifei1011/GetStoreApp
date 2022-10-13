using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models.Home;
using Microsoft.UI.Xaml.Controls;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Collections.Generic;
using Windows.Win32.Foundation;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class StatusBarViewModel : ObservableRecipient
    {
        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public List<StatusBarStateModel> StatusBarStateList => ResourceService.StatusBarStateList;

        private TaskbarManager Taskbar = TaskbarManager.Instance;

        private HWND hwnd = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

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

        private bool _statePrRingActValue = false;

        public bool StatePrRingActValue
        {
            get { return _statePrRingActValue; }

            set { SetProperty(ref _statePrRingActValue, value); }
        }

        private bool _statePrRingVisValue = false;

        public bool StatePrRingVisValue
        {
            get { return _statePrRingVisValue; }

            set { SetProperty(ref _statePrRingVisValue, value); }
        }

        // 页面被卸载时，关闭消息服务和事件
        public IRelayCommand UnloadedCommand => new RelayCommand(() =>
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            PropertyChanged -= OnStatusPropertyChanged;
        });

        public StatusBarViewModel()
        {
            StateInfoText = ResourceService.GetLocalized("/Home/StatusInfoWelcome");

            Taskbar.SetProgressState(TaskbarProgressBarState.NoProgress, hwnd);

            WeakReferenceMessenger.Default.Register(this, (MessageHandler<StatusBarViewModel, StatusBarStateMessage>)((statusbarViewModel, statusBarStateMessage) =>
                        {
                            statusbarViewModel.InfoBarSeverity = StatusBarStateList[statusBarStateMessage.Value].InfoBarSeverity;
                            statusbarViewModel.StateInfoText = StatusBarStateList[statusBarStateMessage.Value].StateInfoText;
                            statusbarViewModel.StatePrRingVisValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingVisValue;
                            statusbarViewModel.StatePrRingActValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingActValue;
                        }));

            PropertyChanged += OnStatusPropertyChanged;
        }

        /// <summary>
        /// 状态栏发生变化时通知任务栏做出相应的变化
        /// </summary>
        private void OnStatusPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatePrRingActValue))
            {
                if (StatePrRingActValue)
                {
                    Taskbar.SetProgressState(TaskbarProgressBarState.Indeterminate, hwnd);
                }
                else
                {
                    Taskbar.SetProgressState(TaskbarProgressBarState.NoProgress, hwnd);
                }
            }
        }
    }
}
