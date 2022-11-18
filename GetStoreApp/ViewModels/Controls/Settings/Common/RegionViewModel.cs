using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public sealed class RegionViewModel : ViewModelBase
    {
        public List<RegionModel> RegionList => RegionService.RegionList;

        private RegionModel _region;

        public RegionModel Region
        {
            get { return _region; }

            set
            {
                _region = value;
                OnPropertyChanged();
            }
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
