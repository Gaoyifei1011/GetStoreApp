using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
            if (sender is SelectorBar selectorBar && selectorBar.SelectedItem is not null)
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

        #endregion 第一部分：下载页面——挂载的事件
    }
}
