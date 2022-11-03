using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class HomePage : Page
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public HomeViewModel ViewModel { get; } = ContainerHelper.GetInstance<HomeViewModel>();

        public HomePage()
        {
            InitializeComponent();
        }

        // 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        public void HomeLoaded(object sender, RoutedEventArgs args)
        {
            if (App.NavigationArgs == AppNaviagtionArgs.Home)
            {
                HomeScroll.ChangeView(null, 0, null);
                App.NavigationArgs = AppNaviagtionArgs.None;
            }
        }
    }
}
