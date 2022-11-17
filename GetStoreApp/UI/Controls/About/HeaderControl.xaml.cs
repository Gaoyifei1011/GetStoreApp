using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class HeaderControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public HeaderViewModel ViewModel { get; } = ContainerHelper.GetInstance<HeaderViewModel>();

        public HeaderControl()
        {
            InitializeComponent();
        }

        private string LocalizeAppVersion(string appVersion)
        {
            return string.Format(ResourceService.GetLocalized("/About/AppVersion"), appVersion);
        }
    }
}
