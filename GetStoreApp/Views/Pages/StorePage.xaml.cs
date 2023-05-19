using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 微软商店页面
    /// </summary>
    public sealed partial class StorePage : Page
    {
        private AppNaviagtionArgs StoreNavigationArgs { get; set; } = AppNaviagtionArgs.None;

        public StorePage()
        {
            InitializeComponent();
            Request.ViewModel.InitializeStorePageOtherViewModel(HistoryLite.ViewModel, StatusBar.ViewModel, Result.ViewModel);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            ViewModel.OnNavigatedTo();
            if (args.Parameter is not null)
            {
                object[] navigationArgs = args.Parameter as object[];
                StoreNavigationArgs = (AppNaviagtionArgs)navigationArgs[0];
                if (navigationArgs.Length == 4)
                {
                    Request.ViewModel.SelectedType = Request.ViewModel.TypeList.Find(item => item.InternalName.Equals(navigationArgs[1]));
                    Request.ViewModel.SelectedChannel = Request.ViewModel.ChannelList.Find(item => item.InternalName.Equals(navigationArgs[2]));
                    Request.ViewModel.LinkText = Convert.ToString(navigationArgs[3]);
                }
            }
            else
            {
                StoreNavigationArgs = AppNaviagtionArgs.None;
            }

            if (HistoryLite.ViewModel.HistoryLiteItem != HistoryRecordService.HistoryLiteNum)
            {
                HistoryLite.ViewModel.HistoryLiteItem = HistoryRecordService.HistoryLiteNum;
            }
            await HistoryLite.ViewModel.GetHistoryLiteDataListAsync();
        }

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        public void StoreLoaded(object sender, RoutedEventArgs args)
        {
            if (StoreNavigationArgs is AppNaviagtionArgs.Store)
            {
                StoreScroll.ChangeView(null, 0, null);
            }
        }
    }
}
