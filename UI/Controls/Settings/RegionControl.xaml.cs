using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
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
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<RegionViewModel>();
            InitializeComponent();
        }

        public string GetSelectedRegionName(RegionModel region)
        {
            return region.FriendlyName;
        }
    }
}
