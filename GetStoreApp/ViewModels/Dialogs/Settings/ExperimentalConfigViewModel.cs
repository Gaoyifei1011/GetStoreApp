using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.ViewModels.Dialogs.Settings
{
    /// <summary>
    /// 实验性功能配置对话框视图模型
    /// </summary>
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

            Messenger.Default.Send(true, MessageToken.RestoreDefault);
        });

        // 关闭对话框
        public IRelayCommand CloseDialogCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            DisplayTimer.Tick -= DisplayTimerTick;
            dialog.Hide();
        });

        /// <summary>
        /// 初始化计时器
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
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
    }
}
