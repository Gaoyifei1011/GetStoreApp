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

        /// <summary>
        /// 关闭对话框后注销计时器
        /// </summary>
        public void OnClosed(object sender, ContentDialogClosedEventArgs args)
        {
            DisplayTimer.Tick -= DisplayTimerTick;
        }

        /// <summary>
        /// 打开对话框时初始化计时器
        /// </summary>
        public void OnOpened(object sender, ContentDialogOpenedEventArgs args)
        {
            DisplayTimer.Tick += DisplayTimerTick;
            DisplayTimer.Interval = new TimeSpan(0, 0, 1);
        }

        /// <summary>
        /// 还原默认值
        /// </summary>
        public async void RestoreDefault()
        {
            await Aria2Service.RestoreDefaultAsync();
            await NetWorkMonitorService.RestoreDefaultValueAsync();

            if (!DisplayTimer.IsEnabled)
            {
                DisplayTimer.Start();
                IsMessageVisable = true;
            }
            CountDown = 3;
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
