using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.Home;
using GetStoreAppWindowsAPI.Controls.Taskbar;
using GetStoreAppWindowsAPI.UI.Controls.Taskbar;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class StatusBarViewModel : ObservableRecipient
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private TaskbarManager Taskbar { get; } = TaskbarManager.Instance;

        private IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        public List<StatusBarStateModel> StatusBarStateList => ResourceService.StatusBarStateList;

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

        public StatusBarViewModel()
        {
            StateInfoText = ResourceService.GetLocalized("/Home/StatusInfoWelcome");

            WeakReferenceMessenger.Default.Register(this, (MessageHandler<StatusBarViewModel, StatusBarStateMessage>)((statusbarViewModel, statusBarStateMessage) =>
            {
                statusbarViewModel.InfoBarSeverity = StatusBarStateList[statusBarStateMessage.Value].InfoBarSeverity;
                statusbarViewModel.StateInfoText = StatusBarStateList[statusBarStateMessage.Value].StateInfoText;
                statusbarViewModel.StatePrRingVisValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingVisValue;
                statusbarViewModel.StatePrRingActValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingActValue;
            }));

            PropertyChanged += OnPropertyChanged;
            App.MainWindow.Closed += OnWindowClosed;
        }

        /// <summary>
        /// 圆环动画状态修改时修改任务栏的动画显示
        /// </summary>
        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(StatePrRingActValue))
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

        /// <summary>
        /// 应用关闭后注销所有消息服务，释放所有资源
        /// </summary>
        private void OnWindowClosed(object sender, WindowEventArgs args)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            PropertyChanged -= OnPropertyChanged;
            App.MainWindow.Closed -= OnWindowClosed;
        }
    }
}
