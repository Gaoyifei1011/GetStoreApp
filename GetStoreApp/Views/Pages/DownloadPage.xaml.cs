using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Window;
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
        }

        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            await Downloading.StartDownloadingTimerAsync();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);
            Downloading.StopDownloadingTimer(true);
            DownloadSchedulerService.DownloadingList.CollectionChanged -= Unfinished.OnDownloadingListItemsChanged;
            DownloadSchedulerService.DownloadingList.CollectionChanged -= Completed.OnDownloadingListItemsChanged;
        }

        /// <summary>
        /// DownloadPivot选中项发生变化时，关闭离开页面的事件，开启要导航到的页面的事件，并更新新页面的数据
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                Pivot pivot = sender as Pivot;
                if (pivot is not null)
                {
                    if (pivot.SelectedIndex is 0)
                    {
                        await Downloading.StartDownloadingTimerAsync();
                    }
                    else if (pivot.SelectedIndex is 1)
                    {
                        Downloading.StopDownloadingTimer(false);
                        await Unfinished.GetUnfinishedDataListAsync();
                    }
                    else if (pivot.SelectedIndex is 2)
                    {
                        Downloading.StopDownloadingTimer(false);
                        await Completed.GetCompletedDataListAsync();
                    }
                }
            }
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        public void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            DownloadFlyout.Hide();
            NavigationService.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }

        /// <summary>
        /// 了解更多下载管理说明
        /// </summary>
        public void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            DownloadFlyout.Hide();
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }
    }
}
