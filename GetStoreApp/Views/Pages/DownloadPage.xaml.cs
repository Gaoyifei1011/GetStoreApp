using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
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

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            ViewModel.UseInsVisValue = UseInstructionService.UseInsVisValue;
            Downloading.ViewModel.StartDownloadingTimer();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);
            Downloading.ViewModel.StopDownloadingTimer(true);
            DownloadSchedulerService.DownloadingList.ItemsChanged -= Unfinished.ViewModel.OnDownloadingListItemsChanged;
            DownloadSchedulerService.DownloadingList.ItemsChanged -= Completed.ViewModel.OnDownloadingListItemsChanged;
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
                    if (pivot.SelectedIndex == 0)
                    {
                        Downloading.ViewModel.StartDownloadingTimer();
                    }
                    else if (pivot.SelectedIndex == 1)
                    {
                        Downloading.ViewModel.StopDownloadingTimer(false);
                        await Unfinished.ViewModel.GetUnfinishedDataListAsync();
                    }
                    else if (pivot.SelectedIndex == 2)
                    {
                        Downloading.ViewModel.StopDownloadingTimer(false);
                        await Completed.ViewModel.GetCompletedDataListAsync();
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
            ViewModel.OpenDownloadSettings();
        }

        /// <summary>
        /// 了解更多下载管理说明
        /// </summary>
        public void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            DownloadFlyout.Hide();
            ViewModel.LearnMore();
        }
    }
}
