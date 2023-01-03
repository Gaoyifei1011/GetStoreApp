using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Services.Controls.Download;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs.Settings
{
    public sealed class RestartAppsViewModel
    {
        // 重启应用
        public IRelayCommand RestartAppsCommand => new RelayCommand<ContentDialog>(async (dialog) =>
        {
            dialog.Hide();
            await RestartAppsAsync();
        });

        // 取消重启应用
        public IRelayCommand CloswWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
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

        /// <summary>
        /// 重启应用，并关闭Aria2下载服务
        /// </summary>
        private async Task RestartAppsAsync()
        {
            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            Messenger.Default.Send(true, MessageToken.WindowClosed);

            // 重启应用
            AppInstance.Restart("");
        }
    }
}
