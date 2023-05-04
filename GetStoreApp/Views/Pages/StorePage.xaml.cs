using GetStoreApp.Extensions.DataType.Enums;
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            ViewModel.OnNavigatedTo();
            if (args.Parameter is not null)
            {
                StoreNavigationArgs = (AppNaviagtionArgs)Enum.Parse(typeof(AppNaviagtionArgs), Convert.ToString(args.Parameter));
            }
            else
            {
                StoreNavigationArgs = AppNaviagtionArgs.None;
            }
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
