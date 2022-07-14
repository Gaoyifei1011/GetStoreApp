using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class HeaderControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public HeaderViewModel ViewModel { get; }

        public HeaderControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<HeaderViewModel>();
            this.InitializeComponent();
        }

        public string LocalizedAppVersion(string appVersion)
        {
            return string.Format(ResourceService.GetLocalized("/About/AppVersion"), appVersion);
        }
    }
}
