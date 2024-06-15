using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

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

        #region 第一部分：下载页面——挂载的事件

        /// <summary>
        /// 下载透视控件选中项发生变化时，关闭离开页面的事件，开启要导航到的页面的事件，并更新新页面的数据
        /// </summary>
        private void OnSelectionChanged(object sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBar selectorBar = sender as SelectorBar;

            if (selectorBar is not null && selectorBar.SelectedItem is not null)
            {
                int selectedIndex = selectorBar.Items.IndexOf(selectorBar.SelectedItem);

                if (selectedIndex is 0)
                {
                    Downloading.Visibility = Visibility.Visible;
                    Completed.Visibility = Visibility.Collapsed;
                }
                else if (selectedIndex is 1)
                {
                    Downloading.Visibility = Visibility.Collapsed;
                    Completed.Visibility = Visibility.Visible;
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

        #endregion 第一部分：下载页面——挂载的事件
    }
}
