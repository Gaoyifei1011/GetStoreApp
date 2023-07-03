using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.WindowsAPI.Controls;
using GetStoreApp.WindowsAPI.Controls.Taskbar;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.ViewModels.Controls.Store
{
    /// <summary>
    /// 微软商店页面：状态栏用户控件视图模型
    /// </summary>
    public sealed class StatusBarViewModel : ViewModelBase
    {
        private TaskbarManager Taskbar { get; } = TaskbarManager.Instance;

        private List<StatusBarStateModel> StatusBarStateList => ResourceService.StatusBarStateList;

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
            StateInfoText = ResourceService.GetLocalized("Store/StatusInfoWelcome");

            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// 设置控件的状态
        /// </summary>
        public void SetControlState(int state)
        {
            InfoBarSeverity = StatusBarStateList[state].InfoBarSeverity;
            StateInfoText = StatusBarStateList[state].StateInfoText;
            StatePrRingVisValue = StatusBarStateList[state].StatePrRingVisValue;
            StatePrRingActValue = StatusBarStateList[state].StatePrRingActValue;
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
                    Taskbar.SetProgressState(TBPFLAG.TBPF_INDETERMINATE, Program.ApplicationRoot.MainWindow.Handle);
                }
                else
                {
                    Taskbar.SetProgressState(TBPFLAG.TBPF_NOPROGRESS, Program.ApplicationRoot.MainWindow.Handle);
                }
            }
        }
    }
}
