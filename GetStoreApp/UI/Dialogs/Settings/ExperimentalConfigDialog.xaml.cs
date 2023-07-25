using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.UI.Controls.Settings.Experiment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// 实验性功能配置对话框
    /// </summary>
    public sealed partial class ExperimentalConfigDialog : ContentDialog, INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        public ExperimentalConfigDialog()
        {
            InitializeComponent();
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
        public async void OnRestoreDefaultClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            if (sender.Tag is not null)
            {
                ((NetWorkMonitorControl)sender.Tag).NetWorkMonitorValue = true;
                Aria2Service.RestoreDefault();
                await NetWorkMonitorService.RestoreDefaultValueAsync();

                if (!DisplayTimer.IsEnabled)
                {
                    DisplayTimer.Start();
                    IsMessageVisable = true;
                }
                CountDown = 3;
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DisplayTimerTick(object sender, object args)
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
