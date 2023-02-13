using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.WindowsAPI.Controls;
using GetStoreApp.WindowsAPI.Controls.Taskbar;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.ViewModels.Controls.Home
{
    /// <summary>
    /// 主页面：状态栏用户控件视图模型
    /// </summary>
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
            StateInfoText = ResourceService.GetLocalized("Home/StatusInfoWelcome");

            Messenger.Default.Register<int>(this, MessageToken.StatusBarState, (statusBarStateMessage) =>
            {
                InfoBarSeverity = StatusBarStateList[statusBarStateMessage].InfoBarSeverity;
                StateInfoText = StatusBarStateList[statusBarStateMessage].StateInfoText;
                StatePrRingVisValue = StatusBarStateList[statusBarStateMessage].StatePrRingVisValue;
                StatePrRingActValue = StatusBarStateList[statusBarStateMessage].StatePrRingActValue;
            });

            Messenger.Default.Register<bool>(this, MessageToken.WindowClosed, (windowClosedMessage) =>
            {
                if (windowClosedMessage)
                {
                    PropertyChanged -= OnPropertyChanged;
                    Messenger.Default.Unregister(this);
                }
            });

            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// 圆环动画状态修改时修改任务栏的动画显示
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(StatePrRingActValue))
            {
                if (StatePrRingActValue)
                {
                    Taskbar.SetProgressState(TBPFLAG.TBPF_INDETERMINATE, Program.ApplicationRoot.MainWindow.GetMainWindowHandle());
                }
                else
                {
                    Taskbar.SetProgressState(TBPFLAG.TBPF_NOPROGRESS, Program.ApplicationRoot.MainWindow.GetMainWindowHandle());
                }
            }
        }
    }
}
