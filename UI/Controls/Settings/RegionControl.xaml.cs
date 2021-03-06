using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class RegionControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public RegionViewModel ViewModel { get; }

        public RegionControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<RegionViewModel>();
            this.InitializeComponent();
        }
    }
}
