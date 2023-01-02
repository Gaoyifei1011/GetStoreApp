using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
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

        // 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        public void HomeLoaded(object sender, RoutedEventArgs args)
        {
            if (App.Current.NavigationArgs == AppNaviagtionArgs.Home)
            {
                HomeScroll.ChangeView(null, 0, null);
                App.Current.NavigationArgs = AppNaviagtionArgs.None;
            }
        }
    }
}
