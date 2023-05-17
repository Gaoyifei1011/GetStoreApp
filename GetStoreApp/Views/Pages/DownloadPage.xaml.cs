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
            ViewModel.OnNavigatedTo();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);
            ViewModel.OnNavigatedFrom();
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        public void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            DownloadTeachingTip.IsOpen = false;
            ViewModel.OpenDownloadSettings();
        }

        /// <summary>
        /// 了解更多下载管理说明
        /// </summary>
        public void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            DownloadTeachingTip.IsOpen = false;
            ViewModel.LearnMore();
        }

        /// <summary>
        /// 显示下载管理说明内容
        /// </summary>
        public void OnTeachingTipClicked(object sender, RoutedEventArgs args)
        {
            DownloadTeachingTip.IsOpen = true;
        }
    }
}
