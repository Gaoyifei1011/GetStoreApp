using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
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

        // 关闭窗口
        public IRelayCommand CloswWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            DisplayTimer.Tick -= DisplayTimerTick;
            dialog.Hide();
        });

        /// <summary>
        /// 对话框加载完成后让内容对话框的烟雾层背景（SmokeLayerBackground）覆盖到标题栏中
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            ContentDialog dialog = sender as ContentDialog;

            if (dialog is not null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(dialog);

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject current = VisualTreeHelper.GetChild(parent, i);
                    if (current is Rectangle { Name: "SmokeLayerBackground" } background)
                    {
                        background.Margin = new Thickness(0);
                        background.RegisterPropertyChangedCallback(FrameworkElement.MarginProperty, OnMarginChanged);
                        break;
                    }
                }
            }

            DisplayTimer.Tick += DisplayTimerTick;
            DisplayTimer.Interval = new TimeSpan(0, 0, 1);
        }

        /// <summary>
        /// 重置内容对话框烟雾背景距离顶栏的间隔
        /// </summary>
        private void OnMarginChanged(DependencyObject sender, DependencyProperty property)
        {
            if (property == FrameworkElement.MarginProperty)
            {
                sender.ClearValue(property);
            }
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
