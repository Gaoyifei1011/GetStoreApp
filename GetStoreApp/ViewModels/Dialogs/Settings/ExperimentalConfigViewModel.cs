using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Contracts.Services.Controls.Settings.Experiment;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.ViewModels.Dialogs.Settings
{
    public class ExperimentalConfigViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = ContainerHelper.GetInstance<IAria2Service>();

        private INetWorkMonitorService NetWorkMonitorService = ContainerHelper.GetInstance<INetWorkMonitorService>();

        private DispatcherTimer DisplayTimer = new DispatcherTimer();

        private int CountDown = 0;

        private bool _isMessageVisable = false;

        public bool IsMessageVisable
        {
            get { return _isMessageVisable; }

            set { SetProperty(ref _isMessageVisable, value); }
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
