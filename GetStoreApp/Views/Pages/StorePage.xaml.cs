using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
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
        private AppNaviagtionArgs storeNavigationArgs = AppNaviagtionArgs.None;

        public StorePage()
        {
            InitializeComponent();
            StoreSelectorBar.SelectedItem = StoreSelectorBar.Items[0];
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (args.Parameter is not null)
            {
                object[] navigationArgs = args.Parameter as object[];
                storeNavigationArgs = (AppNaviagtionArgs)navigationArgs[0];
                if (navigationArgs.Length is 4)
                {
                    QueryLinks.SelectedType = QueryLinks.TypeList.Find(item => item.InternalName.Equals(navigationArgs[1]));
                    QueryLinks.SelectedChannel = QueryLinks.ChannelList.Find(item => item.InternalName.Equals(navigationArgs[2]));
                    QueryLinks.LinkText = Convert.ToString(navigationArgs[3]);
                }
            }
            else
            {
                storeNavigationArgs = AppNaviagtionArgs.None;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：应用商店页面——挂载的事件

        /// <summary>
        /// 分割控件选中项发生改变时引发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBar selectorBar = sender as SelectorBar;

            if (selectorBar is not null && selectorBar.SelectedItem is not null)
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
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (storeNavigationArgs is AppNaviagtionArgs.Store)
            {
                StoreScroll.ChangeView(null, 0, null, true);
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
