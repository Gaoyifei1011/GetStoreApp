using GetStoreAppInstaller.UI.Backdrop;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 应用主页面
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Background = new MicaBrush(MicaKind.BaseAlt, true);
        }

        private async void OnLearnProjectPlanClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        }
    }
}
