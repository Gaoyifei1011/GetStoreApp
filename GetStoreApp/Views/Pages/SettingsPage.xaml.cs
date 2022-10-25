using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public SettingsViewModel ViewModel { get; } = IOCHelper.GetService<SettingsViewModel>();

        public SettingsPage()
        {
            InitializeComponent();
        }

        // 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        public void SettingsLoaded(object sender, RoutedEventArgs args)
        {
            double CurrentScrollPosition = SettingsScroll.VerticalOffset;
            Point CurrentPoint = new Point(0, (int)CurrentScrollPosition);

            if (App.NavigationArgs == AppNaviagtionArgs.DownloadOptions)
            {
                Point TargetPosition = DownloadOptions.TransformToVisual(SettingsScroll).TransformPoint(CurrentPoint);
                SettingsScroll.ChangeView(null, TargetPosition.Y, null);

                App.NavigationArgs = AppNaviagtionArgs.None;
            }
        }
    }
}
