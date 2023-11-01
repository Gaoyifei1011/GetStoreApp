using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.CustomControls.Navigation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 下载页面
    /// </summary>
    public sealed partial class DownloadPage : Page, INotifyPropertyChanged
    {
        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            Downloading.SelectedIndex = SelectedIndex;
            Unfinished.SelectedIndex = SelectedIndex;
            Completed.SelectedIndex = SelectedIndex;

            if (SelectedIndex is 0)
            {
                await Downloading.StartDownloadingTimerAsync();
            }
            else if (SelectedIndex is 1)
            {
                await Unfinished.GetUnfinishedDataListAsync();
            }
            else
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
        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                Segmented segmented = sender as Segmented;

                if (segmented is not null)
                {
                    SelectedIndex = segmented.SelectedIndex;
                    Downloading.SelectedIndex = segmented.SelectedIndex;
                    Unfinished.SelectedIndex = segmented.SelectedIndex;
                    Completed.SelectedIndex = segmented.SelectedIndex;

                    if (segmented.SelectedIndex is 0)
                    {
                        await Downloading.StartDownloadingTimerAsync();
                    }
                    else if (segmented.SelectedIndex is 1)
                    {
                        Downloading.StopDownloadingTimer();
                        await Unfinished.GetUnfinishedDataListAsync();
                    }
                    else if (segmented.SelectedIndex is 2)
                    {
                        Downloading.StopDownloadingTimer();
                        await Completed.GetCompletedDataListAsync();
                    }
                }
            }
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        private void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            DownloadFlyout.Hide();
            NavigationService.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }

        /// <summary>
        /// 了解更多下载管理说明
        /// </summary>
        private void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            DownloadFlyout.Hide();
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        #endregion 第二部分：下载页面——挂载的事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
