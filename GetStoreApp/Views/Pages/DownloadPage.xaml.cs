using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 下载页面
    /// </summary>
    public sealed partial class DownloadPage : Page
    {
        public DownloadPage()
        {
            InitializeComponent();
            DownloadSelctorBar.SelectedItem = DownloadSelctorBar.Items[0];
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            int selectedIndex = DownloadSelctorBar.Items.IndexOf(DownloadSelctorBar.SelectedItem);
            Downloading.SelectedIndex = selectedIndex;
            Unfinished.SelectedIndex = selectedIndex;
            Completed.SelectedIndex = selectedIndex;

            if (selectedIndex is 0)
            {
                await Downloading.StartDownloadingTimerAsync();
            }
            else if (selectedIndex is 1)
            {
                await Unfinished.GetUnfinishedDataListAsync();
            }
            else if (selectedIndex is 2)
            {
                await Completed.GetCompletedDataListAsync();
            }

            // 订阅事件
            Downloading.DownloadingTimer.Tick += Downloading.DownloadInfoTimerTick;
            DownloadSchedulerService.DownloadingCollection.CollectionChanged += Downloading.OnDownloadingListItemsChanged;
            DownloadSchedulerService.WaitingCollection.CollectionChanged += Downloading.OnWaitingListItemsChanged;
            DownloadSchedulerService.DownloadingCollection.CollectionChanged += Unfinished.OnDownloadingListItemsChanged;
            DownloadSchedulerService.DownloadingCollection.CollectionChanged += Completed.OnDownloadingListItemsChanged;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);
            Downloading.StopDownloadingTimer();

            // 取消订阅事件
            Downloading.DownloadingTimer.Tick -= Downloading.DownloadInfoTimerTick;
            DownloadSchedulerService.DownloadingCollection.CollectionChanged -= Downloading.OnDownloadingListItemsChanged;
            DownloadSchedulerService.WaitingCollection.CollectionChanged -= Downloading.OnWaitingListItemsChanged;
            DownloadSchedulerService.DownloadingCollection.CollectionChanged -= Unfinished.OnDownloadingListItemsChanged;
            DownloadSchedulerService.DownloadingCollection.CollectionChanged -= Completed.OnDownloadingListItemsChanged;
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：下载页面——挂载的事件

        /// <summary>
        /// 下载透视控件选中项发生变化时，关闭离开页面的事件，开启要导航到的页面的事件，并更新新页面的数据
        /// </summary>
        private async void OnSelectionChanged(object sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBar selectorBar = sender as SelectorBar;

            if (selectorBar is not null && selectorBar.SelectedItem is not null)
            {
                int selectedIndex = selectorBar.Items.IndexOf(selectorBar.SelectedItem);

                if (selectedIndex is 0)
                {
                    Downloading.Visibility = Visibility.Visible;
                    Unfinished.Visibility = Visibility.Collapsed;
                    Completed.Visibility = Visibility.Collapsed;

                    await Downloading.StartDownloadingTimerAsync();
                }
                else if (selectedIndex is 1)
                {
                    Downloading.Visibility = Visibility.Collapsed;
                    Unfinished.Visibility = Visibility.Visible;
                    Completed.Visibility = Visibility.Collapsed;

                    Downloading.StopDownloadingTimer();
                    await Unfinished.GetUnfinishedDataListAsync();
                }
                else if (selectedIndex is 2)
                {
                    Downloading.Visibility = Visibility.Collapsed;
                    Unfinished.Visibility = Visibility.Collapsed;
                    Completed.Visibility = Visibility.Visible;

                    Downloading.StopDownloadingTimer();
                    await Completed.GetCompletedDataListAsync();
                }
            }
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        private void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            DownloadFlyout.Hide();
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }

        /// <summary>
        /// 了解更多下载管理说明
        /// </summary>
        private void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            DownloadFlyout.Hide();
            MainWindow.Current.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        #endregion 第二部分：下载页面——挂载的事件
    }
}
