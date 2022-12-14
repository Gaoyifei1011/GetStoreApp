using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public sealed class RegionViewModel : ViewModelBase
    {
        public List<RegionModel> RegionList { get; } = RegionService.RegionList;

        private RegionModel _region = RegionService.AppRegion;

        public RegionModel Region
        {
            get { return _region; }

            set
            {
                _region = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 应用在应用商店对应的区域选择
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                Region = args.AddedItems[0] as RegionModel;
                await RegionService.SetRegionAsync(Region);
            }
        }
    }
}
