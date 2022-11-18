using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Base;
using GetStoreAppWindowsAPI.Controls;
using GetStoreAppWindowsAPI.Controls.Taskbar;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using WinRT.Interop;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public sealed class StatusBarViewModel : ViewModelBase
    {
        private TaskbarManager Taskbar { get; } = TaskbarManager.Instance;

        public List<StatusBarStateModel> StatusBarStateList => ResourceService.StatusBarStateList;

        private InfoBarSeverity _infoSeverity = InfoBarSeverity.Informational;

        public InfoBarSeverity InfoBarSeverity
        {
            get { return _infoSeverity; }

            set
            {
                _infoSeverity = value;
                OnPropertyChanged();
            }
        }

        private string _stateInfoText;

        public string StateInfoText
        {
            get { return _stateInfoText; }

            set
            {
                _stateInfoText = value;
                OnPropertyChanged();
            }
        }

        private bool _statePrRingActValue = false;

        public bool StatePrRingActValue
        {
            get { return _statePrRingActValue; }

            set
            {
                _statePrRingActValue = value;
                OnPropertyChanged();
            }
        }

        private bool _statePrRingVisValue = false;

        public bool StatePrRingVisValue
        {
            get { return _statePrRingVisValue; }

            set
            {
                _statePrRingVisValue = value;
                OnPropertyChanged();
            }
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
                    Taskbar.SetProgressState(TaskbarProgressBarState.Indeterminate, WindowNative.GetWindowHandle(App.MainWindow));
                }
                else
                {
                    Taskbar.SetProgressState(TaskbarProgressBarState.NoProgress, WindowNative.GetWindowHandle(App.MainWindow));
                }
            }
        }
    }
}
