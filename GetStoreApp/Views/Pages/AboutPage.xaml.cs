using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class AboutPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public AboutViewModel ViewModel { get; } = IOCHelper.GetService<AboutViewModel>();

        public AboutPage()
        {
            InitializeComponent();
        }

        // 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        public void AboutLoaded(object sender, RoutedEventArgs args)
        {
            double CurrentScrollPosition = AboutScroll.VerticalOffset;
            Point CurrentPoint = new Point(0, (int)CurrentScrollPosition);

            if (App.NavigationArgs == Instructions.Name)
            {
                Point TargetPosition = Instructions.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                AboutScroll.ChangeView(null, TargetPosition.Y, null);

                App.NavigationArgs = string.Empty;
            }
            else if (App.NavigationArgs == SettingsHelp.Name)
            {
                Point TargetPosition = SettingsHelp.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                AboutScroll.ChangeView(null, TargetPosition.Y, null);

                App.NavigationArgs = string.Empty;
            }
        }
    }
}
