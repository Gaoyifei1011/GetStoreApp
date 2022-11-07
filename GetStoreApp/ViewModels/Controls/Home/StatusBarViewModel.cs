using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.Home;
using GetStoreAppWindowsAPI.Controls;
using GetStoreAppWindowsAPI.Controls.Taskbar;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class StatusBarViewModel : ObservableRecipient
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private TaskbarManager Taskbar { get; } = TaskbarManager.Instance;

        private IntPtr WindowHandle { get; } = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

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

            WeakReferenceMessenger.Default.Register<StatusBarViewModel, StatusBarStateMessage>(this, (statusbarViewModel, statusBarStateMessage) =>
            {
                statusbarViewModel.InfoBarSeverity = StatusBarStateList[statusBarStateMessage.Value].InfoBarSeverity;
                statusbarViewModel.StateInfoText = StatusBarStateList[statusBarStateMessage.Value].StateInfoText;
                statusbarViewModel.StatePrRingVisValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingVisValue;
                statusbarViewModel.StatePrRingActValue = StatusBarStateList[statusBarStateMessage.Value].StatePrRingActValue;
            });

            WeakReferenceMessenger.Default.Register<StatusBarViewModel, WindowClosedMessage>(this, (statusbarViewModel, windowClosedMessage) =>
            {
                if (windowClosedMessage.Value)
                {
                    PropertyChanged -= OnPropertyChanged;
                    WeakReferenceMessenger.Default.UnregisterAll(this);
                }
            });

            PropertyChanged += OnPropertyChanged;
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
                    Taskbar.SetProgressState(TaskbarProgressBarState.Indeterminate, WindowHandle);
                }
                else
                {
                    Taskbar.SetProgressState(TaskbarProgressBarState.NoProgress, WindowHandle);
                }
            }
        }
    }
}
