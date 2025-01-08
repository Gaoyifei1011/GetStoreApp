using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 微软商店页面
    /// </summary>
    public sealed partial class StorePage : Page
    {
        public StorePage()
        {
            InitializeComponent();
            StoreSelectorBar.SelectedItem = StoreSelectorBar.Items[0];
        }

        #region 第二部分：应用商店页面——挂载的事件

        /// <summary>
        /// 分割控件选中项发生改变时引发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectorBarSelectionChangedEventArgs args)
        {
            if (sender is SelectorBar selectorBar && selectorBar.SelectedItem is not null)
            {
                int selectedIndex = selectorBar.Items.IndexOf(selectorBar.SelectedItem);

                if (selectedIndex is 0)
                {
                    QueryLinks.Visibility = Visibility.Visible;
                    SearchStore.Visibility = Visibility.Collapsed;
                }
                else if (selectedIndex is 1)
                {
                    QueryLinks.Visibility = Visibility.Collapsed;
                    SearchStore.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// 打开设置中的语言和区域
        /// </summary>
        private async void OnLanguageAndRegionClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:regionformatting"));
        }

        /// <summary>
        /// 了解应用具体的使用说明
        /// </summary>
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.Instructions);
        }

        #endregion 第二部分：应用商店页面——挂载的事件
    }
}
