using GalaSoft.MvvmLight.Messaging;
using GetStoreApp.Core.Models;
using GetStoreApp.Services.Settings;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class RegionViewModel : ObservableObject
    {
        // 区域设置
        private string _selectedRegion = RegionSelectorService.RegionCodeName;

        public string SelectedRegion
        {
            get { return _selectedRegion; }

            set
            {
                Messenger.Default.Send(value, "SelectedRegion");
                RegionSelectorService.SetRegion(value);
            }
        }

        // 区域列表
        public List<GeographicalLocationModel> RegionList = RegionSelectorService.AppGlobalLocations;
    }
}
