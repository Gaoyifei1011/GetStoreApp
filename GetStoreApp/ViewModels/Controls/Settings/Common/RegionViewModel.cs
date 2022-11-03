using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Common;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public class RegionViewModel : ObservableRecipient
    {
        private IRegionService RegionService { get; } = ContainerHelper.GetInstance<IRegionService>();

        public List<RegionModel> RegionList => RegionService.RegionList;

        private RegionModel _region;

        public RegionModel Region
        {
            get { return _region; }

            set { SetProperty(ref _region, value); }
        }

        // 应用在应用商店对应的区域选择
        public IRelayCommand RegionSelectCommand => new RelayCommand(async () =>
        {
            await RegionService.SetRegionAsync(Region);
        });

        public RegionViewModel()
        {
            Region = RegionService.AppRegion;
        }
    }
}
