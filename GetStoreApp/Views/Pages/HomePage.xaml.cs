using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class HomePage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public HomeViewModel ViewModel { get; } = IOCHelper.GetService<HomeViewModel>();

        public HomePage()
        {
            InitializeComponent();
        }

        // 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        public void HomeLoaded(object sender, RoutedEventArgs args)
        {
            if (App.NavigationArgs == "Home")
            {
                HomeScroll.ChangeView(null, 0, null);
                App.NavigationArgs = string.Empty;
            }
        }
    }
}
