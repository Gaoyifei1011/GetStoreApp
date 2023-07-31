using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Window;
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
            Request.InitializeStorePageControl(HistoryLite, StatusBar, Result);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (args.Parameter is not null)
            {
                object[] navigationArgs = args.Parameter as object[];
                StoreNavigationArgs = (AppNaviagtionArgs)navigationArgs[0];
                if (navigationArgs.Length is 4)
                {
                    Request.SelectedType = Request.TypeList.Find(item => item.InternalName.Equals(navigationArgs[1]));
                    Request.SelectedChannel = Request.ChannelList.Find(item => item.InternalName.Equals(navigationArgs[2]));
                    Request.LinkText = Convert.ToString(navigationArgs[3]);
                }
            }
            else
            {
                StoreNavigationArgs = AppNaviagtionArgs.None;
            }

            if (HistoryLite.HistoryLiteItem != HistoryRecordService.HistoryLiteNum)
            {
                HistoryLite.HistoryLiteItem = HistoryRecordService.HistoryLiteNum;
            }
            await HistoryLite.GetHistoryLiteDataListAsync();
        }

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (StoreNavigationArgs is AppNaviagtionArgs.Store)
            {
                StoreScroll.ChangeView(null, 0, null);
            }
        }

        /// <summary>
        /// 了解应用具体的使用说明
        /// </summary>
        public void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.Instructions);
        }
    }
}
