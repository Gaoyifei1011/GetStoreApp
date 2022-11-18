using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Messages;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.ViewModels.Dialogs.Settings
{
    public sealed class ExperimentalConfigViewModel : ViewModelBase
    {
        private DispatcherTimer DisplayTimer = new DispatcherTimer();

        private int CountDown = 0;

        private bool _isMessageVisable = false;

        public bool IsMessageVisable
        {
            get { return _isMessageVisable; }

            set
            {
                _isMessageVisable = value;
                OnPropertyChanged();
            }
        }

        public ExperimentalConfigViewModel()
        {
            DisplayTimer.Tick += DisplayTimerTick;
            DisplayTimer.Interval = new TimeSpan(0, 0, 1);
        }

        private void DisplayTimerTick(object sender, object e)
        {
            if (CountDown > 0)
            {
                CountDown--;
            }
            else
            {
                IsMessageVisable = false;
                DisplayTimer.Stop();
            }
        }

        // 还原默认值
        public IRelayCommand RestoreDefualtCommand => new RelayCommand(async () =>
        {
            await Aria2Service.RestoreDefaultAsync();
            await NetWorkMonitorService.RestoreDefaultValueAsync();

            if (!DisplayTimer.IsEnabled)
            {
                DisplayTimer.Start();
                IsMessageVisable = true;
            }
            CountDown = 3;

            WeakReferenceMessenger.Default.Send(new RestoreDefaultMessage(true));
        });

        // 关闭窗口
        public IRelayCommand CloswWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            DisplayTimer.Tick -= DisplayTimerTick;
            dialog.Hide();
        });
    }
}
